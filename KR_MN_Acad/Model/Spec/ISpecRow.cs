using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Строка спецификации
    /// </summary>
    public interface ISpecRow
    {
        List<ISpecElement> Elements { get; set; }
        string Group { get; set; } 
    }
}
