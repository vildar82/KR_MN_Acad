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
    public class SpecOpenings : ISpecCustom
    {
        private const string name = "КР_Спец_Отверстия";

        public string File
        {
            get
            {
                return Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"КР-МН\Спецификации\" + name + ".xml");
            }
        }

        public SpecOptions GetDefaultOptions()
        {
            SpecOptions specHoleOpt = new SpecOptions();

            specHoleOpt.CheckDublicates = true;
            specHoleOpt.Name = name;

            // Фильтр для блоков
            specHoleOpt.BlocksFilter = new BlocksFilter();
            // Имя блока начинается с "КР_"
            specHoleOpt.BlocksFilter.BlockNameMatch = "^КР_Отв";
            // Обязательные атрибуты
            specHoleOpt.BlocksFilter.AttrsMustHave = new List<string>()
         {
            "ТИП", "МАРКА", "РАЗМЕР"
         };
            // Тип блока - атрибут ТИП = Монолит
            specHoleOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Отверстие", BlockPropType = EnumBlockProperty.Attribute };

            specHoleOpt.GroupPropName = ""; // Нет группировки
            specHoleOpt.KeyPropName = "МАРКА";

            // Свойства элемента блока
            specHoleOpt.ItemProps = new List<ItemProp>()
         {
            new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Размер", BlockPropName = "РАЗМЕР", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Отметка_низа", BlockPropName = "ОТМЕТКА_НИЗА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Назначение", BlockPropName = "НАЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
         };

            // Настройки Таблицы
            specHoleOpt.TableOptions = new TableOptions();
            specHoleOpt.TableOptions.Title = "Ведомость инженерных отверстий";
            specHoleOpt.TableOptions.Layer = "КР_Таблицы";
            specHoleOpt.TableOptions.Columns = new List<TableColumn>()
         {
            new TableColumn () { Name = "Марка отв.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 10 },
            new TableColumn () { Name = "Размеры, мм", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Размер", Width = 20 },
            new TableColumn () { Name = "Отм. низа проема, м", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Отметка_низа", Width = 20 },
            new TableColumn () { Name = "Назначение", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Назначение", Width = 20 },
            new TableColumn () { Name = "Кол-во, шт.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 15 },
            new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 30 },
         };

            return specHoleOpt;
        }
    }
}