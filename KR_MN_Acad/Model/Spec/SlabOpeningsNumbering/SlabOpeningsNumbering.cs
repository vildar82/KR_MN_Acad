using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
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

            using (var t = db.TransactionManager.StartTransaction())
            {
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecSlabOpenings());
                var groups = specService.SelectAndGroupBlocks();

                int index = 1;
                foreach (var group in groups)
                {                    
                    foreach (var item in group)
                    {
                        if (item.AtrKey != null)
                        {
                            item.AtrKey.UpgradeOpen();
                            item.AtrKey.TextString = index.ToString();
                            item.Key = index.ToString();
                            Inspector.AddError($"{item.BlName} {SpecBlocks.SpecService.Optinons.KeyPropName}={item.Key}", item.IdBlRef,
                                    icon: System.Drawing.SystemIcons.Information);
                        }                        
                    }
                    index++;
                }                
                t.Commit();
            }
        }             
    }
}
