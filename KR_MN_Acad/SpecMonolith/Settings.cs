namespace KR_MN_Acad.SpecMonolith
{
   public class Settings
   {
      private static Settings instance;

      private Settings()
      {
      }

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

      public string AttrDescription { get; private set; }
      public string AttrGroup { get; private set; }
      public string AttrIndication { get; private set; }
      public string AttrMark { get; private set; }
      public string AttrName { get; private set; }
      public string AttrType { get; private set; }
      public string AttrWeight { get; private set; }

      // Значение атрибута ТИПА для блоков монолитных конструкций
      public string BlockMonolithTypeName { get; private set; }

      public string BlockPrefix { get; private set; }

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