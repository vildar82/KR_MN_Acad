using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Monolith.Elements;

namespace KR_MN_Acad.Spec.Monolith.Blocks
{
    public abstract class MonolithBase : SpecBlock
    {
        private const string propMark = "МАРКА";
        private const string propLength = "Длина";
        private const string propWidth = "Ширина";
        private const string propDesc = "ПРИМЕЧАНИЕ";

        protected string mark;
        protected int length;
        protected int width;
        protected string desc;

        private Construction construction;

        public MonolithBase (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        protected abstract Construction GetConstruction ();

        public override void Calculate ()
        {
            mark = Block.GetPropValue<string>(propMark);
            length = Block.GetPropValue<int>(propLength);
            width = Block.GetPropValue<int>(propWidth);
            desc = Block.GetPropValue<string>(propDesc);
            construction = GetConstruction();            
            Elements.Add(construction);
        }

        public override void Numbering ()
        {
            Block.FillPropValue(propMark, construction.Mark);
        }
    }
}
