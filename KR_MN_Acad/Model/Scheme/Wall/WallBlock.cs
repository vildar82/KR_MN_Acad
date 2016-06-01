using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока стены
    /// </summary>
    public class WallBlock : SchemeBlock
    {
        const string PropNameConcrete = "Бетон";
        const string PropNameHeight = "Высота";
        const string PropNameLength = "Длина стены";
        const string PropNameThickness = "Толщина стены";
        const string PropNameOutline = "Выпуск";
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticStep = "ШагВертикАрм";
        const string PropNameArmHorDiam = "ДиамГорАрм";
        const string PropNameArmHorStep = "ШагГорАрм";

        /// <summary>
        /// Защитный салой бктона
        /// </summary>
        private const int a = 45;

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
        public ArmatureDivision ArmVertic { get; set; }
        /// <summary>
        /// Горизонтальные арматурные стержни - погоннаж
        /// </summary>
        public ArmatureRunning ArmHor { get; set; }

        public WallBlock(BlockReference blRef, string blName) : base (blRef, blName)
        {
            
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

        public override List<RowScheme> GetElements()
        {
            List<RowScheme> elems = new List<RowScheme>();

            elems.Add(ArmVertic.Armature);
            elems.Add(ArmHor);

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
        }                

        private ArmatureDivision defineArmVertic ()
        {
            int diam = GetPropValue<int>(PropNameArmVerticDiam);
            int step = GetPropValue<int>(PropNameArmVerticStep);
            int width = getWidthVerticArm(step);
            int len = Height + Outline;
            return new ArmatureDivision(diam, len, width, step);
        }

        private ArmatureRunning defineArmHor()
        {
            int diam = GetPropValue<int>(PropNameArmHorDiam);
            int step = GetPropValue<int>(PropNameArmHorStep);
            int width = Height - 100;
            int len = getLengthHorArm();
            return new ArmatureRunning(diam, len, width, step);
        }        

        /// <summary>
        /// преобразование object value свойства Property в указанный тип
        /// Тип T должен точно соответствовать типу object value Property
        /// </summary>        
        private T GetPropValue<T>(string propName)
        {
            T resVal = default(T);
            Property prop;
            if (!Properties.TryGetValue(propName, out prop))
            {
                AddError($"Не определен параметр {propName}.");
            }
            else
            {
                resVal = (T)Convert.ChangeType(prop.Value, typeof(T));
            }
            return resVal;
        }

        /// <summary>
        /// Определение ширины распределения вертикальных стержней в стене
        /// </summary>        
        private int getWidthVerticArm(int step)
        {            
            // Вычесть отступ у торцов стены = шаг вертик стержней - а.
            int indent = step - a;
            return Length - (indent * 2);
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>
        /// <returns></returns>
        private int getLengthHorArm()
        {
            return Length;
        }        
    }
}