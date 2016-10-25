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
    class Options : ITypedDataValues, IExtDataSave
    {
        //private static AcadLib.DictNOD dictNod = new AcadLib.DictNOD("PileNumberingOptions");
        //private const string RecOrder = "NumberingOrder";
        //private const string RecSide = "PileSide";
        //private const string RecPileStartNum = "PileStartNum";        

        [Category("Пользовательские")]
        [DisplayName("Порядок нумерации")]
        [Description("Порядок нумерации свай.")]
        [DefaultValue(EnumNumberingOrder.RightToLeft)]
        [TypeConverter(typeof(EnumOrderConvertor))]
        public EnumNumberingOrder NumberingOrder { get; set; }

        [Browsable(false)]
        [Category("Пользовательские")]
        [DisplayName("Сторона сваи")]
        [Description("Размер сваи.")]
        [DefaultValue(300)]
        public int PileSide { get; set; } = 300;

        [Category("Пользовательские")]
        [DisplayName("Начальный номер")]
        [Description("Номер с которого начнется нумерация свай.")]
        [DefaultValue(1)]
        public int PileStartNum { get; set; } = 1;      

        public void LoadDefault()
        {            
            var dicNOD = new DictNOD("PileNumberingOptions", true);
            var dicOpt = dicNOD.LoadED();
            SetDataValues(dicOpt?.GetRec("Options")?.Values, null);
        }

        public Options PromptOptions()
        {
            Options resVal = this;
            //Запрос начальных значений
            FormNumbering formNum = new FormNumbering((Options)resVal.MemberwiseClone());
            if (Application.ShowModalDialog(formNum) != System.Windows.Forms.DialogResult.OK)
            {
                throw new System.Exception(AcadLib.General.CanceledByUser);
            }
            try
            {
                resVal = formNum.Options;
                Save(resVal);                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "Не удалось сохранить стартовые параметры.");
            }
            return resVal;
        }

        private void Save(Options opt)
        {
            var dicNOD = new DictNOD("PileNumberingOptions", true);            
            dicNOD.Save(GetExtDic(null));            
        }

        public List<TypedValue> GetDataValues (Document doc)
        {
            return new List<TypedValue> {
                TypedValueExt.GetTvExtData((int)NumberingOrder),
                TypedValueExt.GetTvExtData(PileStartNum),
            };
        }

        public void SetDataValues (List<TypedValue> values, Document doc)
        {
            if (values == null) return;
            try
            {
                NumberingOrder = (EnumNumberingOrder)values[0].GetTvValue<int>();
                PileStartNum = values[1].GetTvValue<int>();
            }
            catch 
            {                
            }
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
}
