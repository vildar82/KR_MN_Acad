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
            string mark = string.Empty;
            try
            {
                mark = Block.GetPropValue<string>(propMark);
            }
            catch(Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - string mark = Block.GetPropValue<string>(propMark);");
            }

            int diam = 0;
            try
            {
                diam = Block.GetPropValue<int>(propDiam);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - diam = Block.GetPropValue<int>(propDiam);");
            }

            int depth = 0;
            try
            {
                depth = Block.GetPropValue<int>(propDepth);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - depth = Block.GetPropValue<int>(propDepth);");
            }

            int length = 0;
            try
            {
                length = Block.GetPropValue<int>(propLength);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - length = Block.GetPropValue<int>(propLength);");
            }

            string elev = string.Empty;
            try
            {
                elev = Block.GetPropValue<string>(propElevation);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - elev = Block.GetPropValue<string>(propElevation);");
            }

            string role = string.Empty;
            try
            {
                role = SlabOpenings.SlabService.GetRole(Block);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - role = SlabOpenings.SlabService.GetRole(Block);");
            }

            string desc = string.Empty;
            try
            {
                desc = Block.GetPropValue<string>(propDesc, false);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - desc = Block.GetPropValue<string>(propDesc, false);");
            }

            sleeve = new WallSleeve (mark, diam, depth, length, elev, role, desc, this);
            AddElement(sleeve);

            string wuAtr = string.Empty;
            try
            {
                wuAtr = Block.GetPropValue<string>(propWeightUnit);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - wuAtr = Block.GetPropValue<string>(propWeightUnit);");
            }

            double wu = 0;
            try
            {                
                wu = wuAtr.ToDouble();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "WallSleeveBlock - wuAtr = Block.GetPropValue<string>(propWeightUnit);");
            }

            tube = new Tube(diam, depth, length, wu, this);
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
