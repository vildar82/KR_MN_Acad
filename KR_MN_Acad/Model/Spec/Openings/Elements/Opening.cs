using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Openings.Elements
{
    /// <summary>
    /// Проем
    /// </summary>
    public class Opening : ISpecElement, IOpeningElement
    {        
        private int length;
        private int height;
        private double elev;    
        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Elevation { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public virtual string FriendlyName { get; set; } = "Проем"; 

        /// <summary>
        /// Проемы в стенах - в первых строках
        /// </summary>
        public int Index { get; set; } = 0;
        public GroupType Group { get; set; } = WallOptions.GroupOpening;
        public ISpecBlock SpecBlock { get; set; }
        public double Amount { get; set; } = 0;
        public string Key { get; set; }

        public Opening (string mark, int lenght, int height, double elevation, string role, string desc,
            ISpecBlock specBlock)
        {
            SpecBlock = specBlock;
            Mark = mark;            
            Role = role;
            Count = 1;
            Description = desc;
            Elevation = elevation.ToString("0.000");
            this.length = lenght;
            this.height = height;
            this.elev = elevation;
            Dimension = length + "х" + height + "(h)";
            Key = Dimension + Elevation + Role;
        }

        /// <summary>
        /// Определение номера - Марки
        /// </summary>        
        public virtual string GetNumber (string index)
        {
            return index;
        }

        public void SetNumber (string num)
        {
            Mark = num;
        }

        public bool Equals (ISpecElement other)
        {            
            var s = other as Opening;
            if (s == null) return false;
            return Mark == s.Mark && length == s.length && height == s.height 
                && Role == s.Role && elev == s.elev;
        }

        public int CompareTo (ISpecElement other)
        {
            var s = other as Opening;
            if (s == null) return -1;
            int res =0;
            if(!string.IsNullOrEmpty(Mark))
                res = TableService.alpha.Compare(Mark, s.Mark);
            if (res != 0) return res;
            res = length.CompareTo(s.length);            
            if (res != 0) return res;
            res = height.CompareTo(s.height);
            if (res != 0) return res;
            res = Role.CompareTo(s.Role);
            if (res != 0) return res;
            res = elev.CompareTo(s.elev);
            return res;
        }

        public override int GetHashCode ()
        {
            return Dimension.GetHashCode();
        }

        public string GetParamInfo ()
        {
            return $"{Dimension} {Elevation} {Role}";
        }

        public string GetDesc ()
        {
            return "";
        }

        public void Calc ()
        {            
        }
    }
}
