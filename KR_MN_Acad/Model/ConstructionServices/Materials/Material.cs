using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    public abstract class Material : IMaterial
    {        
        public abstract string Prefix { get; set; }
        public abstract GroupType Type { get; set; }
        public abstract SchemeRow Position { get; set; }
        public abstract SchemeRow RowScheme { get; }
        public abstract string GetPosition(int value);        
        public abstract void Add(IMaterial elem);
        public abstract IMaterial Copy();
        public abstract string GetLeaderDesc();

        public virtual int CompareTo(SchemeRow other)
        {
            var row = RowScheme;
            return row.CompareTo(other);
        }
        public virtual bool Equals(SchemeRow other)
        {
            var row = RowScheme;
            return row.CompareTo(other) == 0;
        }
        public override int GetHashCode()
        {
            return RowScheme.GetHashCode();
        }
    }
}
