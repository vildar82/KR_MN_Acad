using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;

namespace KR_MN_Acad.SpecMonolith
{
   /// <summary>
   /// Спецификация монолитных блоков
   /// </summary>
   public class SpecMonolithService
   {
      public Document Doc { get; private set; }
      public List<MonolithItem> MonolithItems { get; private set; }

      public SpecMonolithService()
      {
         Doc = Application.DocumentManager.MdiActiveDocument;
         MonolithItems = new List<MonolithItem>();
      }

      public void Spec()
      {
         // Выбор блоков
         SelectMonolith select = new SelectMonolith();
         select.Select();         

         using (var t = Doc.TransactionManager.StartTransaction())
         {            
            // Обработка блоков и отбор блоков монолитных конструкций       
            foreach (var idBlRef in select.IdsBlRefSelected)
            {
               MonolithItem monolitItem = new MonolithItem(idBlRef);
               if (monolitItem.Define())
               {
                  MonolithItems.Add(monolitItem);
               }               
            }

            if (MonolithItems.Count==0)
            {
               Doc.Editor.WriteMessage("\nНе определены блоки монолитных конструкций.");
               return;
            }
            Doc.Editor.WriteMessage("\nОпределено блоков монолитных конструкций: {0}", MonolithItems.Count);

            // Обработка для спецификации
            MonolithSpec spec = new MonolithSpec(this);
            spec.Calc();
            //
            spec.CreateTable();
            t.Commit();
         }
      }
   }
}