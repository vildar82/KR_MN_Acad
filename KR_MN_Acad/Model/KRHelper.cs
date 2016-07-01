using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad
{
    public static class KRHelper
    {
        public static string FileKRBlocks { get; } = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Blocks\КР-МН\КР_Блоки.dwg");
        public const string LayerTable = "КР_Таблицы";
    }
}
