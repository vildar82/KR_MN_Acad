using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Openings.Blocks;
using KR_MN_Acad.Spec.Openings.Elements;

namespace KR_MN_Acad.Spec.Openings.Blocks
{
    public class WindowBlock : ApertureBase
    {
        public const string BlockName = "КР_Проем_Оконный_Стены";

        public WindowBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
            prefixMark = "ОП-";
        }

        protected override Aperture GetAperture ()
        {
            return new Window(mark, length, height, elev, "АР", desc, this);
        }
    }
}
