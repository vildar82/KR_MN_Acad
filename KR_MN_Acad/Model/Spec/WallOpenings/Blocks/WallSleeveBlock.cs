using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.WallOpenings.Elements;
using KR_MN_Acad.Spec.Elements;
using AcadLib;

namespace KR_MN_Acad.Spec.WallOpenings.Blocks
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
        private const string propLength = "Толщина стены";
        private const string propElevation = "ОТМЕТКА";
        private const string propDesc = "ПОЯСНЕНИЕ";
        private const string propWeightUnit = "МАССА_МП";

        /// <summary>
        /// гильза
        /// </summary>
        WallSleeve sleeve;
        /// <summary>
        /// Труба (для спецификации)
        /// </summary>
        Tube tube;

        public WallSleeveBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark =Block.GetPropValue<string>(propMark);            
            double diam = Block.GetPropValue<double>(propDiam);            
            double depth = Block.GetPropValue<double>(propDepth);  
            int length = Block.GetPropValue<int>(propLength);
            string elev = Block.GetPropValue<string>(propElevation);            
            string role = SlabOpenings.SlabService.GetRole(Block);
            string desc = Block.GetPropValue<string>(propDesc, false);

            sleeve = new WallSleeve (mark, diam, depth, length, elev, role, desc, this);
            AddElement(sleeve);

            //double wu = Block.GetPropValue<double>(propWeightUnit, isRequired:false);
            tube = new Tube(diam, depth, length, this);
            tube.Mark = mark;
            tube.Calc();
            AddElement(tube);            
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, sleeve.Mark);
        }
    }
}
