using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Openings.Blocks;
using KR_MN_Acad.Spec.SlabOpenings.Elements;

namespace KR_MN_Acad.Spec.SlabOpenings.Blocks
{
    public class SlabOpeningBlock : SpecBlock
    {
        public const string BlockName = "КР_Отв в плите";
        private const string propMark = "МАРКА";
        private const string propSide1 = "Сторона1";
        private const string propSide2 = "Сторона2";
        private const string propDesc = "ПРИМЕЧАНИЕ";        

        SlabOpening opening;        

        public SlabOpeningBlock(BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            int side1 = Block.GetPropValue<int>(propSide1);
            int side2 = Block.GetPropValue<int>(propSide2);
            string role = SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            opening = new SlabOpening (mark, side1, side2, role, desc, this);
            AddElement(opening);
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, opening.Mark);
        }
    }
}
