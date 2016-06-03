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
            options.Filter = new FilterBlocks("КР_Арм_Схема_Стена");
            // Спецификация
            options.Table = new TableOptions("Спецификация материалов на вертикальные конструкции");

            return options;       
        }
    }
}
