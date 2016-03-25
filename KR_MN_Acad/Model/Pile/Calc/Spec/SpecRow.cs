using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Model.Pile.Calc.Spec
{
    public class SpecRow
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
        /// Обозначения
        /// </summary>
        public string DocLink { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Кол
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Масса
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }

        public List<Pile> Piles { get; set; }
        public ObjectId IdBtr { get; set; }
        public ObjectId IdAtrDefPos { get; set; }

        public SpecRow(Pile p, List<Pile> piles)
        {
            View = p.View;
            Piles = piles;
            DocLink = p.DocLink;
            Name = p.Name;
            Count = piles.Count;
            Weight = p.Weight;
            Description = p.Description;
            IdBtr = p.IdBtrAnonym;
            IdAtrDefPos = Pile.GetAttDefpos(IdBtr);
            CalcNums();
        }

        private void CalcNums()
        {
            // Вычислить строку номеров свай
            var sortPos = Piles.OrderBy(p => p.Pos).Select(p => p.Pos);
            Nums = AcadLib.MathExt.IntsToStringSequence(sortPos.ToArray());
        }
    }
}
