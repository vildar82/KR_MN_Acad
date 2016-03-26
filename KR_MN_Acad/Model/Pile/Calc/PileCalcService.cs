using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
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
            Pile.Check(piles);

            Inspector.ShowDialog();
            Inspector.Clear();

            // Подсчет отметок в каждой сваи
            foreach (var p in piles)           
                p.CalcHightMarks();

            // Расчет отметок свай
            var hmRows = getHightMarkRows(piles);            

            // Расчет спецификации свай
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
            List<HightMarkRow> res = new List<HightMarkRow>();                        
            var groups = piles.GroupBy(g => new { g.View, g.TopPileAfterBeat, g.TopPileAfterCut, g.BottomGrillage, g.PilePike })
                                .OrderBy(g=>g.Key.View, AcadLib.Comparers.AlphanumComparator.New);
            foreach (var g in groups)
            {
                var p = g.FirstOrDefault();
                if (p!= null)
                {
                    HightMarkRow r = new HightMarkRow(p, g.ToList());
                    res.Add(r);
                }                
            }
            return res;
        }

        private List<SpecRow> getSpecRows(List<Pile> piles)
        {
            List<SpecRow> res = new List<SpecRow>();
            var groups = piles.GroupBy(g => new { g.View, g.DocLink, g.Name})
                            .OrderBy(g => g.Key.View, AcadLib.Comparers.AlphanumComparator.New);
            foreach (var g in groups)
            {
                var p = g.FirstOrDefault();
                if (p != null)
                {                    
                    SpecRow r = new SpecRow(p, g.ToList());
                    res.Add(r);
                }
            }
            return res;
        }
    }
}
