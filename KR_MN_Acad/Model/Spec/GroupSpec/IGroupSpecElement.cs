using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.SpecGroup
{
    /// <summary>
    /// Элемент групповой спецификации
    /// </summary>
    public interface IGroupSpecElement : ISpecElement
    {
        /// <summary>
        /// Уникальное имя элемента (в переделах элементов IConstruction одного индекса). Нужно для ключа при группировки элементов по индексу
        /// </summary>
        string Key { get; set; }
        /// <summary>
        /// Суммирование элементов и заполнение полей строки
        /// items - элементы в одной строке спецификации
        /// </summary>        
        void SumAndSetRowth (SpecGroupRow row, List<ISpecElement> elems);        
    }
}
