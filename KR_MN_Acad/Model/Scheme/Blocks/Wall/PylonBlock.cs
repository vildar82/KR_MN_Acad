using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements.Bars;

namespace KR_MN_Acad.Scheme.Wall
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

        public PylonBlock (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
        {
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                var width = GetPropValue<int>(PropNameWidth);
                var thickness = GetPropValue<int>(PropNameThickness);
                DefineBaseFields(width, thickness, false);                
                defineFields();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }

        public override void Numbering ()
        {
            base.Numbering();
            // Шпилька
            FillElemProp(Spring, PropNameSpringPos, PropNameSpringDesc);
        }

        private void defineFields ()
        {
            // Хомут
            Shackle = defineShackleByLen(PropNameShackleLength, Thickness,a, Height, ArmVertic.Diameter, PropNameShackleDiam,
                PropNameShacklePos, PropNameShackleStep);                    
            // Если есть второй хомут.
            var shackleCount = GetPropValue<int>(PropNameShackleCount);
            if (shackleCount > 1)
            {
                Shackle.Count *= shackleCount;
                Shackle.Calc();
            }            
            AddElement(Shackle);

            // Шпилька
            var springCount = GetPropValue<int>(PropNameSpringCount, false);
            if (springCount != 0)
            {                
                Spring = defineSpring(PropNameShackleDiam, PropNameSpringPos,PropNameShackleStep, Thickness, a, Height, 
                    PropNameSpringCount);
                AddElement(Spring);
            }
        }
    }
}
