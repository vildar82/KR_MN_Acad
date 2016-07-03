using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Гильза
    /// </summary>
    public class SleeveBlock : SpecBlock
    {
        public const string BlockName = "КР_Гильза";

        const string PropNameMark = "МАРКА";
        const string PropNameDiam = "Диаметр";
        const string PropNameLength = "Толщина стены";
        const string PropNameThickness = "ТОЛЩИНАГИЛЬЗЫ";
        const string PropNameWeightUnit = "МАССА_МП";

        /// <summary>
        /// Труба
        /// </summary>
        public Tube Tube { get; set; }

        public SleeveBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                defineFields();
                AddElements();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }

        public override void Numbering ()
        {
            // Не нужно ничего заполнять.
        }


        private void AddElements ()
        {
            AddElement(Tube);
        }

        private void defineFields ()
        {
            int diam = Block.GetPropValue<int>(PropNameDiam);
            int len = Block.GetPropValue<int>(PropNameLength);
            int t = Block.GetPropValue<int>(PropNameThickness);
            string wuAtr = Block.GetPropValue<string>(PropNameWeightUnit);
            double wu = double.Parse(wuAtr);
            //string mark = Block.GetPropValue<string>(PropNameMark);
            Tube = new Tube(diam, t, len, wu, this);
            Tube.Calc();
        }        
    }
}
