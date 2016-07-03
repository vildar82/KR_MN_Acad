using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements.Bars;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Дополнительный вертикальный гнутый стержень (усиление из стены в плиту)
    /// </summary>
    public class AddVerticLShapedArmBlock : SpecBlock
    {
        public const string BlockName = "КР_Арм_Стен_ДопВертикГс";
        
        const string PropNameLength = "Длина";
        const string PropNameBentLength = "Длина загиба";
        const string PropNameBentHeight = "Высота загиба";
        const string PropNameDiam = "Диаметр";
        const string PropNameStep = "Шаг";
        const string PropNamePos = "ПОЗГС";
        const string PropNameDesc = "ОПИСАНИЕГС";       
        
        /// <summary>
        /// Гнутый стержень
        /// </summary>
        public BentBarLshaped BentBar { get; set; }

        public AddVerticLShapedArmBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        public override void Calculate ()
        {
            try
            {
                var len = Block.GetPropValue<int>(PropNameLength);
                var bentL = Block.GetPropValue<int>(PropNameBentLength);
                var bentH = Block.GetPropValue<int>(PropNameBentHeight);                
                BentBar = defineBent(PropNameDiam, bentL, bentH, len, PropNameStep, PropNamePos);
                AddElement(BentBar);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }            
        }

        public override void Numbering ()
        {
            // Гс
            FillElemPropNameDesc(BentBar, PropNamePos, PropNameDesc);
        }
    }
}
