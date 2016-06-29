using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Openings.Elements
{
    public class Window : Aperture
    {
        public override string FriendlyName { get; set; } = "Окно";

        public Window (string mark, int lenght, int height, double elevation, string role, string desc, ISpecBlock specBlock)
            : base(1, "ОП-", mark, lenght, height, elevation, role, desc, specBlock)
        {
        }
    }
}
