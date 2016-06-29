using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
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

        public ColumnSquareSmallBlock (BlockReference blRef, string blName, SchemeService service) : base (blRef, blName, service)
        {            
        }

        public override void Calculate ()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                Side = Convert.ToInt32(GetPropValue<double>(PropNameSide));
                DefineBaseFields(Side, Side, true);                
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }                
    }
}