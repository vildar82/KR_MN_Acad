using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Элемент групповой спецификации
    /// </summary>
    public interface IConstructionElement : ISpecElement
    {
        IConstructionSize Size { get; set; }
        List<ISpecElement> Elements { get; set; }        
        /// <summary>
        /// Суммирование элементов и заполнение полей строки
        /// items - элементы в одной строке спецификации
        /// </summary>        
        void SumAndSetRow (SpecGroup.SpecGroupRow row, List<ISpecElement> elems);        
    }
}
