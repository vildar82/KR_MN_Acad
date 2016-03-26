using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        public Icon Icon { get; set; }

        public string Info
        {
            get
            {
                return View + ": Серия=" + DocLink + ", Нименование=" + Name + ", Масса=" + Weight;
            }
        }

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
            IdAtrDefPos = Pile.GetAttDefPos(IdBtr);
            CalcNums();
            setImage();
        }

        private void setImage()
        {
            using (var btr = IdBtr.Open(OpenMode.ForRead) as BlockTableRecord)
            {
                Icon = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreviewIcon(btr);
            }
        }

        private void CalcNums()
        {
            // Вычислить строку номеров свай
            var poses = Piles.Select(p => p.Pos);
            Nums = AcadLib.MathExt.IntsToStringSequence(poses.ToArray());
        }
    }
}
