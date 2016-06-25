using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Slab.Elements;

namespace KR_MN_Acad.Spec.Slab.Blocks
{
    public class SlabOpeningBlock : ISpecBlock
    {
        public const string BlockName = "КР_Отв в плите";
        private const string propMark = "МАРКА";
        private const string propSide1 = "Сторона1";
        private const string propSide2 = "Сторона2";
        private const string propDesc = "ПРИМЕЧАНИЕ";        

        SlabOpening opening;

        public List<ISpecElement> Elements { get; set; } = new List<ISpecElement>();      

        public Error Error { get { return Block.Error; } }

        public IBlock Block { get; set; }

        public SlabOpeningBlock(BlockReference blRef, string blName)
        {
            Block = new BlockBase(blRef, blName);                        
        }

        public void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            int side1 = Block.GetPropValue<int>(propSide1);
            int side2 = Block.GetPropValue<int>(propSide2);
            string role = SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            opening = new SlabOpening (mark, side1, side2, role, desc, this);
            Elements.Add(opening);
        }        

        public void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, opening.Mark);
        }
    }
}
