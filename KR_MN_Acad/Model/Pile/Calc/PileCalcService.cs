using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Model.Pile.Calc.HightMark;

namespace KR_MN_Acad.Model.Pile.Calc
{
    public class PileCalcService
    {
        public void Calc(List<Pile> piles, PileOptions pileOptions = null)
        {
            if (pileOptions == null)
            {
                pileOptions = PileOptions.Load();
            }

            // Подсчет отметок в каждой сваи
            foreach (var p in piles)           
                p.CalcHightMarks(pileOptions);            
            
            // Расчет отметок свай
            var hmRows = getHightMarkRows(piles);
            hmRows.ForEach(r =>r.CalcNums());           
            // Вставка таблицы
            HightMarkTable hmTable = new HightMarkTable(pileOptions, hmRows);
            hmTable.CreateTable();
        }

        private List<HightMarkRow> getHightMarkRows(List<Pile> piles)
        {
            List<HightMarkRow> res = new List<HightMarkRow>();
            var groups = piles.GroupBy(g => new { g.View, g.TopPileAfterBeat, g.TopPileAfterCut, g.BottomGrillage, g.PilePike }).OrderBy(g=>g.Key.View);
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
    }
}
