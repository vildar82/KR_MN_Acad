using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using KR_MN_Acad.SpecMonolith;

[assembly: CommandClass(typeof(KR_MN_Acad.Commands))]

namespace KR_MN_Acad
{
   public class Commands
   {
      public static AutoCAD_PIK_Manager.LogAddin Log { get; private set; } = new AutoCAD_PIK_Manager.LogAddin("Plugin KR_MN_Acad");

      /// <summary>
      /// Спецификация монолитных блоков
      /// </summary>
      [CommandMethod("PIK", "KR-SpecMonolith", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
      public void SpecMonolithCommand ()
      {
         Log.Info("Start Command: KR-SpecMonolith");
         Document doc = Application.DocumentManager.MdiActiveDocument;
         if (doc == null) return;
         Database db = doc.Database;
         Editor ed = doc.Editor;
         using (var DocLock = doc.LockDocument())
         {
            try
            {
               Inspector.Clear();
               // Спецификация монолитных блоков
               SpecMonolithService specMonolith = new SpecMonolithService();
               specMonolith.Spec();

               if (Inspector.HasErrors)
               {
                  Inspector.Show();
               }
            }
            catch (System.Exception ex)
            {
               if (!ex.Message.Contains("Отменено пользователем"))
               {
                  Log.Error(ex, "Command: KR-SpecMonolith. Doc {0}", doc.Name);
               }
               ed.WriteMessage("Ошибка - {0}", ex.Message);
            }
         }         
      }     
   }
}
