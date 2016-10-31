using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Model.Pile.Calc.HightMark
{
    public class HightMarkRow
    {
        /// <summary>
        /// Условное обозначение сваи
        /// </summary>
        public string View { get; set; }
        public string PileType { get; set; }
        
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
        /// <summary>
        /// Отметка низа ростверка
        /// </summary>
        public double BottomGrillage { get; set; }
        /// <summary>
        /// Отметка острия сваи
        /// </summary>
        public double PilePike { get; set; }

        public List<Pile> Piles { get; set; }
        public ObjectId IdBtr { get; set; }
        public ObjectId IdAtrDefPos { get; set; }

        private HashSet<int> _nums { get; set; }

        public string Info
        {
            get
            {
                return View + ": ВерхЗабивки=" + TopPileAfterBeat + ", ВерхСрубки=" + TopPileAfterCut + ", НизРостверка=" + BottomGrillage + ", тип " + PileType;
            }
        }

        public HightMarkRow(Pile p, List<Pile> piles)
        {
            View = p.View;
            PileType = p.GetPileType();
            TopPileAfterBeat = p.TopPileAfterBeat;
            TopPileAfterCut = p.TopPileAfterCut;
            BottomGrillage = p.BottomRostverk;
            PilePike = p.PilePike;
            Piles = piles;
            IdBtr = p.IdBtrAnonym;
            IdAtrDefPos = Pile.GetAttDefPos(IdBtr);
            CalcNums();            
        }       

        private void CalcNums()
        {
            // Вычислить строку номеров свай
            var poses = Piles.Select(p => p.Pos);
            Nums = AcadLib.MathExt.IntsToStringSequence(poses.ToArray());
        }
    }
}
