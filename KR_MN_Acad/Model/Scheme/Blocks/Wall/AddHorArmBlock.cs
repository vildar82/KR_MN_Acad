using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements.Bars;

namespace KR_MN_Acad.Scheme.Wall
{
    public class AddHorArmBlock : SchemeBlock
    {
        public const string BlockName = "КР_Арм_Стен_ДопГорАрм";

        const string PropNameRows = "Рядов";
        const string PropNameLength = "Длина";
        const string PropNameHeight = "Высота";
        const string PropNameDiam = "Диаметр";
        const string PropNameStep = "Шаг";
        const string PropNamePos = "ПОЗГОРАРМ";
        const string PropNameDesc = "ОПИСАНИЕГОРАРМ";       
        
        public Bar ArmHor { get; set; }

        public AddHorArmBlock (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
        {
        }

        public override void Calculate ()
        {
            try
            {
                var len = GetPropValue<int>(PropNameLength);
                var height = GetPropValue<int>(PropNameHeight);
                var step = GetPropValue<int>(PropNameStep);
                var rows = GetPropValue<int>(PropNameRows);
                ArmHor = defineBarDiv(len, height, step, PropNameDiam, PropNamePos, rows, "Горизонтальные стержни усиления");
                AddElement(ArmHor);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }            
        }

        public override void Numbering ()
        {
            // ГорАрм         
            FillElemProp(ArmHor, PropNamePos, PropNameDesc);
        }
    }
}
