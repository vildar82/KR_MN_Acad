using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Slab.Blocks
{
    public class SlabOpening : ISpecBlock
    {
        private BlockBase block;

        public SlabRow Row { get; set; }

        public Error Error { get { return block.Error; } }

        public SlabOpening(BlockReference blRef, string blName)
        {
            block = new BlockBase(blRef, blName);                        
        }

        public void Calculate ()
        {
            Row = new SlabRow();
        }

        public List<ISpecElement> GetElements ()
        {
            throw new NotImplementedException();
        }

        public void Numbering ()
        {
            throw new NotImplementedException();
        }
    }
}
