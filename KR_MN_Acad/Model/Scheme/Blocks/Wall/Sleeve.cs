using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Гильза
    /// </summary>
    public class Sleeve : SchemeBlock
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

        public Sleeve (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
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
            int diam = GetPropValue<int>(PropNameDiam);
            int len = GetPropValue<int>(PropNameLength);
            int t = GetPropValue<int>(PropNameThickness);
            string wuAtr = GetPropValue<string>(PropNameWeightUnit);
            double wu = double.Parse(wuAtr);
            string mark = GetPropValue<string>(PropNameMark);
            Tube = new Tube(mark, diam, t, len, wu, this);
            Tube.Calc();
        }        
    }
}
