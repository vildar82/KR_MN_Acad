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
            SpecOptions specOpt = new SpecOptions();

            specOpt.CheckDublicates = true;
            specOpt.Name = name;

            // Фильтр для блоков
            specOpt.BlocksFilter = new BlocksFilter();
            // Имя блока начинается с "КР_"
            specOpt.BlocksFilter.BlockNameMatch = "КР_Отв|КР_Гильза";
            // Обязательные атрибуты
            specOpt.BlocksFilter.AttrsMustHave = new List<string>()
            {
                "ТИП", "МАРКА", "НАЗНАЧЕНИЕ",  "РАЗМЕР"
            };
            // Тип блока - атрибут ТИП = Монолит
            specOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Отверстие", BlockPropType = EnumBlockProperty.Attribute };

            specOpt.GroupPropName = ""; // Нет группировки
            specOpt.KeyPropName = "МАРКА";

            // Свойства элемента блока
            specOpt.ItemProps = new List<ItemProp>()
            {
                new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Размер", BlockPropName = "РАЗМЕР", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Отметка", BlockPropName = "ОТМЕТКА", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Назначение", BlockPropName = "НАЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            };

            // Настройки Таблицы
            specOpt.TableOptions = new TableOptions();
            specOpt.TableOptions.Title = "Ведомость инженерных отверстий";
            specOpt.TableOptions.Layer = "КР_Таблицы";
            specOpt.TableOptions.Columns = new List<TableColumn>()
            {
                new TableColumn () { Name = "Марка отв.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 10 },
                new TableColumn () { Name = "Размеры, мм", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Размер", Width = 20 },
                new TableColumn () { Name = "Отм. низа проема, м", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Отметка", Width = 20 },
                new TableColumn () { Name = "Назначение", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Назначение", Width = 20 },
                new TableColumn () { Name = "Кол-во, шт.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 15 },
                new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 30 },
            };

            // Префиксы для параметров
            specOpt.PrefixParam = new XmlSerializableDictionary<string>
            {
                // Префикс для Гильзы - Ось отв.
                { "КР_Гильза" + "Отметка", "ось отв. " }
            };

            // Настройки нумерации
            specOpt.NumOptions = new NumberingOptions();
            specOpt.NumOptions.PrefixByBlockName = new XmlSerializableDictionary<string>
            {
                { "КР_Гильза", "Г" }
            };
            specOpt.NumOptions.ExGroupNumbering = "ОТМЕТКА";

            return specOpt;
        }
    }
}