using System;

namespace KR_MN_Acad.Spec.Openings.Elements
{
    public interface IOpeningElement : ISpecElement
    {        
        string Dimension { get; set; }
        string Elevation { get; set; }
        string Role { get; set; }
        int Count { get; set; }
        string Description { get; set; } 
    }
}