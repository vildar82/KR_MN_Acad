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
      public string Name
      {
         get
         {
            return "КР_Спец_Проемы";
         }
      }

      public SpecOptions GetDefaultOptions()
      {
         SpecOptions specMonolOpt = new SpecOptions();

         specMonolOpt.Name = Name;

         // Фильтр для блоков
         specMonolOpt.BlocksFilter = new BlocksFilter();
         // Имя блока начинается с "КР_"
         specMonolOpt.BlocksFilter.BlockNameMatch = "^КР_Проем";
         // Обязательные атрибуты
         specMonolOpt.BlocksFilter.AttrsMustHave = new List<string>()
         {
            "ТИП", "МАРКА", "РАЗМЕР"
         };
         // Тип блока - атрибут ТИП = Монолит
         specMonolOpt.BlocksFilter.Type = new ItemProp() { BlockPropName = "ТИП", Name = "Проем", BlockPropType = EnumBlockProperty.Attribute };

         specMonolOpt.GroupPropName = ""; // Нет группировки
         specMonolOpt.KeyPropName = "МАРКА";

         // Свойства элемента блока
         specMonolOpt.ItemProps = new List<ItemProp>()
         {
            new ItemProp () { Name = "Марка", BlockPropName = "МАРКА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Размер", BlockPropName = "РАЗМЕР", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Отметка_низа", BlockPropName = "ОТМЕТКА_НИЗА", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Назначение", BlockPropName = "НАЗНАЧЕНИЕ", BlockPropType = EnumBlockProperty.Attribute },
            new ItemProp () { Name = "Примечание", BlockPropName = "ПРИМЕЧАНИЕ", BlockPropType = EnumBlockProperty.Attribute },
         };

         // Настройки Таблицы
         specMonolOpt.TableOptions = new TableOptions();
         specMonolOpt.TableOptions.Title = "Ведомость дверных и оконных проемов";
         specMonolOpt.TableOptions.Layer = "КР_Таблицы";
         specMonolOpt.TableOptions.Columns = new List<TableColumn>()
         {
            new TableColumn () { Name = "Марка отв.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Марка", Width = 10 },
            new TableColumn () { Name = "Размеры, мм", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Размер", Width = 20 },
            new TableColumn () { Name = "Отм. низа проема, м", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Отметка_низа", Width = 20 },
            new TableColumn () { Name = "Назначение", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Назначение", Width = 20 },
            new TableColumn () { Name = "Кол-во, шт.", Aligment = CellAlignment.MiddleCenter, ItemPropName = "Count", Width = 15 },
            new TableColumn () { Name = "Примечание", Aligment = CellAlignment.MiddleLeft, ItemPropName = "Примечание", Width = 30 },
         };

         return specMonolOpt;
      }      
   }
}