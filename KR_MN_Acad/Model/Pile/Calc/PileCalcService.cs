using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using KR_MN_Acad.Model.Pile.Calc.HightMark;
using KR_MN_Acad.Model.Pile.Calc.Spec;

namespace KR_MN_Acad.Model.Pile.Calc
{
    public class PileCalcService
    {
        public static PileOptions PileOptions { get; set; }

        public void Calc(List<Pile> piles, PileOptions pileOpt = null)
        {
            if (pileOpt == null)            
                PileOptions = PileOptions.Load();            
            else            
                PileOptions = pileOpt;

            piles = piles.OrderBy(p => p.Pos).ToList();

            // проверка номеров свай
            CheckNums(piles);

            Inspector.ShowDialog();
            Inspector.Clear();

            // Подсчет отметок в каждой сваи
            foreach (var p in piles)           
                p.CalcHightMarks();

            // Назначение Вида сваям по уникальности параметров
            SetPileViews(piles);

            // Строки таблицы отметок свай
            var hmRows = getHightMarkRows(piles);            

            // Строки спецификации свай
            var specRows = getSpecRows(piles);

            // Форма дерева свай
            FormPiles formPiles = new FormPiles(hmRows, specRows);
            if (Application.ShowModalDialog(formPiles) != System.Windows.Forms.DialogResult.OK)
            {
                formPiles.ButtonDialogVisible(false);
                Application.ShowModelessDialog(formPiles);
                throw new Exception(AcadLib.General.CanceledByUser);
            }

            // Вставка таблицы отметок
            HightMarkTable hmTable = new HightMarkTable(hmRows);
            hmTable.CreateTable();

            // Вставка спец
            SpecTable specTable = new SpecTable(specRows);
            specTable.CreateTable();
        }

        private List<HightMarkRow> getHightMarkRows(List<Pile> piles)
        {
            var res = new List<HightMarkRow>();                        
            var groups = piles.GroupBy(g => new { g.View, g.PileType, g.TopPileAfterBeat, g.TopPileAfterCut, g.BottomRostverk, g.PilePike })
                                .OrderBy(g=>g.Key.View, AcadLib.Comparers.AlphanumComparator.New).ThenByDescending(s=>s.Key.PileType);
            foreach (var g in groups)
            {
                var p = g.FirstOrDefault();
                if (p!= null)
                {
                    var r = new HightMarkRow(p, g.ToList());
                    res.Add(r);
                }                
            }
            return res;
        }

        private List<SpecRow> getSpecRows(List<Pile> piles)
        {
            var res = new List<SpecRow>();
            var groups = piles.GroupBy(g => new { g.View, g.PileType, g.DocLink, g.Name})
                            .OrderBy(g => g.Key.DocLink, AcadLib.Comparers.AlphanumComparator.New)
                            .ThenBy(o=>o.Key.Name, AcadLib.Comparers.AlphanumComparator.New)
                            .ThenByDescending(o=>o.Key.PileType);
            foreach (var g in groups)
            {
                var p = g.FirstOrDefault();
                if (p != null)
                {                    
                    var r = new SpecRow(p, g.ToList());
                    res.Add(r);
                }
            }
            return res;
        }

        /// <summary>
        /// Назначение Вида сваям по уникальным параметрам
        /// </summary>        
        private void SetPileViews (List<Pile> piles)
        {
            var groups = piles.GroupBy(g => g).OrderByDescending(g=>g.Count());
            if (groups.Count()> 13)
            {
                throw new Exception("Ошибка. Определено больше 13 типов свай.");
            }

            var db = HostApplicationServices.WorkingDatabase;
            using (var t = db.TransactionManager.StartTransaction())
            {
                int index = 1;
                foreach (var group in groups)
                {
                    var groupByType = group.GroupBy(g => g.PileType);
                    foreach (var item in groupByType)
                    {
                        var fp = item.First();
                        fp.SetView(index);
                        foreach (var pile in item)
                        {
                            if (pile.View != fp.View)
                            {
                                var num = pile.Pos;
                                CopyPile(db, fp.IdBlRef, pile.IdBtrOwner, pile);
                                pile.View = fp.View;
                                pile.Pos = num;
                                pile.FillPos();
                            }
                        }
                    }                    
                    index++;
                }
                t.Commit();
            }
        }

        /// <summary>
        /// Копирование сваи
        /// </summary>
        /// <param name="db">Чертеж</param>
        /// <param name="idCopy">Свая из которой будет делаться копия</param>
        /// <param name="idOwner">Пространство</param>
        /// <param name="pile">Свая старая в место которой будет вставлена новая. Эта свая будет удалена</param>
        public static void CopyPile (Database db, ObjectId idCopy, ObjectId idOwner, Pile pile)
        {
            using (IdMapping map = new IdMapping())
            {
                var idColCopy = new ObjectIdCollection();
                idColCopy.Add(idCopy);
                db.DeepCloneObjects(idColCopy, idOwner, map, false);
                ObjectId idCopyVal = map[idCopy].Value;

                var blRefCopy = idCopyVal.GetObject(OpenMode.ForWrite) as BlockReference;                

                var matrix = Matrix3d.Displacement(pile.Position - blRefCopy.Position);
                blRefCopy.TransformBy(matrix);                

                var blRefPileOld = pile.IdBlRef.GetObject(OpenMode.ForWrite) as BlockReference;
                blRefPileOld.Erase();    
                            
                pile.Update(blRefCopy);                                
            }
        }

        public static void CheckNums(List<Pile> piles)
        {
            // Проверка последовательности номеров.
            //var sortNums = piles.OrderBy(p => p.Pos);

            // Проверка повторяющихся номеров
            var repeatNums = piles.GroupBy(g => g.Pos).Where(g => g.Skip(1).Any());
            foreach (var repaet in repeatNums)
            {
                foreach (var item in repaet)
                {
                    Inspector.AddError($"Повтор номера {item.Pos}", item.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }

            // Проверка пропущенных номеров
            var minNum = piles.First().Pos;
            var maxNum = piles.Last().Pos;
            var trueSeq = Enumerable.Range(minNum, maxNum - minNum);
            var missNums = trueSeq.Except(piles.Select(p => p.Pos)).Where(p => p > 0);
            foreach (var item in missNums)
            {
                Inspector.AddError($"Пропущен номер сваи {item}", System.Drawing.SystemIcons.Error);
            }

            // Недопустимые номера - меньше 1
            var negateNums = piles.Where(p => p.Pos < 1);
            foreach (var item in negateNums)
            {
                Inspector.AddError($"Недопустимый номер сваи {item.Pos}", item.IdBlRef, System.Drawing.SystemIcons.Error);
            }
        }
    }
}
