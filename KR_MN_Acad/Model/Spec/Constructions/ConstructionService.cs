using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Spec.Constructions
{
    public class ConstructionService : SpecService
    {
        public ConstructionService (Document doc) : base(doc, new ConstructionOptions(doc.Database))
        {
        }

        protected override List<ObjectId> SelectBlocks ()
        {
            var selOpt = new PromptEntityOptions("\nВыбор блока конструкции:");
            selOpt.SetRejectMessage("\nМожно выбрать только блок.");
            selOpt.AddAllowedClass(typeof(BlockReference), true);
            var selRes = ed.GetEntity(selOpt);
            if (selRes.Status ==  PromptStatus.OK)
            {
                return new List<ObjectId> { selRes.ObjectId };
            }
            throw new Exception(AcadLib.General.CanceledByUser);
        }
    }
}
