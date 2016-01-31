using KR_MN_Acad.Spec.SpecTemplate.Options;

namespace KR_MN_Acad.SpecService
{
   /// <summary>
   /// Значение столбца
   /// </summary>
   public class ColumnValue
   {
      public TableColumn ColumnSpec { get; private set; }      

      public string Value { get; set; }

      public ColumnValue(TableColumn column)
      {
         ColumnSpec = column;
      }
   }
}