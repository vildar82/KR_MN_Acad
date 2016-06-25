using System;

namespace KR_MN_Acad.Spec.Slab.Elements
{
    public interface ISlabElement : ISpecElement, IEquatable<ISlabElement>, IComparable<ISlabElement>
    {
        /// <summary>
        /// Индекс группировки строк - если разные элементы в спецификации нужно нумеровать по своим уникальным номерам
        /// </summary>
        int Index { get; }
        string Mark { get; set; }
        string Dimension { get; set; }      
        string Role { get; set; }
        int Count { get; set; }
        string Description { get; set; } 
    }
}