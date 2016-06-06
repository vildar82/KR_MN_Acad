using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AcadLib.General;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Арматурная деталь - шпилька, скашка, пешка и т.п.
    /// </summary>
    public class BarDetail : Bar
    {
        public BarDetail(int diam, int len, int count, string prefix, string pos,
            ISchemeBlock block, string friendlyName) 
            : base(diam, len, pos, block, friendlyName)
        {
            Prefix = prefix;
            Type = Spec.GroupType.Details;
            Count = count;            
        }        

        public override int CompareTo(IElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return -1;
            var res = Prefix.CompareTo(other.Prefix);
            if (res != 0)
                return res;
            return base.CompareTo(other);
        }

        public override bool Equals(IElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return false;
            if (Prefix != other.Prefix)
                return false;
            return base.Equals(other);
        }

        public override string GetDesc()
        {
            return $"{SpecRow?.PositionColumn}, {Symbols.Diam}{Diameter}";
        }
    }
}
