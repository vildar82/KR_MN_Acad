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
using KR_MN_Acad.Model.Pile.Calc;

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
        private PileNumberingOptions Options { get; set; }
        public PileOptions PileOptions { get; set; }        

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
            Options = new PileNumberingOptions();
            Options.LoadDefault();
            Options = Options.PromptOptions();            

            // Выбор свай для нумерации
            var selblocks = Ed.SelectBlRefs("Выбор блоков свай для нумерации");

            // фильтр блоков свай            
            var piles = PileFilter.Filter(selblocks, PileOptions, false);

            // Сброс положения атрибута номера сваи если задано в настройках
            ResetPos(ref piles);            

            // Определения стороны сваи и проверка ее одинаковости
            Options.PileSide = GetPileSides(ref piles);

            // Проверка дубликатов
            AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));

            // Проверка сваи и расстояний между ними.
            CheckPiles(piles);

            // Сортировка             
            var pilesSort = Sort(piles);

            // Перенумерация
            Num(pilesSort);       
        }        

        private int GetPileSides (ref List<Pile> piles)
        {
            var groupBySide = piles.GroupBy(g => g.Side).OrderByDescending(o => o.Key).ToList();
            if (groupBySide.Skip(1).Any())
            {
                // Ошибка - несколько размеров свай
                string sides = string.Join(", ", groupBySide.Select(s => $"{s.Key} - {s.Count()}шт."));
                Inspector.AddError($"Определено несколько размеров сторон свай: {sides}. Рекомендуется выбирать сваи с одним размером сторон.",
                    System.Drawing.SystemIcons.Error);
            }
            var side = groupBySide.First().Key;
            return side;
        }

        private void CheckPiles(List<Pile> piles)
        {
            STRtree<Pile> rtree = new STRtree<Pile> ();
            foreach (var p in piles)
            {
                var r = getPileEnvelope(p);
                rtree.Insert(r, p);
            }

            var minLenAbs = Options.PileSide * PileOptions.PileRatioLmin;
            var minLen = minLenAbs - 0.1;
            
            List<Pile> pilesErrMinLen = new List<Pile>();

            foreach (var p in piles)
            {                
                var envelope = new Envelope(p.Position.X-minLen, p.Position.X+minLen, p.Position.Y-minLen, p.Position.Y+minLen);                 
                var pilesMinLen = rtree.Query(envelope);                
                foreach (var item in pilesMinLen)
                {
                    if (p == item) continue;
                    if (p.Position.DistanceTo(item.Position) < minLen)
                    {
                        pilesErrMinLen.Add(p);
                    }
                }
            }
            foreach (var item in pilesErrMinLen)
            {
                Inspector.AddError($"Нарушено минимальное расстояние между сваями - Сторона сваи * {PileOptions.PileRatioLmin} = {minLenAbs}. Точка вставки сваи {item.Position}",
                    item.IdBlRef, System.Drawing.SystemIcons.Warning);
            }
        }        

        private List<Pile> Sort(List<Pile> piles)
        {
            List<Pile> resVal;
            var rowWidth = Options.PileSide * (PileOptions.PileRatioLmin==0?1 :PileOptions.PileRatioLmin) * 0.5;

            AcadLib.Comparers.DoubleEqualityComparer comparer = new AcadLib.Comparers.DoubleEqualityComparer(rowWidth);
            if (Options.NumberingOrder == EnumNumberingOrder.RightToLeft)
            {
                // Слева-направо
                resVal = piles.OrderBy(p => p.Position.X).GroupBy(p => p.Position.Y, comparer)
                     .OrderByDescending(g => g.Key).SelectMany(g => g).ToList();
                var leftToR = piles.OrderBy(p => p.Position.X);
            }
            else
            {
                // Сверху-вниз
                resVal = piles.OrderByDescending(p => p.Position.Y).GroupBy(p => p.Position.X, comparer)
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
                    pile.Pos = pos;
                    pile.FillPos();
                    pos++;
                }
                t.Commit();
            }
        }

        private Envelope getPileEnvelope(Pile p)
        {            
            Coordinate c = new Coordinate(p.Position.X, p.Position.Y);
            return new Envelope(c);
        }

        /// <summary>
        /// Сброс положения атрибута номера сваи
        /// </summary>        
        private void ResetPos(ref List<Pile> piles)
        {
            if (Options == null || Options.ResetPos == PileResetEnum.None) return;

            using (var t = Db.TransactionManager.StartTransaction())
            {
                var groupPiles = piles.GroupBy(g => g);
                foreach (var groupByParams in groupPiles)
                {
                    // группировка по виду и типу
                    var groupByViews = groupByParams.GroupBy(g => g.PileType);
                    foreach (var group in groupByViews)
                    {
                        var firstPile = group.First();
                        firstPile.ResetBlock();                        

                        foreach (var pile in group.Skip(1))
                        {                            
                            PileCalcService.CopyPile(Db, firstPile.IdBlRef, firstPile.IdBtrOwner, pile);                            
                        }                        
                    }
                }
                t.Commit();
            }            
        }
    }
}