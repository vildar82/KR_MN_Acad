using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Monolith.Elements;

namespace KR_MN_Acad.Spec.Monolith.Blocks
{
    public class BeamBlock : MonolithBase
    {
        public const string BlockName ="КР_Балка";
        private const string propHeight = "ВысотаБалки";
        
        public BeamBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }                

        protected override Construction GetConstruction ()
        {
            int height = Block.GetPropValue<int>(propHeight);
            return new Beam(mark, length, width, height, this);
        }
    }
}
