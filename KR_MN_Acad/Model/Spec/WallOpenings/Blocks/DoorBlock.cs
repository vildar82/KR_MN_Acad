using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Openings.Blocks;
using KR_MN_Acad.Spec.WallOpenings.Elements;

namespace KR_MN_Acad.Spec.WallOpenings.Blocks
{
    public class DoorBlock : ApertureBase
    {
        public const string BlockName = "КР_Проем_Дверной-Стены";

        public DoorBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
            prefixMark = "ДП-";
        }

        protected override Aperture GetAperture ()
        {
            return new Door(mark, length, height, elev, "АР", desc, this);
        }
    }
}
