using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec
{
    public abstract class SpecBlock : ISpecBlock
    {
        public IBlock Block { get; set; }
        public List<ISpecElement> Elements { get; } = new List<ISpecElement>();
        public Error Error { get { return Block.Error; } }

        public SpecBlock(BlockReference blRef, string blName)
        {
            Block = new BlockBase(blRef, blName);
        }
        
        public abstract void Calculate ();
        public abstract void Numbering ();
    }
}
