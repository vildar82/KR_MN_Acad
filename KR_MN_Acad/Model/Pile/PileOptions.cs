﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AcadLib;
using AcadLib.XData;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Model.Pile
{
    [Serializable]
    public class PileOptions : IExtDataSave, ITypedDataValues
    {           
        public static string FileXml = Path.Combine(AutoCAD_PIK_Manager.Settings.PikSettings.ServerShareSettingsFolder, @"КР-МН\Сваи\PileOptions.xml");
        public const string DicPluginName = "KR_MN";
        public const string DicPileName = "Pile";
        public const string RecAbsoluteZero = "AbsoluteZero";
        public const string RecDimPileBeatToCut = "Срубка";
        public const string RecDimPileCutToRostwerk = "Заделка";
        public const string RecSchema = "Схема";

        [Browsable(false)]
        //[Category("Общие")]
        //[DisplayName("Имя блока сваи")]
        //[Description("^КР_свая - имя блока начинается с КР_свая. Регистр игнорируется.")]
        //[DefaultValue("^КР_свая")]
        public string PileBlockNameMatch { get; set; } = "^КР_свая";

        [Browsable(false)]
        //[Category("Общие")]
        //[DisplayName("Атрибут номера")]
        //[Description("Тэг атрибута номера сваи.")]
        //[DefaultValue("ПОЗ")]
        public string PileAttrPos { get; set; } = "ПОЗ";

        [Category("Общие")]
        [DisplayName("Минимальное расстояние")]
        [Description("Коэфициент минимального расстояния между сваями. Lmin = k * 'сторона сваи'. При нумерации свай, будет проверяться соблюдение минимального расстояния между сваями.")]
        [DefaultValue(3)]
        public double PileRatioLmin { get; set; } = 3;

        [XmlIgnore]
        [Category("Ростверк")]
        [DisplayName("Срубка, мм")]
        [Description("Расстояние от верха сваи до срубки, мм. \nХранится в чертеже.")]
        [DefaultValue(250)]
        public double DimPileBeatToCut { get; set; } = 250;

        [XmlIgnore]
        [Category("Ростверк")]
        [DisplayName("Заделка, мм")]
        [Description("Расстояние от низа ростверка до верха сваи после срубки, мм. \nДля шарнирного узла заделки сваи - 50мм, для жесткого - 100мм. \nХранится в чертеже.")]
        [DefaultValue(50)]
        public double DimPileCutToRostwerk { get; set; } = 50;

        [XmlIgnore]
        [Category("Ростверк")]
        [DisplayName("Абс.отметка")]
        [Description("Абсолютная отметка 0. Например 141.700. \nХранится в чертеже.")]
        [DefaultValue(150.00)]
        public double AbsoluteZero { get; set; } = 150.00;

        //[XmlIgnore]
        //[Category("Ростверк")]
        //[DisplayName("Схема")]
        //[Description("Схема ростверка - обычная или с промежуточной плитой.")]
        //[DefaultValue(SchemaEnum.Normal)]
        //public SchemaEnum Schema { get; set; }

        [Browsable(false)]
        //[Category("Разное")]
        //[DisplayName("Слой таблиц")]
        //[Description("Слой для вставки таблиц свай.")]
        //[DefaultValue("КР_Таблицы")]
        public string TableLayer { get; set; } = "КР_Таблицы";

        public PileOptions PromptOptions()
        {
            PileOptions resVal = this;
            //Запрос начальных значений
            AcadLib.UI.FormProperties formProp = new AcadLib.UI.FormProperties();
            PileOptions thisCopy = (PileOptions)resVal.MemberwiseClone();
            formProp.propertyGrid1.SelectedObject = thisCopy;
            if (Application.ShowModalDialog(formProp) != System.Windows.Forms.DialogResult.OK)
            {
                throw new Exception(General.CanceledByUser);
            }
            try
            {
                resVal = thisCopy;
                resVal.Save();
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

        private static PileOptions LoadFromXml()
        {
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            return ser.DeserializeXmlFile<PileOptions>();
        }

        public void Save()
        {
            SaveToNOD();
            AcadLib.Files.SerializerXml ser = new AcadLib.Files.SerializerXml(FileXml);
            ser.SerializeList(this);
        }

        private void SaveToNOD()
        {
            var nod = new DictNOD(DicPluginName, true);
            DicED dicPile = GetExtDic(null);            
            nod.Save(dicPile);
            //nod.Save(AbsoluteZero, RecAbsoluteZero);
            //nod.Save(DimPileBeatToCut, RecDimPileBeatToCut);
            //nod.Save(DimPileCutToRostwerk, RecDimPileCutToRostwerk);
        }

        private void LoadFromNOD()
        {
            var nod = new DictNOD(DicPluginName, true);
            SetExtDic(nod.LoadED(DicPileName), null);
            //AbsoluteZero = nod.Load(RecAbsoluteZero, AbsoluteZero);
            //DimPileBeatToCut = nod.Load(RecDimPileBeatToCut, DimPileBeatToCut);
            //DimPileCutToRostwerk = nod.Load(RecDimPileCutToRostwerk, DimPileCutToRostwerk);
        }

        public DicED GetExtDic (Document doc)
        {
            DicED dic = new DicED(DicPileName);
            dic.AddRec("Options", GetDataValues(null));
            return dic;
        }

        public void SetExtDic (DicED dicPile, Document doc)
        {            
            SetDataValues(dicPile?.GetRec("Options")?.Values, null);
        }

        public List<TypedValue> GetDataValues (Document doc)
        {
            return new List<TypedValue> {
                TypedValueExt.GetTvExtData(AbsoluteZero),
                TypedValueExt.GetTvExtData(DimPileBeatToCut),
                TypedValueExt.GetTvExtData(DimPileCutToRostwerk),
                //TypedValueExt.GetTvExtData(Schema),
            };
        }

        public void SetDataValues (List<TypedValue> values, Document doc)
        {
            if (values == null) return;
            try
            {
                int i = 0;
                AbsoluteZero = values[i++].GetTvValue<double>();
                DimPileBeatToCut = values[i++].GetTvValue<double>();
                DimPileCutToRostwerk = values[i++].GetTvValue<double>();
                //Schema = (SchemaEnum)values[i++].GetTvValue<int>();
            }
            catch
            {                
            }
        }
    }
}
