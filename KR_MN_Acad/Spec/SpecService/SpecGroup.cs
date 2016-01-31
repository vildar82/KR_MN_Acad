using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace KR_MN_Acad.SpecService
{
   /// <summary>
   /// Группирование элементов в спецификации
   /// </summary>
   public class SpecGroup
   {
      public string Name { get; private set; }
      /// <summary>
      /// Уникальные строки элементов таблицы - по ключевому свойству
      /// </summary>
      public List<SpecRecord> Records { get; private set; } = new List<SpecRecord>();

      public SpecGroup(string name)
      {
         Name = name;         
      }

      public static List<SpecGroup> Grouping(SpecTable specTable)
      {
         List<SpecGroup> groups = new List<SpecGroup>();
         var itemsGroupBy = specTable.Items.GroupBy(i => i.Group).OrderBy(g => g.Key);
         foreach (var itemGroup in itemsGroupBy)
         {
            SpecGroup group = new SpecGroup(itemGroup.Key);
            group.Calc(itemGroup, specTable);
            // проверка уникальности элементов в группе
            group.Check(specTable);
            groups.Add(group);
         }
         return groups;
      }

      public void Calc(IGrouping<string, SpecItem> itemGroup, SpecTable specTable)
      {
         // itemGroup - элементы одной группы.
         // Нужно сгруппировать по ключевому свойству
         var uniqRecs = itemGroup.GroupBy(m => m.Key).OrderBy(m=>m.Key);
         foreach (var urec in uniqRecs)
         {
            SpecRecord rec = new SpecRecord(urec.Key, urec.ToList(), specTable);
            Records.Add(rec);
         }
      }

      /// <summary>
      /// Проверка группы
      /// </summary>
      public void Check(SpecTable specTable)
      {
         Records.ForEach(r => r.CheckRecords(specTable));
      }
   }
}
