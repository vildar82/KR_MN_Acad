using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AcadLib;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme.Wall
{
    public static class WallSchemeOptions
    {
        public static SchemeOptions GetSchemeOptions()
        {
            SchemeOptions options = new SchemeOptions();
            // Фильтр блоков
            options.Filter = new FilterBlocks(WallBlock.WallBlockName);
            // Спецификация
            options.Table = new TableOptions("Спецификация материалов на вертикальные конструкции");

            // Типы блоков для схемы
            options.TypesBlock = new Dictionary<string, Type>
            {
                { WallBlock.WallBlockName , typeof(WallBlock) },
                { WallJoinBlock.WallJoinBlockName , typeof(WallJoinBlock) }
            };
            return options;
        }
    }
}
