using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Monolith.Elements
{
    public interface IConstruction : ISpecElement
    {
        string Designation { get; set; }
        string Name { get; set; }
        int Count { get; set; }
        double WeightUnit  { get; set; }
        double WeightTotal { get; set; }
        string GetWeightUnit ();
        string GetWeightTotal ();        
    }
}
