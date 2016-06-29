﻿using System;
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
            // Спецификация
            options.Table = new TableOptions("Спецификация материалов на вертикальные конструкции");
            options.Table.Layer = "КР_Таблицы";

            // Типы блоков для схемы
            options.TypesBlock = new Dictionary<string, Type>
            {
                { WallBlock.BlockName , typeof(WallBlock) },
                { WallEndBlock.BlockName , typeof(WallEndBlock) },
				{ WallEndCornerBlock.BlockName , typeof(WallEndCornerBlock) },
                { WallEndTBlock.BlockName , typeof(WallEndTBlock) },
                { ColumnSquareSmallBlock.BlockName , typeof(ColumnSquareSmallBlock) },
                { ColumnSquareBigBlock.BlockName , typeof(ColumnSquareBigBlock) },
                { AddHorArmBlock.BlockName , typeof(AddHorArmBlock) },
                { AddVerticLShapedArmBlock.BlockName , typeof(AddVerticLShapedArmBlock) },
                { PylonBlock.BlockName , typeof(PylonBlock) },                
                { SleeveBlock.BlockName , typeof(SleeveBlock) }
            };

            // Правила блоков
            options.Rule = new WallRule();

            return options;
        }
    }
}
