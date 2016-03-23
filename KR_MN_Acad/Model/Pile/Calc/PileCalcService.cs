using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            {
                PileOptions = PileOptions.Load();
            }
            else
            {
                PileOptions = pileOpt;
            }

            // Подсчет отметок в каждой сваи
            foreach (var p in piles)           
                p.CalcHightMarks();

            // Расчет отметок свай
            var hmRows = getHightMarkRows(piles);
            hmRows.ForEach(r => r.CalcNums());

            // Расчет спецификации свай
            var specRows = getSpecRows(piles);

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
