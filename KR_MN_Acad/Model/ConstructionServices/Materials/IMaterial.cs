using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    public interface IMaterial : IComparable<SchemeRow>, IEquatable<SchemeRow>
    {        
        GroupType Type { get; }
        string Prefix { get; }
        string GetPosition(int value);
        SchemeRow Position { get; set; }

        SchemeRow RowScheme { get; }
        void Add(IMaterial elem);
        IMaterial Copy();
        /// <summary>
        /// Запись для выноски
        /// </summary>
        /// <returns></returns>
        string GetLeaderDesc();
    }
}
