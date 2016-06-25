using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// элемент спецификации
    /// </summary>
    public interface ISpecElement : IEquatable<ISpecElement>, IComparable<ISpecElement>
    {
        string Group { get; set; }
        int Index { get; set; }
        string Mark { get; set; }
        ISpecBlock SpecBlock { get; set; }
        /// <summary>
        /// Определение номера по индексу номера строки группы
        /// </summary>
        /// <param name="index">авто индекс</param>
        /// <returns>Индекс для строки элементов - может быть нужен префикс, и т.д.</returns>
        string GetNumber (string index);
        /// <summary>
        /// Присвоение номера - Марки
        /// </summary>
        /// <param name="num">Номер - марка для присвоения</param>
        void SetNumber (string num);
        string GetParamInfo ();
    }
}
