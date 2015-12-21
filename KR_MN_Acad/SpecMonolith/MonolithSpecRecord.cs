using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.SpecMonolith
{
   public struct MonolithSpecRecord
   {
      public string Mark { get; set; }
      public string Indication { get; set; }
      public string Name { get; set; }
      public int Count { get; set; }
      public double Weight { get; set; }
      public string Description { get; set; }
      public List<MonolithItem> Items { get; set; }   
      
      public MonolithSpecRecord (string mark, string name, List<MonolithItem> items)
      {
         Mark = mark;
         Name = name;
         Items = items ?? new List<MonolithItem>();
         Count = Items.Count;
         Indication = items.FirstOrDefault(r => !string.IsNullOrEmpty(r.Indication))?.Indication ?? string.Empty;
         Weight= items.FirstOrDefault(r => r.Weight!=0)?.Weight ?? 0;
         Description= items.FirstOrDefault(r => !string.IsNullOrEmpty(r.Description))?.Indication ?? string.Empty;         
      }      
   }
}
