using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Materials;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Материал для ведомости расхода стали
    /// </summary>
    public interface IBillMaterial : IMaterial
    {
        /// <summary>
        /// Заголовок выборки расхода стали (ВРС)
        /// Напрример: Изделия арматурные, Изделия закладные
        /// </summary>
        string BillTitle { get; }
        /// <summary>
        /// Группа в ВРС
        /// Например: Арматура класса, Проволока, Прокат марки
        /// </summary>
        string BillGroup { get; }
        /// <summary>
        /// Марка, например: А240С, А500С, Вр1, С245.
        /// </summary>
        string BillMark { get; }
        /// <summary>
        /// Имя ячейки в ВРС.
        /// Например: ∅12, ∅14, -6х40, L63x6
        /// </summary>
        string BillName { get; }        
    }
}
