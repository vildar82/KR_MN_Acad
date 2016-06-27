using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Monolith.Elements;

namespace KR_MN_Acad.Spec.Monolith.Blocks
{
    public class ColumnBlock : MonolithBase
    {
        public const string BlockName ="КР_Колонна";
        private const string propHeight = "Высота";
                
        public ColumnBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }        

        protected override Construction GetConstruction ()
        {
            int height = Block.GetPropValue<int>(propHeight);
            return new Elements.Column(mark, length, width, height, this);
        }
    }
}
