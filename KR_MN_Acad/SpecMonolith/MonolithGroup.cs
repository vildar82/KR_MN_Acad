using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.SpecMonolith
{
   public class MonolithGroup
   {
      public string Name { get; private set; }
      public List<MonolithSpecRecord> Records { get; private set; }

      public MonolithGroup(string name)
      {
         Name = name;
      }

      public void Calc(IGrouping<string, MonolithItem> itemGroup)
      {
         var uniqRecs = itemGroup.GroupBy(m => new { m.Mark, m.Name}).OrderBy(m=>m.Key.Mark);
         foreach (var urec in uniqRecs)
         {
            MonolithSpecRecord rec = new MonolithSpecRecord(urec.Key.Mark, urec.Key.Name, urec.ToList());
            Records.Add(rec);
         }
      }      
   }
}
