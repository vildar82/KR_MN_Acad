using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Rules
{
    /// <summary>
    /// Правила - проверка блоков
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Проверка блоков
        /// </summary>        
        void Check(List<ISchemeBlock> blocks);
    }
}
