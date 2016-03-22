﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Model.Pile.Calc.HightMark
{
    public class HightMarkRow
    {
        /// <summary>
        /// Условное обозначение сваи
        /// </summary>
        public string View { get; set; }
        /// <summary>
        /// Номера свай
        /// </summary>
        public string Nums { get; set; }
        /// <summary>
        /// Верх сваи после забивки
        /// </summary>
        public double TopPileAfterBeat { get; set; }
        /// <summary>
        /// Верх сваи после срубки
        /// </summary>
        public double TopPileAfterCut { get; set; }

        public void CalcNums()
        {
            // Вычислить строку номеров свай
            var sortPos = Piles.OrderBy(p => p.Pos).Select(p=>p.Pos);
            Nums = string.Join(",", sortPos);
        }

        /// <summary>
        /// Отметка низа ростверка
        /// </summary>
        public double BottomGrillage { get; set; }
        /// <summary>
        /// Отметка острия сваи
        /// </summary>
        public double PilePike { get; set; }

        public List<Pile> Piles { get; set; }        

        private HashSet<int> _nums { get; set; }

        public HightMarkRow(Pile p, List<Pile> piles)
        {
            View = p.View;
            TopPileAfterBeat = p.TopPileAfterBeat;
            TopPileAfterCut = p.TopPileAfterCut;
            BottomGrillage = p.BottomGrillage;
            Piles = piles;
        }
    }
}