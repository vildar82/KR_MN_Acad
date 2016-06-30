using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Monolith.Elements
{
    public class Beam : Construction
    {        
        private int length;
        private int width;
        private int height;
        public override GroupType Group { get; set; } = GroupType.MonolithBeam;
        public override int Index { get; set; } = 0;
        public override string Name { get; set; }
        public override string FriendlyName { get; set; } = "Балка монолитная";

        public Beam (string mark, int length, int width, int height, ISpecBlock block) : 
            base("Б-", mark, block, 0)
        {
            this.length = length;
            this.width = width;
            this.height = height;
            Name = $"Балка монолитная, {width}х{height}, L={length}мм";
        }

        public override bool Equals (ISpecElement other)
        {
            var b  = other as Beam;
            if (b == null) return false;
            return length == b.length && width == b.width && height == b.height;                
        }

        public override int CompareTo (ISpecElement other)
        {
            var b  = other as Beam;
            if (b == null) return -1;
            var res = width.CompareTo(b.width);
            if (res != 0) return res;
            res = height.CompareTo(b.height);
            if (res != 0) return res;
            res = length.CompareTo(b.length);
            return res;
        }        
    }
}
