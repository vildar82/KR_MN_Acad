using System.Collections.Generic;
using KR_MN_Acad.ConstructionServices.Materials;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme.Spec
{
    public interface ISpecRow
    {        
        List<IElement> Elements { get; set; }
        string PositionColumn { get; set; }
        string DocumentColumn { get; set; }
        string NameColumn { get; set; }
        string CountColumn { get; set; }
        string WeightColumn { get; set; }
        string DescriptionColumn { get; set; }

        void Calculate();
    }
}