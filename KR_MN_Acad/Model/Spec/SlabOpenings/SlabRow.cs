using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using KR_MN_Acad.Spec.SlabOpenings.Elements;

namespace KR_MN_Acad.Spec.SlabOpenings
{
    /// <summary>
    /// строка таблицы отверстий в плите
    /// </summary>
    public class SlabRow : ISpecRow
    {
        public List<ISpecElement> Elements { get; set; }
        public string Group { get; set; }
        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }        

        public SlabRow (string group, List<ISpecElement> items)
        {
            Group = group;
            Elements = items;
            var slabElems = items.Cast<ISlabElement>();       
            var first = slabElems.First();
            Mark = first.Mark;
            Dimension = first.Dimension;
            Role = first.Role;
            Count = slabElems.Sum(s => s.Count);
            Description = first.Description;            
        }        
    }
}
