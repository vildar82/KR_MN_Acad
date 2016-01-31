using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.Extensions;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.SpecTemplate.Options;

namespace KR_MN_Acad.SpecService
{
   /// <summary>
   /// Элемент спецификации
   /// </summary>
   public class SpecItem
   {   
      public ObjectId IdBlRef { get; private set; }
      public string BlName { get; private set; }
      public Dictionary<string, DBText> AttrsDict { get; private set; }
      // Название группы для элемента
      public string Group { get; private set; } = "";
      /// <summary>
      /// Ключевое свойство элемента - обычно это Марка элемента.
      /// </summary>
      public string Key { get; private set; } 

      public SpecItem(ObjectId idBlRef)
      {
         IdBlRef = idBlRef;
      }

      /// <summary>
      /// Фильтр блоков. И составление списка всех элементов (1 блок - 1 элемент).
      /// </summary>      
      public static List<SpecItem> FilterSpecItems(SpecTable specTable)
      {
         List<SpecItem> items = new List<SpecItem>();
         // Обработка блоков и отбор блоков монолитных конструкций       
         foreach (var idBlRef in specTable.SelBlocks.IdsBlRefSelected)
         {
            SpecItem specItem = new SpecItem(idBlRef);
            if (specItem.Define(specTable))
            {
               items.Add(specItem);
            }
         }

         if (items.Count == 0)
         {
            throw new Exception ("Не определены блоки монолитных конструкций.");
         }
         else
         {
            specTable.Doc.Editor.WriteMessage($"\nОпределено блоков монолитных конструкций: {items.Count}\n");
         }
         return items;
      }

      public bool Define(SpecTable specTable)
      {
         if (IdBlRef.IsNull)
         {
            return false;
         }

         bool resVal = false;
         var blRef = IdBlRef.GetObject(OpenMode.ForRead, false, true) as BlockReference;
         if (blRef != null && blRef.AttributeCollection != null)
         {
            BlName = blRef.GetEffectiveName();
            if (Regex.IsMatch(BlName, specTable.SpecOptions.BlocksFilter.BlockNameMatch, RegexOptions.IgnoreCase))
            {
               // Проверка обязательных атрибутов
               AttrsDict = blRef.GetAttributeDictionary();               
               resVal = true;
               foreach (var atrMustHave in specTable.SpecOptions.BlocksFilter.AttrsMustHave)
               {
                  if (!AttrsDict.ContainsKey(atrMustHave))
                  {
                     resVal = false;
                     string atrsMustHave = string.Join(", ", specTable.SpecOptions.BlocksFilter.AttrsMustHave);
                     Inspector.AddError($"Блок {BlName} пропущен, т.к. в нем нет обязательных атрибутов: {atrsMustHave}");
                  }
               }
               
               // определение Группы
               DBText groupAttr;
               if (AttrsDict.TryGetValue(specTable.SpecOptions.GroupPropName, out groupAttr))
               {
                  Group = groupAttr.TextString;
               }

               // Ключевое свойство
               DBText keyAttr;
               if (AttrsDict.TryGetValue(specTable.SpecOptions.KeyPropName, out keyAttr))
               {
                  Key = keyAttr.TextString;
               }
               else
               {
                  Inspector.AddError($"Блок {BlName} пропущен, т.к. в нем нет ключевого атрибута: {specTable.SpecOptions.KeyPropName}");
                  resVal = false;
               }
            }
         }
         return resVal;
      }

      /// <summary>
      /// Проверка соответствия значениям в столбцах
      /// </summary>
      /// <param name="columnsValue"></param>
      public void CheckColumnsValur(List<ColumnValue> columnsValue, SpecTable specTable)
      {
         string err = string.Empty;
         foreach (var colVal in columnsValue)
         {
            if (colVal.ColumnSpec.ItemPropName == "Count")
            {
               continue;
            }

            DBText atr;
            if (AttrsDict.TryGetValue(colVal.ColumnSpec.ItemPropName, out atr))
            {
               if (!colVal.Value.Equals(atr.TextString, StringComparison.OrdinalIgnoreCase))
               {
                  err += $"'{colVal.ColumnSpec.ItemPropName}'='{atr.TextString}' не соответствует эталонному значению '{colVal.Value}', '{specTable.SpecOptions.KeyPropName}' = '{Key}'.\n";
               }
            }
            else
            {
               // В элементе вообще нет свойства для этого столбца
               err += $"Нет определено свойтво '{colVal.ColumnSpec.ItemPropName}'.\n";
            }
         }
         if (!string.IsNullOrEmpty(err))
         {
            Inspector.AddError($"Ошибки в блоке {BlName}: {err} Этот блок попадет в спецификацию с эталонными значениями.", IdBlRef);
         }
      }
   }
}