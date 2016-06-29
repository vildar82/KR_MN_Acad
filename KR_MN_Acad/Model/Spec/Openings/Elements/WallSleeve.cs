using System;

namespace KR_MN_Acad.Spec.Openings.Elements
{    
    public class WallSleeve: ISpecElement, IOpeningElement
    {
        private int diam;
        private int depth;
        private double elev;

        public string Dimension { get; set; }
        public string Elevation { get; set; }
        public string Mark { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; } = "Гильза";
        public int Index { get; set; } = 1;
        public string Group { get; set; } = string.Empty;
        public ISpecBlock SpecBlock { get; set; }

        public WallSleeve (string mark, int diam, int depth, double elev, string role, string desc, ISpecBlock specBlock)
        {
            SpecBlock = specBlock;
            this.diam = diam;
            this.depth = depth;
            this.elev = elev;
            Elevation = "Ось отв. " + elev.ToString("N3");
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
            var s = other as WallSleeve;
            if (s == null) return false;
            return Mark == s.Mark && diam == s.diam && depth == s.depth && Role == s.Role && elev == s.elev;
        }

        public int CompareTo (ISpecElement other)
        {
            var s = other as WallSleeve;
            if (s == null) return -1;
            int res=0;
            if (!string.IsNullOrEmpty(Mark))
                res = TableService.alpha.Compare(Mark, s.Mark);
            if (res != 0) return res;
            res = diam.CompareTo(s.diam);
            if (res != 0) return res;
            res = depth.CompareTo(s.depth);
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
            return $"{Dimension} {elev} {Role}";
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