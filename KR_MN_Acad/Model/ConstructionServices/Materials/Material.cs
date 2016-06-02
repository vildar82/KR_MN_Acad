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
        public abstract RowScheme Position { get; set; }

        public abstract string GetPosition(int value);
        public abstract RowScheme GetRow();
        public abstract void Add(IMaterial elem);
        public abstract IMaterial Copy();

        public virtual int CompareTo(RowScheme other)
        {
            var row = GetRow();
            return row.CompareTo(other);
        }
        public virtual bool Equals(RowScheme other)
        {
            var row = GetRow();
            return row.CompareTo(other) == 0;
        }
        public override int GetHashCode()
        {
            return GetRow().GetHashCode();
        }        
    }
}
