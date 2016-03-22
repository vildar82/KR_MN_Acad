using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autodesk.AutoCAD.ApplicationServices;

namespace KR_MN_Acad.Model.Pile
{
    [Serializable]
    public class PileOptions
    {   
        [XmlIgnore]
        public static string FileXml = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"КР-МН\Сваи\PileOptions.xml");

        [Category("Общие")]
        [DisplayName("Имя блока сваи")]
        [Description("^КР_свая - имя блока начинается с КР_свая. Регистр игнорируется.")]
        //[DefaultValue("^КР_свая")]
        public string PileBlockNameMatch { get; set; }

        [Category("Общие")]
        [DisplayName("Атрибут номера")]
        [Description("Тэг атрибута номера сваи.")]
        //[DefaultValue("ПОЗ")]
        public string PileAttrPos { get; set; }

        [Category("Общие")]
        [DisplayName("Минимальное расстояние")]
        [Description("Коэфициент минимального расстояния между сваями. Lmin = k * 'сторона сваи'.")]
        //[DefaultValue(3)]
        public double PileRatioLmin { get; set; }

        public PileOptions PromptOptions()
        {
            PileOptions resVal = this;
            //Запрос начальных значений
            AcadLib.UI.FormProperties formProp = new AcadLib.UI.FormProperties();
            PileOptions thisCopy = (PileOptions)resVal.MemberwiseClone();
            formProp.propertyGrid1.SelectedObject = thisCopy;
            if (Application.ShowModalDialog(formProp) != System.Windows.Forms.DialogResult.OK)
            {
                throw new System.Exception(AcadLib.General.CanceledByUser);
            }
            try
            {
                resVal = thisCopy;
                Save();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "Не удалось сохранить стартовые параметры.");
            }
            return resVal;
        }

        public static PileOptions Load()
        {
            PileOptions options = null;
            if (File.Exists(FileXml))
            {
                try
                {
                    // Загрузка настроек таблицы из файла XML
                    options = PileOptions.LoadFromXml();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex, $"Ошибка при попытке загрузки настроек таблицы из XML файла {FileXml}");
                }
            }

            if (options == null)
            {
                // Создать дефолтные
                options = new PileOptions();
                options.SetDefault();
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
            return options;
        }

        private void SetDefault()
        {
            PileBlockNameMatch= "^КР_свая";
            PileAttrPos = "ПОЗ";
            PileRatioLmin = 3;
        }

        private static PileOptions LoadFromXml()
        {
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            return ser.DeserializeXmlFile<PileOptions>();
        }

        public void Save()
        {            
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            ser.SerializeList(this);
        }
    }
}
