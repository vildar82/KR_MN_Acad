using System;
using System.Collections.Generic;

namespace KR_MN_Acad.Spec
{
    public interface ISpecOptions
    {        
        /// <summary>
        /// Типы блоков спецификации
        /// </summary>
        Dictionary<string, Type> TypesBlock { get; set; }
        /// <summary>
        /// Сервис расчета спецификации
        /// </summary>
        ITableService TableService { get; set; }
        bool CheckDublicates { get; set; }
    }
}