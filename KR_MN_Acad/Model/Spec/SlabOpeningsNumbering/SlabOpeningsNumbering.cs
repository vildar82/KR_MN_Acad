using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Spec
{
    public static class SlabOpeningsNumbering
    {
        public static void Numbering()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            var sel = ed.SelectBlRefs("Выбор блоков");

            using (var t = db.TransactionManager.StartTransaction())
            {
                var slabOpBlocks = SlabOpeningsBlock.GetBlocks(sel);

                // Группировка по назначению потом по размеру
                var sortSlOpBlocks = slabOpBlocks.OrderBy(b => b.Destination).
                    ThenByDescending(b => b.Size, AcadLib.Comparers.AlphanumComparator.New);

                int index = 1;
                foreach (var item in sortSlOpBlocks)
                {
                    item.AtrMark.UpgradeOpen();
                    item.AtrMark.TextString = index++.ToString();                    
                }
                t.Commit();
            }
        }             
    }
}
