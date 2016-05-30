using System;
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
    [Serializable]
    public class WallSchemeOptions
    {           
        static string FileXml = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"КР-МН\Схемы\WallSchemeOptions.xml");
        const string DictNod = "PIK";
        const string RecAbsoluteZero = "AbsoluteZero";        

        [Category("Общие")]
        [DisplayName("Имя блока стены")]
        [Description("Соответствие имени блока.")]
        [DefaultValue("КР_Арм_Схема_Стена")]
        public string WallBlockNameMatch { get; set; } = "КР_Арм_Схема_Стена";        

        public WallSchemeOptions PromptOptions()
        {
            WallSchemeOptions resVal = this;
            //Запрос начальных значений
            AcadLib.UI.FormProperties formProp = new AcadLib.UI.FormProperties();
            WallSchemeOptions thisCopy = (WallSchemeOptions)resVal.MemberwiseClone();
            formProp.propertyGrid1.SelectedObject = thisCopy;
            if (Application.ShowModalDialog(formProp) != System.Windows.Forms.DialogResult.OK)
            {
                throw new Exception(General.CanceledByUser);
            }
            try
            {
                resVal = thisCopy;
                Save();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "Не удалось сохранить параметры.");
            }
            return resVal;
        }

        public static WallSchemeOptions Load()
        {
            WallSchemeOptions options = null;
            if (File.Exists(FileXml))
            {
                try
                {
                    // Загрузка настроек таблицы из файла XML
                    options = LoadFromXml();                    
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"Ошибка при попытке загрузки настроек таблицы из XML файла {FileXml}");
                }
            }
            if (options == null)
            {
                // Создать дефолтные
                options = new WallSchemeOptions();                
                // Сохранение дефолтных настроек 
                try
                {
                    options.Save();
                }
                catch (Exception exSave)
                {
                    Logger.Log.Error(exSave, $"Попытка сохранение настроек в файл {FileXml}");
                }
            }
            // Загрузка начтроек чертежа
            options.LoadFromNOD();

            return options;
        }               

        private static WallSchemeOptions LoadFromXml()
        {
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            return ser.DeserializeXmlFile<WallSchemeOptions>();
        }

        public void Save()
        {
            SaveToNOD();
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            ser.SerializeList(this);
        }

        private void SaveToNOD()
        {
            //var nod = new DictNOD(DictNod);
            //nod.Save(AbsoluteZero, RecAbsoluteZero);            
        }

        private void LoadFromNOD()
        {
            //var nod = new DictNOD(DictNod);
            //AbsoluteZero = nod.Load(RecAbsoluteZero, AbsoluteZero);            
        }
    }
}
