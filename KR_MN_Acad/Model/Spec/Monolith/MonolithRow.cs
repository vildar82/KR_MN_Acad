using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Monolith
{
    public class MonolithRow : ISpecRow
    {
        public List<ISpecElement> Elements { get; set; }
        public string Group { get; set; }
        public string Mark { get; set; }
        public string Designation { get; set; }
        public string Name { get; set; }
        public string Count { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }

        public MonolithRow (string group, List<ISpecElement> items)
        {
            Group = group;
            Elements = items;
            var slabElems = items.Cast<Elements.IConstruction>();
            var first = slabElems.First();
            Mark = first.Mark;
            Designation = first.Designation;
            Name = first.Name;
            Count = slabElems.Sum(s => s.Count).ToString();
            Weight = first.GetWeightUnit();
            Description = first.GetWeightTotal();
        }
    }
}
