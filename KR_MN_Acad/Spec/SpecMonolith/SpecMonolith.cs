using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using SpecBlocks;
using SpecBlocks.Options;

namespace KR_MN_Acad.Spec.SpecMonolith
{
   /// <summary>
   /// Спецификация монолитных блоков
   /// </summary>
   public class SpecMonolith
   {
      private const string SpecTemplateName = "КР_Спец_Монолит";           

      public void Spec()
      {
         // Настройки спецификации монолитных конструкций
         SpecOptions specMonilithOptions = getSpecOptions();

         // Клас создания таблицы по заданным настройкам
         SpecTable specMonolith = new SpecTable(specMonilithOptions);
         specMonolith.CreateTable();
      }

      private SpecOptions getSpecOptions()
      {
         var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SpecTemplateName + ".xml");
         SpecOptions specOptionsMonilith;
         try
         {
            specOptionsMonilith = SpecOptions.Load(file);
         }
         catch (Exception ex)
         {
            Commands.Log.Error(ex, $"Попытка загрузки настроек таблицы из XML по имени {SpecTemplateName}");
            // Создать дефолтные
            specOptionsMonilith = getDefaultSpecMonolithOptions();
            // Сохранение дефолтных настроек 
            try
            {
               specOptionsMonilith.Save(file);
            }
            catch (Exception exSave)
            {
               Commands.Log.Error(exSave, $"Попытка сохранение настроек таблицы из XML по имени {SpecTemplateName}");
            }
         }

         return specOptionsMonilith;
      } 

      private SpecOptions getDefaultSpecMonolithOptions()
      {
         SpecOptions specMonolOpt = new SpecOptions();

         specMonolOpt.Name = SpecTemplateName;

         // Фильтр для блоков
         specMonolOpt.BlocksFilter = new BlocksFilter();
         // Имя блока начинается с "КР_"
         specMonolOpt.BlocksFilter.BlockNameMatch = "^КР_";
         // Обязательные атрибуты
         specMonolOpt.BlocksFilter.AttrsMustHave = new List<string>()
         {
            "ТИП", "МАРКА", "НАИМЕНОВАНИЕ"
         };

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