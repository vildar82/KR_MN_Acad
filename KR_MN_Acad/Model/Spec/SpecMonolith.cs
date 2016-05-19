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
    public class SpecMonolith : ISpecCustom
    {
        private const string name = "КР_Спец_Монолит";

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
            specOpt.BlocksFilter.BlockNameMatch = "^КР_";
            // Обязательные атрибуты
            specOpt.BlocksFilter.AttrsMustHave = new List<string>()
            {
                "ТИП", "МАРКА", "НАИМЕНОВАНИЕ"
            };
            // Тип блока - атрибут ТИП = Монолит
            specOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Монолит", BlockPropType = EnumBlockProperty.Attribute };

            specOpt.GroupPropName = "ГРУППА";
            specOpt.KeyPropName = "МАРКА";

            // Свойства элемента блока
            specOpt.ItemProps = new List<ItemProp>()
            {
                new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Обозначение", BlockPropName = "ОБОЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Наименование", BlockPropName = "НАИМЕНОВАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Масса", BlockPropName = "МАССА", BlockPropType = EnumBlockProperty.Attribute },
                new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            };

            // Настройки Таблицы
            specOpt.TableOptions = new TableOptions();
            specOpt.TableOptions.Title = "Спецификация к схеме расположения элементов замаркированных на данном листе";
            specOpt.TableOptions.Layer = "КР_Таблицы";
            specOpt.TableOptions.Columns = new List<TableColumn>()
            {
                new TableColumn () { Name = "Марка", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 15 },
                new TableColumn () { Name = "Обозначение", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Обозначение", Width = 60 },
                new TableColumn () { Name = "Наименование", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Наименование", Width = 65 },
                new TableColumn () { Name = "Кол.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 10 },
                new TableColumn () { Name = "Масса, ед. кг", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Масса", Width = 15 },
                new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 20 },
            };

            // Настройки нумерации
            specOpt.NumOptions = new NumberingOptions();
            specOpt.NumOptions.PrefixByBlockName = new XmlSerializableDictionary<string, string>
            {
                { "КР_Колонна", "К-" },
                { "КР_Пилон", "П-" },
                { "КР_Балка", "Б-" },
                { "КР_Стена", "См-" }
            };

            return specOpt;
        }
    }
}