using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Выбор объектов на чертеже для расчета схем армирования и материалов
    /// </summary>
    public static class SelectService
    {
        /// <summary>
        /// Выбор
        /// </summary>
        public static List<ObjectId> Select()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            return ed.SelectBlRefs("\nВыбор объектов схемы:");
        }
    }
}
