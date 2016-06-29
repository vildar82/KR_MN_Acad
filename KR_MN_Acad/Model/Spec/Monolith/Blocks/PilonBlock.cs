using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Monolith.Elements;

namespace KR_MN_Acad.Spec.Monolith.Blocks
{
    public class PilonBlock : MonolithBase
    {
        public const string BlockName ="КР_Пилон";
        private const string propHeight = "Высота";
        
        public PilonBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }                

        protected override Construction GetConstruction ()
        {
            int height = Block.GetPropValue<int>(propHeight);
            return new Pylon(mark, length, width, height, this);
        }
    }
}
