using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    public interface IMaterial : IComparable<RowScheme>, IEquatable<RowScheme>
    {        
        GroupType Type { get; }
        string Prefix { get; }
        string GetPosition(int value);
        RowScheme Position { get; set; }

        RowScheme GetRow();
        void Add(IMaterial elem);
        IMaterial Copy();
    }
}
