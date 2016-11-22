using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using KR_MN_Acad.Spec.WallOpenings.Elements;

namespace KR_MN_Acad.Spec.WallOpenings
{
    /// <summary>
    /// строка таблицы отверстий в плите
    /// </summary>
    public class OpeningRow : ISpecRow
    {
        public List<ISpecElement> Elements { get; set; }
        public string Group { get; set; }
        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Elevation { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }        

        public OpeningRow (string group, List<ISpecElement> items)
        {            
            var slabElems = items.OfType<IOpeningElement>();
            if (slabElems.Any())
            {
                Group = group;
                Elements = items;
                var first = slabElems.First();
                Mark = first.Mark;
                Dimension = first.Dimension;
                Elevation = first.Elevation;
                Role = first.Role;
                Count = slabElems.Sum(s => s.Count);
                Description = first.Description;
            }
        }        
    }
}
