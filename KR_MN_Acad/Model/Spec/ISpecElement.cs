using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// элемент спецификации
    /// </summary>
    public interface ISpecElement : IEquatable<ISpecElement>, IComparable<ISpecElement>
    {
        int GetHashCode ();
    }
}
