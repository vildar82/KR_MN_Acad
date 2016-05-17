using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using SpecBlocks;

namespace KR_MN_Acad.Spec
{
    class SlabOpeningsBlock
    {
        public string BlName { get; set; }
        public string Size { get; set; }
        public string Destination { get; set; }
        public AttributeReference AtrMark { get; set; }

        public SlabOpeningsBlock()
        {
            
        }        

        public Result Define(BlockReference blRef, string blName)
        {
            BlName = blName;
            string errMsg = string.Empty;
            var atrs = AcadLib.Extensions.AttributeExt.GetAttributeDictionary(blRef);
            // параметры группировки
            // 1 - атр Размер            
            DBText atrSize = getAtr("РАЗМЕР", atrs, ref errMsg);
            Size = atrSize?.TextString;

            // назначение            
            DBText atrDest = getAtr("НАЗНАЧЕНИЕ", atrs, ref errMsg);
            Destination = atrDest?.TextString;

            // Марка
            AtrMark = getAtr("МАРКА", atrs, ref errMsg) as AttributeReference;            

            if(string.IsNullOrEmpty(errMsg))
            {
                return Result.Ok();
            }
            else
            {
                string err = $"Ошибки в блоке {BlName}: " + errMsg;                
                return Result.Fail(err);
            }
        }

        private DBText getAtr(string tag, Dictionary<string, DBText> atrs, ref string errMsg)
        {
            DBText atr;
            if (!atrs.TryGetValue(tag, out atr))
            {
                // Нет атрибута размера
                errMsg += $"Нет атрибута {tag}. ";
            }
            return atr;
        }

        internal static List<SlabOpeningsBlock> GetBlocks(List<ObjectId> sel)
        {
            List<SlabOpeningsBlock> slabOpBlocks = new List<SlabOpeningsBlock>();            
            foreach (var item in sel)
            {
                var blRef = item.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                if (blRef == null) continue;
                string blName = blRef.GetEffectiveName();
                if (blName.Equals("КР_Отв в плите", StringComparison.OrdinalIgnoreCase))
                {
                    SlabOpeningsBlock slOpBl = new SlabOpeningsBlock();
                    var resDef = slOpBl.Define(blRef, blName);
                    if(resDef.Failure)
                    {
                        Inspector.AddError(resDef.Error, blRef, System.Drawing.SystemIcons.Warning);
                    }                    
                    slabOpBlocks.Add(slOpBl);
                }
            }
            return slabOpBlocks;
        }
    }
}
