using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec;
using KR_MN_Acad.Spec.ArmWall.Blocks;
using KR_MN_Acad.Spec.Constructions.Elements;
using KR_MN_Acad.Spec.Elements.Bars;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Пилон
    /// </summary>
    public class PylonBlock : ColumnBase
    {
        public const string BlockName = "КР_Арм_Стены_Пилон";

        const string PropNameWidth = "Длина";
        const string PropNameThickness = "Толщина";
        const string PropNameShackleLength = "ДлинаХомута";
        const string PropNameShackleCount = "КолХомутов"; // Хомуты одинаковые!!
        const string PropNameSpringCount = "КолШпилек";
        const string PropNameSpringPos = "ПОЗШПИЛЬКИ";
        const string PropNameSpringDesc = "ОПИСАНИЕШПИЛЬКИ";

        public Spring Spring { get; set; }

        public PylonBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        protected override ISpecElement GetConstruction (string mark)
        {
            Pylon pylon = new Pylon (Width, Thickness, Height, mark, this, Elementary);
            pylon.Calc();
            return pylon;
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                var width = Block.GetPropValue<int>(PropNameWidth);
                var thickness = Block.GetPropValue<int>(PropNameThickness);
                DefineBaseFields(width, thickness, false);                
                defineFields();
                base.Calculate();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }

        protected override void NumberingElementary ()
        {
            base.NumberingElementary();
            // Шпилька
            FillElemPropNameDesc(Spring, PropNameSpringPos, PropNameSpringDesc);
        }

        private void defineFields ()
        {
            // Хомут
            Shackle = defineShackleByLen(PropNameShackleLength, Thickness,a, Height, ArmVertic.Diameter, PropNameShackleDiam,
                PropNameShacklePos, PropNameShackleStep);                    
            // Если есть второй хомут.
            var shackleCount = Block.GetPropValue<int>(PropNameShackleCount);
            if (shackleCount > 1)
            {
                Shackle.AddCount(Shackle.Count);                
            }            
            AddElementary(Shackle);

            // Шпилька
            var springCount = Block.GetPropValue<int>(PropNameSpringCount, false);
            if (springCount != 0)
            {                
                Spring = defineSpring(PropNameShackleDiam, PropNameSpringPos,PropNameShackleStep, Thickness, a, Height, 
                    PropNameSpringCount);
                AddElementary(Spring);
            }
        }        
    }
}
