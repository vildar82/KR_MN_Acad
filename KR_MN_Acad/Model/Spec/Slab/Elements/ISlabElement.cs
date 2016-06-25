using System;

namespace KR_MN_Acad.Spec.Slab.Elements
{
    public interface ISlabElement : ISpecElement
    {        
        string Dimension { get; set; }      
        string Role { get; set; }
        int Count { get; set; }
        string Description { get; set; } 
    }
}