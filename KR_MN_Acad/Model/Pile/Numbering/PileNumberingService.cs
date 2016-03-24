using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.Errors;
using NetTopologySuite.Index.Strtree;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using GeoAPI.Geometries;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    /// <summary>
    /// Нумерация свай
    /// </summary>
    public class PileNumberingService
    {
        public Document Doc { get; private set; }
        public Database Db { get; private set; }
        public Editor Ed { get; private set; }
        private Options Options { get; set; }
        public PileOptions PileOptions { get; set; }

        private double pileHalfSide;

        public PileNumberingService()
        {
            Doc = Application.DocumentManager.MdiActiveDocument;
            Db = Doc.Database;
            Ed = Doc.Editor;
        }

        /// <summary>
        /// Нумерация свай
        /// </summary>
        public void Numbering()
        {
            // Форма настроек нумерации свай - порядок нумерации, имя блока сваи, имя атрибута номера сваи.
            PileOptions = PileOptions.Load();
            Options = new Options();
            Options.LoadDefault();
            Options = Options.PromptOptions();

            pileHalfSide = Options.PileSide * 0.5;

            // Выбор свай для нумерации
            var selblocks = Ed.SelectBlRefs("Выбор блоков свай для нумерации");

            // фильтр блоков свай            
            var piles = PileFilter.Filter(selblocks, PileOptions);

            // Проверка дубликатов
            AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));

            // Проверка сваи и расстояний между ними.
            CheckPiles(piles);

            // Сортировка             
            var pilesSort = Sort(piles);

            // Перенумерация
            Num(pilesSort);       
        }

        private void CheckPiles(List<Pile> piles)
        {
            STRtree<Pile> rtree = new STRtree<Pile>();
            foreach (var p in piles)
            {
                var r = getPileEnvelope(p);
                rtree.Insert(r, p);
            }

            var minLenAbs = Options.PileSide * PileOptions.PileRatioLmin;
            var minLen = minLenAbs - 0.1;

            HashSet<Pile> pilesHash = new HashSet<Pile>();
            HashSet<Pile> pilesErrMinLen = new HashSet<Pile>();

            foreach (var p in piles)
            {                
                Envelope envelope = new Envelope(p.Pt.X-minLen, p.Pt.X+minLen, p.Pt.Y-minLen, p.Pt.Y+minLen);                 
                var pilesMinLen = rtree.Query(envelope);                
                foreach (var item in pilesMinLen)
                {
                    if (p==item)
                    {
                        pilesHash.Add(item);
                    }

                    if (p.Pt.DistanceTo(item.Pt)<minLen)
                    {
                        if (pilesHash.Add(item))
                        {
                            pilesErrMinLen.Add(p);
                        }
                    }
                }
            }
            foreach (var item in pilesErrMinLen)
            {
                Inspector.AddError($"Нарушено минимальное расстояние между сваями - Сторона сваи * {PileOptions.PileRatioLmin} = {minLenAbs}. Точка вставки сваи {item.Pt}",
                    item.IdBlRef, System.Drawing.SystemIcons.Warning);
            }
        }        

        private List<Pile> Sort(List<Pile> piles)
        {
            List<Pile> resVal;
            var rowWidth = Options.PileSide * PileOptions.PileRatioLmin * 0.5;

            AcadLib.Comparers.DoubleEqualityComparer comparer = new AcadLib.Comparers.DoubleEqualityComparer(rowWidth);
            if (Options.NumberingOrder == EnumNumberingOrder.RightToLeft)
            {
                // Слева-направо
                resVal = piles.OrderBy(p => p.Pt.X).GroupBy(p => p.Pt.Y, comparer)
                     .OrderByDescending(g => g.Key).SelectMany(g => g).ToList();
            }
            else
            {
                // Сверху-вниз
                resVal = piles.OrderByDescending(p => p.Pt.Y).GroupBy(p => p.Pt.X, comparer)
                     .OrderBy(g => g.Key).SelectMany(g => g).ToList();
            }
            return resVal;
        }

        private void Num(List<Pile> piles)
        {
            using (var t = Db.TransactionManager.StartTransaction())
            {
                int pos = Options.PileStartNum;
                foreach (var pile in piles)
                {
                    var atrPos = pile.PosAttrRef.IdAtr.GetObject(OpenMode.ForWrite, false, true) as AttributeReference;
                    atrPos.TextString = pos.ToString();
                    pile.Pos = pos;
                    pos++;
                }
                t.Commit();
            }
        }

        private Envelope getPileEnvelope(Pile p)
        {            
            Coordinate c = new Coordinate(p.Pt.X, p.Pt.Y);
            return new Envelope(c);
        }        
    }
}