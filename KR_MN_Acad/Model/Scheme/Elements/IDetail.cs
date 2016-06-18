using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;

namespace KR_MN_Acad.Scheme.Elements
{
    /// <summary>
    /// Деталь для ведомости деталей
    /// </summary>
    public interface IDetail
    {        
        /// <summary>
        /// Блок ведомости детали
        /// </summary>
        string BlockNameDetail { get; }
        /// <summary>
        /// Заполнение параметров детали
        /// </summary>
        /// <param name="atrs"></param>
        void SetDetailsParam (List<AttributeInfo> atrs);
    }
}
