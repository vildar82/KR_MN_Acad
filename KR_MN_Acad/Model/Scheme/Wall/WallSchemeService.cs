using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Схема расчета армирования стен
    /// </summary>
    public class WallSchemeService
    {
        public static WallSchemeOptions Options { get; set; }
        public static Document Doc { get; set; }
        public static Database Db { get; set; }
        public static Editor Ed { get; set; }
        /// <summary>
        /// Нумерация стен - определение позиций стержней и т.д.
        /// </summary>
        public void Numbering()
        {
            Doc = Application.DocumentManager.MdiActiveDocument;
            Ed = Doc.Editor;
            Db = Doc.Database;
            Options = WallSchemeOptions.Load();
            // Выбор блоков стен
            var ids = SelectService.Select();
            // Определение стен
            //var walls = WallBlock.GetWalls(ids);
        }
    }
}
