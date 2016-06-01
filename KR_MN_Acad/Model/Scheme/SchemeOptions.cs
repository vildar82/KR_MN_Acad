using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme
{
    [Serializable]
    /// <summary>
    /// Настройки схемы - фильтр блоков и т.п.
    /// </summary>    
    public class SchemeOptions
    {
        public FilterBlocks Filter { get; set; }
        public TableOptions Table { get; set; }        
    }

    public class FilterBlocks
    {
        /// <summary>
        /// Имя блока должно соответствовать этому регулярному выражения Regex.IsMatch(blockName, thisPattern, ignoreCase);
        /// </summary>
        public string BlockNameMatch { get; set; }
    }

    public class TableOptions
    {
        /// <summary>
        /// Название таблицы
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Слой для вставки таблицы
        /// </summary>
        public string Layer { get; set; }
    }
}
