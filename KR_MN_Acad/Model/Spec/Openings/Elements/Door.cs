using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Openings.Elements
{
    public class Door : Aperture
    {
        public override string FriendlyName { get; set; } = "Дверь";

        public Door (string mark, int lenght, int height, string elevation, string role, string desc, ISpecBlock specBlock)
            : base(0, "ДП-", mark, lenght, height, elevation, role, desc, specBlock)
        {
            Group = ApertureOptions.GroupDoor;
        }
    }
}
