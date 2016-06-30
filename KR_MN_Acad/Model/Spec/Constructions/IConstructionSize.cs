using System;

namespace KR_MN_Acad.Spec.Constructions
{
    public interface IConstructionSize : IEquatable<IConstructionSize>, IComparable<IConstructionSize>
    {
        int Height { get; set; }
        int Length { get; set; }
        int Width { get; set; }
    }
}