using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Строчка в спецификации материалов схемы армирования
    /// </summary>
    public class SpecRow : IComparable<SpecRow>, IEquatable<SpecRow>, ISpecRow
    {
        public readonly static AcadLib.Comparers.AlphanumComparator Alpha = AcadLib.Comparers.AlphanumComparator.New;
                
        public List<IElement> Elements { get; set; }
        /// <summary>
        /// Позиция - столбец
        /// </summary>
        public string PositionColumn { get;  set; }
        /// <summary>
        /// Обозначение - столбец
        /// </summary>
        public string DocumentColumn { get;  set; }
        /// <summary>
        /// Наименование - столбец
        /// </summary>
        public string NameColumn { get; set; }
        /// <summary>
        /// Кол.
        /// </summary>
        public string CountColumn { get; set; }
        /// <summary>
        /// Масса ед., кг
        /// </summary>
        public string WeightColumn { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string DescriptionColumn { get; set; }

        public IElement SomeElement { get; set; }

        public double Amount { get; set; }

        public SpecRow(string pos, List<IElement> elems)
        {            
            Elements = elems;
            SomeElement = elems.First();
            foreach (var elem in elems)
            {
                elem.SpecRow = this;
            }
            var item = elems.First();            
            PositionColumn = pos;                  
        }

        public int CompareTo(SpecRow other)
        {            
            var result = PositionColumn.CompareTo(other.PositionColumn);
            if (result != 0) return result;
                        
            return 0;            
        }        

        public bool Equals(SpecRow other)
        {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return PositionColumn.GetHashCode();
        }

        // Суммирование элементов
        public void Calculate()
        {
            var elem = Elements.First();
            elem.Sum(Elements);
        }
    }
}
