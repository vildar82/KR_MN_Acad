using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.WallOpenings.Blocks;
using KR_MN_Acad.Spec.SlabOpenings.Elements;
using KR_MN_Acad.Spec.Elements;

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
        private const string propLength = "Толщина_плиты";
        private const string propDesc = "ПОЯСНЕНИЕ";
        private const string propWeightUnit = "МАССА_МП";

        SlabSleeve sleeve;
        Tube tube;

        public SlabSleeveBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {            
        }

        public override void Calculate ()
        {
            string mark = Block.GetPropValue<string>(propMark);
            double diam = Block.GetPropValue<double>(propDiam);
            double depth = Block.GetPropValue<double>(propDepth);
            int length = Block.GetPropValue<int>(propLength);
            string role = SlabService.GetRole(Block);            
            string desc = Block.GetPropValue<string>(propDesc, false);
            sleeve = new SlabSleeve (mark, diam, depth, length, role, desc, this);
            AddElement(sleeve);

            //double weightUnit = Block.GetPropValue<double>(propWeightUnit, isRequired:false);            
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
