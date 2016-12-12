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
        /// <summary>
        /// Если есть дополнительные таблицы (спецификация, детали и т.п.)
        /// </summary>
        List<ITableService> OtherTableService { get; set; }
        bool CheckDublicates { get; set; }
        bool HasBillTable { get; set; }
        bool HasDetailTable { get; set; }
    }
}