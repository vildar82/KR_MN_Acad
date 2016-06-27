using System;

namespace KR_MN_Acad.Spec.SlabOpenings.Elements
{
    /// <summary>
    /// Гильза в плите
    /// </summary>
    public class SlabSleeve:ISpecElement, ISlabElement
    {
        private int diam;
        private int depth;
        public string Dimension { get; set; }
        public string Mark { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Гильзы - в конце
        /// </summary>
        public int Index { get; set; } = 1;
        public string Group { get; set; } = "";
        public ISpecBlock SpecBlock { get; set; }

        public SlabSleeve (string mark, int diam, int depth, string role, string desc, ISpecBlock specBlock)
        {
            SpecBlock = specBlock;
            this.diam = diam;
            this.depth = depth;
            Mark = mark;            
            Role = role;
            Description = desc;
            Count = 1;
            Dimension = "Гильза " + AcadLib.General.Symbols.Diam + diam + "х" + depth;
        }                

        public string GetNumber (string index)
        {
            return "Г" + index;
        }

        public void SetNumber (string num)
        {
            Mark = num;
        }

        public bool Equals (ISpecElement other)
        {
            var s = other as SlabSleeve;
            if (s == null) return false;
            return Mark == s.Mark && diam == s.diam && depth == s.depth && Role == s.Role;
        }

        public int CompareTo (ISpecElement other)
        {
            var s = other as SlabSleeve;
            if (s == null) return -1;
            int res=0;
            if (!string.IsNullOrEmpty(Mark))
                res = TableService.alpha.Compare(Mark, s.Mark);
            if (res != 0) return res;
            res = diam.CompareTo(s.diam);
            if (res != 0) return res;
            res = depth.CompareTo(s.depth);
            if (res != 0) return res;
            return Role.CompareTo(s.Role);
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