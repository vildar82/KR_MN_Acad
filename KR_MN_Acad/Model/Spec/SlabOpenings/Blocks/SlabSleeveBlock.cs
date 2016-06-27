using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Openings.Blocks;
using KR_MN_Acad.Spec.SlabOpenings.Elements;

namespace KR_MN_Acad.Spec.SlabOpenings.Blocks
{
    /// <summary>
    /// Блок гильзы в плите
    /// </summary>
    public class SlabSleeveBlock : SpecBlock
    {
        public const string BlockName = "КР_Гильза в плите";
        private const string propMark = "МАРКА";
        private const string propDiam = "Диаметр";
        private const string propDepth = "ТОЛЩИНАГИЛЬЗЫ";        
        private const string propDesc = "ПОЯСНЕНИЕ";        

        SlabSleeve sleeve;        

        public SlabSleeveBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            int diam = Block.GetPropValue<int>(propDiam);
            int depth = Block.GetPropValue<int>(propDepth);
            string role = SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            sleeve = new SlabSleeve (mark, diam, depth, role, desc, this);
            Elements.Add(sleeve);
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, sleeve.Mark);
        }
    }
}
