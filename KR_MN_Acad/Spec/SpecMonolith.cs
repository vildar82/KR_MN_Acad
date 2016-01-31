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
      public string Name
      {
         get
         {
            return "КР_Спец_Монолит";
         }
      }

      public SpecOptions GetDefaultOptions()
      {
         SpecOptions specMonolOpt = new SpecOptions();

         specMonolOpt.Name = Name;

         // Фильтр для блоков
         specMonolOpt.BlocksFilter = new BlocksFilter();
         // Имя блока начинается с "КР_"
         specMonolOpt.BlocksFilter.BlockNameMatch = "^КР_";
         // Обязательные атрибуты
         specMonolOpt.BlocksFilter.AttrsMustHave = new List<string>()
         {
            "ТИП", "МАРКА", "НАИМЕНОВАНИЕ"
         };
         // Тип блока - атрибут ТИП = Монолит
         specMonolOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Монолит", BlockPropType = EnumBlockProperty.Attribute };

         specMonolOpt.GroupPropName = "ГРУППА";
         specMonolOpt.KeyPropName = "МАРКА";

         // Свойства элемента блока
         specMonolOpt.ItemProps = new List<ItemProp>()
         {
            new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Обозначение", BlockPropName = "ОБОЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Наименование", BlockPropName = "НАИМЕНОВАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Масса", BlockPropName = "МАССА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
         };

         // Настройки Таблицы
         specMonolOpt.TableOptions = new TableOptions();
         specMonolOpt.TableOptions.Title = "Спецификация к схеме расположения элементов замаркированных на данном листе";
         specMonolOpt.TableOptions.Layer = "КР_Таблицы";
         specMonolOpt.TableOptions.Columns = new List<TableColumn>()
         {
            new TableColumn () { Name = "Марка", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 15 },
            new TableColumn () { Name = "Обозначение", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Обозначение", Width = 60 },
            new TableColumn () { Name = "Наименование", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Наименование", Width = 65 },
            new TableColumn () { Name = "Кол.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 10 },
            new TableColumn () { Name = "Масса, ед. кг", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Масса", Width = 15 },
            new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 20 },
         };

         return specMonolOpt;
      }      
   }
}