using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace KR_MN_Acad.SpecMonolith
{
   public class MonolithGroup
   {
      public string Name { get; private set; }
      public List<MonolithSpecRecord> Records { get; private set; }

      public MonolithGroup(string name)
      {
         Name = name;
         Records = new List<MonolithSpecRecord>();
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

      /// <summary>
      /// Проверка группы - уникальностиь марок и наименований
      /// </summary>
      public void Check()
      {
         // Повторяющаяся марка - марка одна, а наименования разные
         var repeatMarks = Records.GroupBy(r => r.Mark).Where(g => g.Count() > 1);
         if (repeatMarks.Count()>0)
         {            
            foreach (var repeatMark in repeatMarks)
            {
               MonolithSpecRecord firstRec = new MonolithSpecRecord();
               bool isFirst = true;
               foreach (var repeatItem in repeatMark)
               {
                  if (isFirst)
                  {
                     firstRec = repeatItem;
                     isFirst = false;
                     continue;
                  }
                  foreach (var monolithItem in repeatItem.Items)
                  {
                     Inspector.AddError("Повтор марки {0} с наименованием {1}. Уже была определена марка {2} с наименованием {3}"
                        .f(repeatItem.Mark, repeatItem.Name, firstRec.Mark, firstRec.Name),
                        monolithItem.Extents, monolithItem.IdBlRef);
                  }                  
               }               
            }            
         }

         // Повторяющаяся наименования - марка одна, а наименования разные
         var repeatNames = Records.GroupBy(r => r.Name).Where(g => g.Count() > 1);
         if (repeatNames.Count() > 0)
         {
            foreach (var repeatName in repeatNames)
            {
               MonolithSpecRecord firstRec = new MonolithSpecRecord();
               bool isFirst = true;
               foreach (var repeatItem in repeatName)
               {
                  if (isFirst)
                  {
                     firstRec = repeatItem;
                     isFirst = false;
                     continue;
                  }
                  foreach (var monolithItem in repeatItem.Items)
                  {
                     Inspector.AddError("Повтор наименования {0} для марки {1}. Уже было определено наименование {2} для марки {3}"
                        .f(repeatItem.Name, repeatItem.Mark, firstRec.Name , firstRec.Mark ),
                        monolithItem.Extents, monolithItem.IdBlRef);
                  }
               }
            }
         }
      }
   }
}
