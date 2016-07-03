using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Spec;
using KR_MN_Acad.Spec.ArmWall.Blocks;
using KR_MN_Acad.Spec.Constructions.Elements;
using KR_MN_Acad.Spec.Elements.Bars;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Описание блока колонны квадратной > 400мм
    /// </summary>
    public class ColumnSquareBigBlock : ColumnBase
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

        public ColumnSquareBigBlock (BlockReference blRef, string blName) : base (blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                Side = Block.GetPropValue<int>(PropNameSide);
                DefineBaseFields(Side, Side, true);
                defineFields();
                
                base.Calculate();
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
            AddElementary(Shackle2);            
        }

        /// <summary>
        /// Определение стороны хомута2
        /// </summary>        
        private int getSideShackle2 ()
        {
            return RoundHelper.RoundWhole(Side * 0.707); 
        }

        protected override void NumberingElementary ()
        {
            base.NumberingElementary();            
            // Хомут2
            FillElemPropNameDesc(Shackle2, PropNameShacklePos2, PropNameShackleDesc2);
        }        
    }
}