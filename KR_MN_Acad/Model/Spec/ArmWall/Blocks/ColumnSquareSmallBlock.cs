using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Описание блока колонны квадратной до 400мм
    /// </summary>
    public class ColumnSquareSmallBlock : ColumnBase
    {        
        public const string BlockName = "КР_Арм_Колонна_Квадратная_до400";

        const string PropNameSide = "Ширина колонны";

        /// <summary>
        /// Сторона колонны (ширина)
        /// </summary>
        public int Side { get; set; }

        public ColumnSquareSmallBlock (BlockReference blRef, string blName) : base (blRef, blName)
        {            
        }

        protected override void AddElements ()
        {
            base.AddElements();
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.            
            Side = Block.GetPropValue<int>(PropNameSide);
            DefineBaseFields(Side, Side, true);
            AddElements();
        }
    }
}