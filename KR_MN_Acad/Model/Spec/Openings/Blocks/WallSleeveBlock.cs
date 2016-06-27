using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Openings.Elements;

namespace KR_MN_Acad.Spec.Openings.Blocks
{
    /// <summary>
    /// Блок гильзы в плите
    /// </summary>
    public class WallSleeveBlock : SpecBlock
    {
        public const string BlockName = "КР_Гильза";
        private const string propMark = "МАРКА";
        private const string propDiam = "Диаметр";
        private const string propDepth = "ТОЛЩИНАГИЛЬЗЫ";
        private const string propElevation = "ОТМЕТКА";
        private const string propDesc = "ПОЯСНЕНИЕ";        

        WallSleeve sleeve;       

        public WallSleeveBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            int diam = Block.GetPropValue<int>(propDiam);
            int depth = Block.GetPropValue<int>(propDepth);
            double elev = Block.GetPropValue<double>(propElevation);
            string role = SlabOpenings.SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            sleeve = new WallSleeve (mark, diam, depth, elev, role, desc, this);
            Elements.Add(sleeve);
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, sleeve.Mark);
        }
    }
}
