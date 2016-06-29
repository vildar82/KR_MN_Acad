using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Monolith.Elements
{
    public class Column : Construction
    {        
        private int length;
        private int width;
        private int height;
        public override string Group { get; set; } = GroupType.MonolithColumn.Name;
        public override int Index { get; set; } = 1;
        public override string Name { get; set; }
        public override string FriendlyName { get; set; } = "Колонна монолитная";

        public Column (string mark, int length, int width, int height, ISpecBlock block) : 
            base("К-", mark, block, 0)
        {
            this.length = length;
            this.width = width;
            this.height = height;
            Name = $"Колонна монолитная, {length}х{width}, h={height}мм";
        }

        public override bool Equals (ISpecElement other)
        {
            var b  = other as Column;
            if (b == null) return false;
            return length == b.length && width == b.width && height == b.height;                
        }

        public override int CompareTo (ISpecElement other)
        {
            var b  = other as Column;
            if (b == null) return -1;
            var res = length.CompareTo(b.length);            
            if (res != 0) return res;
            res = width.CompareTo(b.width);            
            if (res != 0) return res;
            res = height.CompareTo(b.height);
            return res;
        }        
    }
}
