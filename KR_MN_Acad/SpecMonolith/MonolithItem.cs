using System;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using AcadLib.Extensions;

namespace KR_MN_Acad.SpecMonolith
{
   /// <summary>
   /// Монолитный элемент
   /// </summary>
   public class MonolithItem
   {      
      public ObjectId IdBlRef { get; private set; }

      public string Group { get; private set; } = string.Empty;
      // Обозначение
      public string Indication { get; private set; } = string.Empty;

      public string Mark { get; private set; }

      // Наименование
      public string Name { get; private set; }
      //Масса ед.кг.
      public double Weight { get; private set; }
      public string Description { get; private set; } = string.Empty;

      public AttributeRefInfo AttrDescription { get; private set; }
      public AttributeRefInfo AttrWeight { get; private set; }
      public AttributeRefInfo AttrName { get; private set; }
      public AttributeRefInfo AttrMark { get; private set; }
      public AttributeRefInfo AttrIndication { get; private set; }
      public AttributeRefInfo AttrGroup { get; private set; }
      public AttributeRefInfo AttrType { get; private set; }

      public MonolithItem(ObjectId idBlRef)
      {
         IdBlRef = idBlRef;
      }

      public bool Define(out string errMsg)
      {
         errMsg = string.Empty;
         bool resVal = false;
         using (var blRef = IdBlRef.GetObject( OpenMode.ForRead, false, true)as BlockReference)
         {
            if (blRef.AttributeCollection!=null)
            {
               var blName = blRef.GetEffectiveName();
               if (blName.StartsWith(Settings.Instance.BlockPrefix))
               {
                  //определение атрибутов
                  resVal =defineAttrs(blRef.AttributeCollection, ref errMsg);
               }
            }
         }
         return resVal;
      }

      private bool defineAttrs(AttributeCollection attrs, ref string errMsg)
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
               if(double.TryParse(AttrWeight.Text, out weight))
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
         return checkAttrs(ref errMsg);
      }      

      private bool checkAttrs(ref string errMsg)
      {
         bool res = true;
         // ТИП
         if (AttrType==null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrType);
            res = false;            
         }
         else if (!string.Equals(AttrType.Text, Settings.Instance.BlockMonolithTypeName, StringComparison.CurrentCultureIgnoreCase))
         {
            errMsg += "Значение атрибута {0} не равно {1}. ".f(Settings.Instance.AttrType, Settings.Instance.BlockMonolithTypeName);
            res = false;
         }
         // НАИМЕНОВАНИЕ
         if (AttrName== null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrName);
            res = false;
         }
         else if(string.IsNullOrEmpty(AttrName.Text))
         {
            errMsg += "Пустое значение атрибута {0}. ".f(Settings.Instance.AttrName);
            res = false;
         }
         // Марка
         if (AttrMark == null)
         {
            errMsg += "Не определен атрибут {0}. ".f(Settings.Instance.AttrMark);
            res = false;
         }
         else if (string.IsNullOrEmpty(AttrMark.Text))
         {
            errMsg += "Пустое значение атрибута {0}. ".f(Settings.Instance.AttrMark);
            res = false;
         }

         return res;
      }
   }
}