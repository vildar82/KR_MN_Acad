using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    class Options
    {
        private static AcadLib.DictNOD dictNod = new AcadLib.DictNOD("PileNumberingOptions");
        private const string RecOrder = "NumberingOrder";
        private const string RecRowWidth = "PileRowWidth";
        private const string RecPileStartNum = "PileStartNum";
        

        [Category("Пользовательские")]
        [DisplayName("Порядок нумерации")]
        [Description("Порядок нумерации свай.")]
        [DefaultValue(EnumNumberingOrder.RightToLeft)]
        [TypeConverter(typeof(EnumOrderConvertor))]
        public EnumNumberingOrder NumberingOrder { get; set; }

        [Category("Пользовательские")]
        [DisplayName("Ширина ряда")]
        [Description("Расстояние между сваями одного ряда.")]
        [DefaultValue(600)]
        public int PileRowWidth { get; set; }

        [Category("Пользовательские")]
        [DisplayName("Начальный номер")]
        [Description("Номер с которого начнется нумерация свай.")]
        [DefaultValue(1)]
        public int PileStartNum { get; set; }

        [Category("Общие")]
        [DisplayName("Имя блока сваи")]
        [Description("^КР_свая - имя блока начинается с КР_свая. Регистр игнорируется.")]
        [DefaultValue("^КР_свая")]
        public string PileBlockNameMatch { get; set; }

        [Category("Общие")]
        [DisplayName("Атрибут номера")]
        [Description("Тэг атрибута номера сваи.")]
        [DefaultValue("ПОЗ")]
        public string PileAttrPos { get; set; }        

        public void LoadDefault()
        {
            NumberingOrder = (EnumNumberingOrder)dictNod.Load(RecOrder, 0);
            PileBlockNameMatch = "^КР_свая";
            PileAttrPos = "ПОЗ";
            PileRowWidth = dictNod.Load(RecRowWidth, 600);
            PileStartNum = dictNod.Load(RecPileStartNum, 1);
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
            dictNod.Save((int)opt.NumberingOrder, RecOrder);
            dictNod.Save(opt.PileRowWidth, RecRowWidth);
            dictNod.Save(opt.PileStartNum, RecPileStartNum);            
        }
    }           

    public class EnumOrderConvertor : EnumConverter
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
