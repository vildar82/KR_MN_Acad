using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.XData;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    class PileNumberingOptions : ITypedDataValues, IExtDataSave
    {
        //private static AcadLib.DictNOD dictNod = new AcadLib.DictNOD("PileNumberingOptions");
        //private const string RecOrder = "NumberingOrder";
        //private const string RecSide = "PileSide";
        //private const string RecPileStartNum = "PileStartNum";        

        [Category("Основное")]
        [DisplayName("Порядок нумерации")]
        [Description("Порядок нумерации свай.")]
        [DefaultValue(EnumNumberingOrder.RightToLeft)]
        [TypeConverter(typeof(EnumOrderConvertor))]
        public EnumNumberingOrder NumberingOrder { get; set; }

        [Browsable(false)]
        [Category("Основное")]
        [DisplayName("Сторона сваи")]
        [Description("Размер сваи.")]
        [DefaultValue(300)]
        public int PileSide { get; set; } = 300;

        [Category("Основное")]
        [DisplayName("Начальный номер")]
        [Description("Номер с которого начнется нумерация свай.")]
        [DefaultValue(1)]
        public int PileStartNum { get; set; } = 1;
        
        [Category("Дополнительно")]
        [DisplayName("Сброс блоков свай")]
        [Description("Установка стандартного положения блока сваи и атрибута номера - без поворота, масштабирования, зеркалирования.")]
        [DefaultValue(PileResetEnum.None)]
        [TypeConverter(typeof(EnumResetPosConvertor))]
        public PileResetEnum ResetPos { get; set; } = PileResetEnum.None;        

        public PileNumberingOptions PromptOptions()
        {
            PileNumberingOptions resVal = this;
            //Запрос начальных значений
            FormNumbering formNum = new FormNumbering((PileNumberingOptions)resVal.MemberwiseClone());
            if (Application.ShowModalDialog(formNum) != System.Windows.Forms.DialogResult.OK)
            {
                throw new System.Exception(AcadLib.General.CanceledByUser);
            }
            try
            {
                resVal = formNum.Options;
                resVal.Save();// Save(resVal);                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "Не удалось сохранить стартовые параметры.");
            }
            return resVal;
        }

        public void LoadDefault()
        {
            var dicNOD = new DictNOD(PileOptions.DicPileName, true);
            var dicOpt = dicNOD.LoadED("PileNumberingOptions");
            SetDataValues(dicOpt?.GetRec("Options")?.Values, null);
        }

        private void Save()
        {
            var dicNOD = new DictNOD(PileOptions.DicPileName, true);            
            dicNOD.Save(GetExtDic(null));            
        }

        public List<TypedValue> GetDataValues (Document doc)
        {
            return new List<TypedValue> {
                TypedValueExt.GetTvExtData("NumberingOrder"),
                TypedValueExt.GetTvExtData(NumberingOrder),
                TypedValueExt.GetTvExtData("PileStartNum"),
                TypedValueExt.GetTvExtData(PileStartNum),                
            };
        }

        public void SetDataValues (List<TypedValue> values, Document doc)
        {
            if (values == null) return;
            var dictValues = values.ToDictionary();
            NumberingOrder = dictValues.GetValue("NumberingOrder", EnumNumberingOrder.RightToLeft);
            PileStartNum = dictValues.GetValue("PileStartNum", 1);            
        }

        public DicED GetExtDic (Document doc)
        {
            var dic = new DicED("PileNumberingOptions");
            dic.AddRec("Options", GetDataValues(doc));
            return dic;
        }

        public void SetExtDic (DicED dic, Document doc)
        {
            SetDataValues(dic.GetRec("Options")?.Values, doc);
        }
    }           

    public class EnumOrderConvertor : System.ComponentModel.EnumConverter
    {
        public EnumOrderConvertor() : base(typeof(EnumNumberingOrder)) { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value.ToString())
            {
                case "Слева-направо":
                    return Enum.Parse(typeof(EnumNumberingOrder), "RightToLeft");
                case "Сверху-вниз":
                    return Enum.Parse(typeof(EnumNumberingOrder), "TopDown");
            }
            return Enum.Parse(typeof(EnumNumberingOrder), value.ToString());
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            switch (value.ToString())
            {
                case "RightToLeft":
                    return "Слева-направо";
                case "TopDown":
                    return "Сверху-вниз";
            }
            return value.ToString();                        
        }
    }

    public class EnumResetPosConvertor : System.ComponentModel.EnumConverter
    {
        public EnumResetPosConvertor() : base(typeof(PileResetEnum)) { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value.ToString())
            {
                case "Не изменять":
                    return PileResetEnum.None;
                case "По умолчанию":
                    return PileResetEnum.Default;
            }
            return PileResetEnum.None;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            switch (value.ToString())
            {
                case "None":
                    return "Не изменять";
                case "Default":
                    return "По умолчанию";
            }
            return value.ToString();
        }
    }
}
