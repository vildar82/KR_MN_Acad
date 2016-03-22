using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace KR_MN_Acad.Model.Pile
{ 
    public class Pile
    {
        public string BlName { get; set; }
        public ObjectId IdBlRef { get; set; }
        public Point3d Pt { get; set; }        
        public List<AttributeInfo> AttrRefs { get; set; }
        public AttributeInfo PosAttrRef { get; set; }
        public PileOptions Options { get; set; }

        public Pile(BlockReference blRef, string blName, PileOptions pileOptions)
        {
            BlName = blName;
            IdBlRef = blRef.Id;
            Pt = blRef.Position;            
            // Выбор атрибутов
            AttrRefs = AttributeInfo.GetAttrRefs(blRef);
            Options = pileOptions;
            PosAttrRef = AttrRefs.FirstOrDefault(a => a.Tag.Equals(Options.PileAttrPos, StringComparison.OrdinalIgnoreCase));            
        }

        public AcadLib.Result Check()
        {            
            if (PosAttrRef == null)
            {
                return AcadLib.Result.Fail($"Не найден атрибут '{Options.PileAttrPos}'");
            }
            return AcadLib.Result.Ok();
        }                
    }
}