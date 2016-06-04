using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.ConstructionServices.Materials;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока стены
    /// </summary>
    public class WallBlock : SchemeBlock
    {
        private SchemeService service;        
                     
        const string PropNameConcrete = "Бетон";
        const string PropNameHeight = "Высота стены";
        const string PropNameLength = "Длина стены";
        const string PropNameThickness = "Толщина стены";
        const string PropNameOutline = "Выпуск";
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticStep = "ШагВертикАрм";
        const string PropNameArmHorDiam = "ДиамГорАрм";
        const string PropNameArmHorStep = "ШагГорАрм";
        const string PropNameSpringDiam = "ДиамШпилек";
        const string PropNameSpringStepHor = "ШагШпилекГор";
        const string PropNameSpringStepVertic = "ШагШпилекВерт";
        const string PropNamePosVerticArm = "ПОЗВЕРТИКАРМ";
        const string PropNamePosHorArm = "ПОЗГОРАРМ";
        const string PropNamePosSpring = "ПОЗШПИЛЬКИ";
        const string PropNameDescHorArm = "ОПИСАНИЕГОРАРМ";
        const string PropNameDescVerticArm = "ОПИСАНИЕВЕРТИКАРМ";
        const string PropNameDescSpring = "ОПИСАНИЕШПИЛЬКИ";

        /// <summary>
        /// Защитный салой бктона
        /// </summary>
        private const int a = 45;
        /// <summary>
        /// Отступ вертикальной арматуры от торца стены
        /// </summary>
        private int indentVerticArm;

        /// <summary>
        /// Класс Бетона
        /// </summary>
        public string Concrete { get; set; }
        /// <summary>
        /// Высота стены
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Длина стены
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Толщина стены
        /// </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// Выпуск стержней по вертикали
        /// </summary>
        public int Outline { get; set; }
        /// <summary>
        /// Распределенные вертикальные арматурные стержни
        /// </summary>
        public BarDivision ArmVertic { get; set; }
        /// <summary>
        /// Горизонтальные арматурные стержни - погоннаж
        /// </summary>
        public BarRunningStep ArmHor { get; set; }
        /// <summary>
        /// Шпильки
        /// </summary>
        public Spring Spring { get; set; }

        public WallBlock(BlockReference blRef, string blName, SchemeService service) : base (blRef, blName)
        {
            this.service = service;            
        }

        public override void Calculate()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                defineFields();
            }
            catch(Exception ex)
            {
                AddError(ex.Message);
            }
        }

        public override List<IElement> GetElements()
        {
            List<IElement> elems = new List<IElement>();

            elems.Add(ArmVertic);
            elems.Add(ArmHor);
            elems.Add(Spring);

            return elems;
        }

        private void defineFields()
        {
            Concrete = GetPropValue<string>(PropNameConcrete);
            Height = Convert.ToInt32(GetPropValue<double>(PropNameHeight));
            Length = Convert.ToInt32(GetPropValue<double>(PropNameLength));
            Thickness = Convert.ToInt32(GetPropValue<double>(PropNameThickness));
            Outline = Convert.ToInt32(GetPropValue<double>(PropNameOutline));
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor();
            // Шпильки
            Spring = defineSpring();
        }

        

        private BarDivision defineArmVertic ()
        {
            int diam = GetPropValue<int>(PropNameArmVerticDiam);
            int step = GetPropValue<int>(PropNameArmVerticStep);
            int width = getWidthVerticArm(step);
            int len = Height + Outline;            
            var armDiv = new BarDivision(diam, len, width, step);
            armDiv.Calc();
            return armDiv;
        }

        private BarRunningStep defineArmHor()
        {
            int diam = GetPropValue<int>(PropNameArmHorDiam);
            int step = GetPropValue<int>(PropNameArmHorStep);
            int width = Height - 100;
            int len = getLengthHorArm();
            var armHor = new BarRunningStep (diam, len, width, step);
            armHor.Calc();
            return armHor;                    
        }

        private Spring defineSpring()
        {
            int diam = GetPropValue<int>(PropNameSpringDiam);
            int stepHor = GetPropValue<int>(PropNameSpringStepHor);
            int stepVert = GetPropValue<int>(PropNameSpringStepVertic);
            int len = (Thickness - (2 * a)) + 2 * 75;

            // кол шпилек по горизонтале
            int countHor = (Length - indentVerticArm * 2) / stepHor + 1;
            int countVert = Height  / stepVert + 1;
            int countSprings = countHor * countVert;

            Spring sp = new Spring(diam, len, stepHor, stepVert, countSprings);
            sp.Calc();
            return sp;
        }

        /// <summary>
        /// преобразование object value свойства Property в указанный тип
        /// Тип T должен точно соответствовать типу object value Property
        /// </summary>        
        private T GetPropValue<T>(string propName)
        {
            T resVal = default(T);
            Property prop = GetProperty(propName);
            if (prop != null)
            {
                resVal = (T)Convert.ChangeType(prop.Value, typeof(T));
            }
            return resVal;
        }

        private Property GetProperty(string propName)
        {
            Property prop;
            if (!Properties.TryGetValue(propName, out prop))
            {
                AddError($"Не определен параметр {propName}.");
            }
            return prop;            
        }


        /// <summary>
        /// Определение ширины распределения вертикальных стержней в стене
        /// </summary>        
        private int getWidthVerticArm(int step)
        {
            // Вычесть отступ у торцов стены = шаг вертик стержней - а.
            indentVerticArm = step - a;
            return Length - (indentVerticArm * 2);
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>
        /// <returns></returns>
        private int getLengthHorArm()
        {
            return Length;
        }

        /// <summary>
        /// Заполнение позиций
        /// </summary>
        public override void Numbering()
        {
            // ПозГорАрм
            FillProp(GetProperty(PropNamePosHorArm), ArmHor.SpecRow.PositionColumn);
            // ПозВертикАрм
            FillProp(GetProperty(PropNamePosVerticArm), ArmVertic.SpecRow.PositionColumn);
            // ПозШпильки
            FillProp(GetProperty(PropNamePosSpring), Spring.SpecRow.PositionColumn);
            // ОписГорАрм
            FillProp(GetProperty(PropNameDescHorArm), ArmHor.GetDesc());
            // ОписВертикАрм
            FillProp(GetProperty(PropNameDescVerticArm), ArmVertic.GetDesc());
            // ОписШпилтьки
            FillProp(GetProperty(PropNameDescSpring), Spring.GetDesc());
        }

        private void FillProp(Property prop, object value)
        {
            if (prop == null) return;
            if (prop.Type == PropertyType.Attribute && !prop.IdAtrRef.IsNull)
            {
                using (var atr = prop.IdAtrRef.GetObject(OpenMode.ForWrite, false, true) as AttributeReference)
                {
                    atr.TextString = value.ToString();
                }
            }
        }
    }
}