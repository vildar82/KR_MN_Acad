using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using SpecBlocks;
using SpecBlocks.Options;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Спецификация монолитных блоков
    /// </summary>
    public class SpecAperture : ISpecCustom
    {
        private const string name = "КР_Спец_Проемы";

        public string File
        {
            get
            {
                return Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"КР-МН\Спецификации\" + name + ".xml");
            }
        }

        public SpecOptions GetDefaultOptions()
        {
            SpecOptions specApertureOpt = new SpecOptions();

            specApertureOpt.CheckDublicates = true;
            specApertureOpt.Name = name;

            // Фильтр для блоков
            specApertureOpt.BlocksFilter = new BlocksFilter();
            // Имя блока начинается с "КР_"
            specApertureOpt.BlocksFilter.BlockNameMatch = "^КР_Проем";
            // Обязательные атрибуты
            specApertureOpt.BlocksFilter.AttrsMustHave = new List<string>()
         {
            "ТИП", "МАРКА", "РАЗМЕР"
         };
            // Тип блока - атрибут ТИП = Монолит
            specApertureOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Проем", BlockPropType = EnumBlockProperty.Attribute };

            specApertureOpt.GroupPropName = ""; // Нет группировки
            specApertureOpt.KeyPropName = "МАРКА";

            // Свойства элемента блока
            specApertureOpt.ItemProps = new List<ItemProp>()
         {
            new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Размер", BlockPropName = "РАЗМЕР", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Отметка_низа", BlockPropName = "ОТМЕТКА_НИЗА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Назначение", BlockPropName = "НАЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
         };

            // Настройки Таблицы
            specApertureOpt.TableOptions = new TableOptions();
            specApertureOpt.TableOptions.Title = "Ведомость дверных и оконных проемов";
            specApertureOpt.TableOptions.Layer = "КР_Таблицы";
            specApertureOpt.TableOptions.Columns = new List<TableColumn>()
         {
            new TableColumn () { Name = "Марка отв.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 10 },
            new TableColumn () { Name = "Размеры, мм", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Размер", Width = 20 },
            new TableColumn () { Name = "Отм. низа проема, м", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Отметка_низа", Width = 20 },
            new TableColumn () { Name = "Назначение", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Назначение", Width = 20 },
            new TableColumn () { Name = "Кол-во, шт.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 15 },
            new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 30 },
         };

            return specApertureOpt;
        }
    }
}