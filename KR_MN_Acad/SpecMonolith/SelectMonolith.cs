using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.SpecMonolith
{
   // Выбор блоков монолитных конструкцийй
   public class SelectMonolith
   {
      public List<ObjectId> IdsBlRefSelected { get; private set; }

      public void Select()
      {
         // запрос выбора пользователю 
         Document doc = Application.DocumentManager.MdiActiveDocument;
         Editor ed = doc.Editor;
         IdsBlRefSelected = ed.SelectBlRefs("Выбор блоков для расчета спецификации монолитных конструкций.");
         ed.WriteMessage("\nВыбрано {0} блоков", IdsBlRefSelected.Count);
      }
   }
}
