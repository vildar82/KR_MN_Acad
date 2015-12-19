using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.SpecMonolith
{
   public class Settings
   {
      private static Settings instance;

      public string BlockPrefix { get; private set; }
      // Значение атрибута ТИПА для блоков монолитных конструкций
      public string BlockMonolithTypeName { get; private set; }
      public string AttrType { get; private set; }
      public string AttrGroup { get; private set; }
      public string AttrMark { get; private set; }
      public string AttrIndication { get; private set; }
      public string AttrName { get; private set; }
      public string AttrWeight { get; private set; }
      public string AttrDescription { get; private set; }

      private Settings() { }

      public static Settings Instance
      {
         get
         {
            if (instance == null)
            {
               instance = Load();
            }
            return instance;
         }
      }

      private static Settings Load()
      {
         Settings resVal = new Settings();

         // загрузка из файла

         resVal.SetDefault();

         return resVal;
      }

      private void SetDefault()
      {
         BlockPrefix = "КР_";
         AttrType = "ТИП";
         AttrGroup = "ГРУППА";
         AttrMark = "МАРКА";
         AttrIndication = "ОБОЗНАЧЕНИЕ";
         AttrName = "НАИМЕНОВАНИЕ";
         AttrWeight = "МАССА";
         AttrDescription = "ПРИМЕЧАНИЕ";
         BlockMonolithTypeName = "Монолит";
      }
   }
}
