using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Openings.Elements;

namespace KR_MN_Acad.Spec.Openings.Blocks
{
    public class WallOpeningBlock : SpecBlock
    {
        public const string BlockName = "КР_Отв";
        private const string propMark = "МАРКА";
        private const string propLength = "Длина";
        private const string propHeight = "Высота";
        private const string propElevation = "ОТМЕТКА";
        private const string propDesc = "ПРИМЕЧАНИЕ";        

        Opening opening;       

        public WallOpeningBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            int length = Block.GetPropValue<int>(propLength);
            int height = Block.GetPropValue<int>(propHeight);
            double elev = Block.GetPropValue<double>(propElevation);
            string role = SlabOpenings.SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            opening = new Opening (mark, length, height, elev, role, desc, this);
            Elements.Add(opening);
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, opening.Mark);
        }
    }
}
