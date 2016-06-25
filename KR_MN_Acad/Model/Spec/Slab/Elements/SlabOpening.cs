using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Slab.Elements
{
    /// <summary>
    /// Проем в плите
    /// </summary>
    public class SlabOpening : ISpecElement, ISlabElement
    {        
        private int length;
        private int width;        
        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Проемы в стенах - в первых строках
        /// </summary>
        public int Index { get; set; } = 0;
        public string Group { get; set; } = "";

        public ISpecBlock SpecBlock { get; set; }

        public SlabOpening(string mark, int side1, int side2, string role, string desc, ISpecBlock specBlock)
        {
            SpecBlock = specBlock;
            Mark = mark;            
            Role = role;
            Count = 1;
            Description = desc;
            if (side1 >= side2)
            {
                length = side1;
                width = side2;
            }
            else
            {
                length = side2;
                width = side1;
            }
            Dimension = length + "х" + width;
        }

        /// <summary>
        /// Определение номера - Марки
        /// </summary>        
        public string GetNumber (string index)
        {
            return index;
        }

        public void SetNumber (string num)
        {
            Mark = num;
        }

        public bool Equals (ISpecElement other)
        {            
            var s = other as SlabOpening;
            if (s == null) return false;
            return Mark == s.Mark && length == s.length && width == s.width && Role == s.Role;
        }

        public int CompareTo (ISpecElement other)
        {
            var s = other as SlabOpening;
            if (s == null) return -1;
            int res =0;
            if(!string.IsNullOrEmpty(Mark))
                res = TableService.alpha.Compare(Mark, s.Mark);
            if (res != 0) return res;
            res = length.CompareTo(s.length);            
            if (res != 0) return res;
            res = width.CompareTo(s.width);
            if (res != 0) return res;
            res = Role.CompareTo(s.Role);
            return res;
        }

        public override int GetHashCode ()
        {
            return Dimension.GetHashCode();
        }

        public string GetParamInfo ()
        {
            return $"{Dimension} {Role}";
        }
    }
}
