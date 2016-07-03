using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements.Bars;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    public class AddHorArmBlock : SpecBlock
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

        public AddHorArmBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        public override void Calculate ()
        {
            try
            {
                var len = Block.GetPropValue<int>(PropNameLength);
                var height = Block.GetPropValue<int>(PropNameHeight);
                var step = Block.GetPropValue<int>(PropNameStep);
                var rows = Block.GetPropValue<int>(PropNameRows);
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
            FillElemPropNameDesc(ArmHor, PropNamePos, PropNameDesc);
        }
    }
}
