using System;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.Extensions;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.SpecMonolith
{
   /// <summary>
   /// Монолитный элемент
   /// </summary>
   public class MonolithItem
   {
      public MonolithItem(ObjectId idBlRef)
      {
         IdBlRef = idBlRef;
      }

      public AttributeRefInfo AttrDescription { get; private set; }
      public AttributeRefInfo AttrGroup { get; private set; }
      public AttributeRefInfo AttrIndication { get; private set; }
      public AttributeRefInfo AttrMark { get; private set; }
      public AttributeRefInfo AttrName { get; private set; }
      public AttributeRefInfo AttrType { get; private set; }
      public AttributeRefInfo AttrWeight { get; private set; }
      public string Description { get; private set; } = string.Empty;
      public string Group { get; private set; } = string.Empty;
      public ObjectId IdBlRef { get; private set; }

      // Обозначение
      public string Indication { get; private set; } = string.Empty;

      public string Mark { get; private set; }

      // Наименование
      public string Name { get; private set; }

      //Масса ед.кг.
      public double Weight { get; private set; }

      public bool Define()
      {         
         bool resVal = false;
         using (var blRef = IdBlRef.GetObject(OpenMode.ForRead, false, true) as BlockReference)
         {
            if (blRef.AttributeCollection != null)
            {
               var blName = blRef.GetEffectiveName();
               if (blName.StartsWith(Settings.Instance.BlockPrefix))
               {
                  //определение атрибутов
                  string errMsg = defineAttrs(blRef.AttributeCollection);
                  resVal = string.IsNullOrEmpty(errMsg);
                  if (!resVal)
                  {
                     Inspector.AddError("Не определенный как блок монолитной конструкции - {0}: {1}".f(blName, errMsg), blRef);
                  }
               }
            }
         }
         return resVal;
      }

      private string checkAttrs()
      {
         string errMsg = string.Empty;
         // ТИП
         if (AttrType == null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrType);            
         }
         else if (!string.Equals(AttrType.Text, Settings.Instance.BlockMonolithTypeName, StringComparison.CurrentCultureIgnoreCase))
         {
            errMsg += "Значение атрибута {0} не равно {1}. ".f(Settings.Instance.AttrType, Settings.Instance.BlockMonolithTypeName);            
         }
         // НАИМЕНОВАНИЕ
         if (AttrName == null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrName);            
         }
         else if (string.IsNullOrEmpty(AttrName.Text))
         {
            errMsg += "Пустое значение атрибута {0}. ".f(Settings.Instance.AttrName);            
         }
         // Марка
         if (AttrMark == null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrMark);            
         }
         else if (string.IsNullOrEmpty(AttrMark.Text))
         {
            errMsg += "Пустое значение атрибута {0}. ".f(Settings.Instance.AttrMark);            
         }
         return errMsg;
      }

      private string defineAttrs(AttributeCollection attrs)
      {        
         foreach (ObjectId idAtrRef in attrs)
         {
            var atrRef = idAtrRef.GetObject(OpenMode.ForRead, false, true) as AttributeReference;
            // ТИП
            if (atrRef.Is(Settings.Instance.AttrType))
            {
               AttrType = new AttributeRefInfo(atrRef);
            }
            // ГРУППА
            else if (atrRef.Is(Settings.Instance.AttrGroup))
            {
               AttrGroup = new AttributeRefInfo(atrRef);
               Group = AttrGroup.Text;
            }
            // ОБОЗНАЧЕНИЕ
            else if (atrRef.Is(Settings.Instance.AttrIndication))
            {
               AttrIndication = new AttributeRefInfo(atrRef);
               Indication = AttrIndication.Text;
            }
            // МАРКА
            else if (atrRef.Is(Settings.Instance.AttrMark))
            {
               AttrMark = new AttributeRefInfo(atrRef);
               Mark = AttrMark.Text;
            }
            // НАИМЕНОВАНИЕ
            else if (atrRef.Is(Settings.Instance.AttrName))
            {
               AttrName = new AttributeRefInfo(atrRef);
               Name = AttrName.Text;
            }
            // МАССА
            else if (atrRef.Is(Settings.Instance.AttrWeight))
            {
               AttrWeight = new AttributeRefInfo(atrRef);
               double weight;
               if (double.TryParse(AttrWeight.Text, out weight))
               {
                  Weight = weight;
               }
            }
            // ПРИМЕЧАНИЕ
            else if (atrRef.Is(Settings.Instance.AttrDescription))
            {
               AttrDescription = new AttributeRefInfo(atrRef);
               Description = AttrDescription.Text;
            }
         }
         return checkAttrs();
      }
   }
}