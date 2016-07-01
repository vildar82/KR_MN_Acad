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
        string Key { get; set; }
        GroupType Group { get; set; }
        int Index { get; set; }
        string Mark { get; set; }
        string FriendlyName { get; set; }
        ISpecBlock SpecBlock { get; set; }
        /// <summary>
        /// Масса кг - элемента
        /// </summary>
        double Amount { get; set; }

        /// <summary>
        /// Определение номера по индексу номера строки группы
        /// </summary>
        /// <param name="index">авто индекс</param>
        /// <returns>Индекс для строки элементов - может быть нужен префикс, и т.д.</returns>
        string GetNumber (string index);
        void SetNumber (string num);
        string GetParamInfo ();
        string GetDesc ();
        /// <summary>
        /// Вызывается после создания элемента - для расчета массы и т.д.
        /// </summary>
        void Calc ();        
    }
}
