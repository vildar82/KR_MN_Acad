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
        bool IsDefaultGroupings { get; set; }

        /// <summary>
        /// Суммирование элементов и заполнение полей строки
        /// items - элементы в одной строке спецификации
        /// </summary>        
        void SumAndSetRow (SpecGroupRow row, List<ISpecElement> elems);
        Dictionary<string, List<ISpecElement>> GroupsBySize (IGrouping<int, ISpecElement> indexTypeGroup);
        Dictionary<string, List<ISpecElement>> GroupsByArm (List<ISpecElement> value);
    }
}
