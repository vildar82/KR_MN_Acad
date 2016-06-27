using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Openings.Elements
{
    public abstract class Aperture : Opening
    {
        private string prefix;
        public Aperture (int index, string prefix, string mark, int lenght, int height, double elevation, string role, string desc, ISpecBlock specBlock) 
            : base(mark, lenght, height, elevation, role, desc, specBlock)
        {            
            this.prefix = prefix;
            Index = index;              
        }

        public override string GetNumber (string index)
        {
            return prefix + index;
        }
    }
}
