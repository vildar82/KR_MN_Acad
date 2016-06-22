using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока колонны квадратной до 400мм
    /// </summary>
    public class ColumnSquareBig : ColumnBase
    {        
        public const string BlockName = "КР_Арм_Колонна_Квадратная_больше400";

        const string PropNameSide = "Ширина колонны";
        const string PropNameShacklePos2 = "ПОЗХОМУТА2";
        const string PropNameShackleDesc2 = "ОПИСАНИЕХОМУТА2";

        /// <summary>
        /// Сторона колонны (ширина)
        /// </summary>
        public int Side { get; set; }
        /// <summary>
        /// Хомут2
        /// </summary>
        public Shackle Shackle2 { get; set; }

        public ColumnSquareBig (BlockReference blRef, string blName, SchemeService service) : base (blRef, blName, service)
        {            
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                Side = Convert.ToInt32(GetPropValue<double>(PropNameSide));
                DefineBaseFields(Side, Side, true);
                defineFields();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }

        private void defineFields ()
        {
            // Хомут2
            int widthShackle2 = getSideShackle2();
            Shackle2 = defineShackleByGab(widthShackle2, widthShackle2, Height, ArmVertic.Diameter, a, PropNameShackleDiam,
                PropNameShacklePos2, PropNameShackleStep);
            Shackle2.Calc();
            AddElement(Shackle2);            
        }

        /// <summary>
        /// Определение стороны хомута2
        /// </summary>        
        private int getSideShackle2 ()
        {
            return RoundHelper.RoundWhole(Side * 0.707); 
        }

        public override void Numbering ()
        {
            base.Numbering();
            // Хомут2
            FillElemProp(Shackle2, PropNameShacklePos2, PropNameShackleDesc2);
        }
    }
}