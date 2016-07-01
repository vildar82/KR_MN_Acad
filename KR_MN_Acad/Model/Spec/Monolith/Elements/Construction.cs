using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Monolith.Elements
{
    public abstract class Construction : IConstruction
    {
        private string prefix;
        public int Count { get; set; }
        public string Designation { get; set; } = "";
        public abstract GroupType Group { get; set; }
        public abstract int Index { get; set; }
        public string Mark { get; set; }
        public abstract string Name { get; set; }
        public ISpecBlock SpecBlock { get; set; }
        public double WeightTotal { get; set; }
        public double WeightUnit { get; set; }
        public abstract string FriendlyName { get; set; }
        public double Amount { get; set; } = 0;

        public string Key { get; set; }

        public Construction (string prefix, string mark, ISpecBlock block, double weightUnit)
        {            
            this.prefix = prefix;            
            Mark = mark;
            Count = 1;
            Key = Name;
        }

        public virtual string GetNumber (string index)
        {
            return prefix + index;
        }
        public void SetNumber (string num)
        {
            Mark = num;
        }

        public string GetParamInfo ()
        {
            return Name;
        }

        public abstract bool Equals (ISpecElement other);
        public abstract int CompareTo (ISpecElement other);

        public virtual string GetWeightUnit ()
        {
            return "";
        }

        public virtual string GetWeightTotal ()
        {
            return "";
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode();
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
