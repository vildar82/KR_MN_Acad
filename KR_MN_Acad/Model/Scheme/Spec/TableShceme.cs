using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Таблица спецификации материалов схемы армирования
    /// </summary>
    public class TableShceme
    {
        /// <summary>
        /// Название спецификации
        /// </summary>
        public string Title { get; set; }
        public List<GroupScheme> Groups { get; set;}
    }
}
