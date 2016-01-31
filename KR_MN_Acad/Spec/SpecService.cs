using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpecBlocks;
using SpecBlocks.Options;

namespace KR_MN_Acad.Spec
{
   public class SpecService
   {
      private static bool isLoadedSpecBlocks = false;
      private ISpecCustom specCustom;  
      
      public SpecService (ISpecCustom specCustom)
      {
         this.specCustom = specCustom;
         // Загрузка сборки SpecBlocks
         LoadSpecBlocks();
      }

      public void Spec()
      {
         SpecOptions specOpt = getSpecOptions();
         if (specOpt == null)
         {
            throw new Exception("Настройки таблицы не определены.");
         }
         // Клас создания таблицы по заданным настройкам
         SpecTable specTable = new SpecTable(specOpt);
         specTable.CreateTable();
      }

      private SpecOptions getSpecOptions()
      {
         // Путь к файлу настроек таблицы - по имени спецификации
         var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), specCustom.Name + ".xml");
         SpecOptions specOptions = null;
         if (File.Exists(file))
         {
            try
            {
               // Загрузка настроек таблицы из файла XML
               specOptions = SpecOptions.Load(file);
            }
            catch (Exception ex)
            {
               Commands.Log.Error(ex, $"Ошибка при попытке загрузки настроек таблицы из XML файла {file}");               
            }
         }

         if (specOptions == null)
         {
            // Создать дефолтные
            specOptions = specCustom.GetDefaultOptions();
            // Сохранение дефолтных настроек 
            try
            {
               specOptions.Save(file);
            }
            catch (Exception exSave)
            {
               Commands.Log.Error(exSave, $"Попытка сохранение настроек в файл {file}");
            }
         }

         return specOptions;
      }

      private static void LoadSpecBlocks()
      {
         if (isLoadedSpecBlocks)
         {
            return;
         }
         // Загрузка сборки SpecBlocks
         var dllSpecBlocks = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.LocalSettingsFolder, @"Script\NET\SpecBlocks\SpecBlocks.dll");
         if (File.Exists(dllSpecBlocks))
         {
            try
            {
               Assembly.LoadFrom(dllSpecBlocks);
               isLoadedSpecBlocks = true;
            }
            catch (Exception ex)
            {
               throw ex;
            }
         }
         else
         {
            throw new Exception($"Не найден файл {dllSpecBlocks}.");
         }
      }
   }
}
