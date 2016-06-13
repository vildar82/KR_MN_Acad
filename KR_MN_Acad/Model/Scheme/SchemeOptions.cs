using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme
{    
    /// <summary>
    /// Настройки схемы - фильтр блоков и т.п.
    /// </summary>    
    public class SchemeOptions
    {
        public Rules.Rule Rule { get; set; }
        public Dictionary<string, Type> TypesBlock { get; set; }        
        public TableOptions Table { get; set; }      
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

        public TableOptions(string title)
        {
            Title = title;
        }
    }
}
