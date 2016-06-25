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
        string Group { get; set; }        
        /// <summary>
        /// Нумерация элементов.
        /// </summary>
        /// <param name="index">Порядеовый номер строки в группе</param>
        void Numbering (string index);
    }
}
