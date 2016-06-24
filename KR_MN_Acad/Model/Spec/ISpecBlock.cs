using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Блок спецификации
    /// </summary>
    public interface ISpecBlock
    {
        /// <summary>
        /// Описание ошибок в блоке
        /// </summary>
        Error Error { get; }
        /// <summary>
        /// Определение блока - элементов спецификации
        /// </summary>
        void Calculate ();
        /// <summary>
        /// Получение элементов спецификации из блока
        /// </summary>
        /// <returns></returns>
        List<ISpecElement> GetElements ();
        /// <summary>
        /// Заполнение нумерации в блоке
        /// </summary>
        void Numbering ();
    }
}
