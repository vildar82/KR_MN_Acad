﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока стены "КР_Арм_Стена"
    /// </summary>
    public class WallBlock : SchemeBlock
    {        
        public const string WallBlockName = "КР_Арм_Стена";     
                     
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
        public ConcreteH Concrete { get; set; }

        public WallBlock(BlockReference blRef, string blName, SchemeService service) : base (blRef, blName, service)
        {            
        }

        public override void Calculate()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                defineFields();
                AddElements();
            }
            catch(Exception ex)
            {
                AddError(ex.Message);
            }
        }

        private void AddElements()
        {           
            AddElement(ArmHor);
            AddElement(ArmVertic);
            AddElement(Spring);
            AddElement(Concrete);
        }

        private void defineFields()
        {
            Length = Convert.ToInt32(GetPropValue<double>(PropNameLength));
            Height = Convert.ToInt32(GetPropValue<double>(PropNameHeight));            
            Thickness = Convert.ToInt32(GetPropValue<double>(PropNameThickness));
            Outline = Convert.ToInt32(GetPropValue<double>(PropNameOutline));
            var concrete = GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor();
            // Шпильки
            Spring = defineSpring();
        }        

        private BarDivision defineArmVertic ()
        {
            string pos = GetPropValue<string>(PropNamePosVerticArm);
            int diam = GetPropValue<int>(PropNameArmVerticDiam);
            int step = GetPropValue<int>(PropNameArmVerticStep);
            int width = getWidthVerticArm(step);
            int len = Height + Outline;            
            var armDiv = new BarDivision(diam, len, width, step, 2, pos, this, "Вертикальная арматура");
            armDiv.Calc();            
            return armDiv;
        }

        private BarRunningStep defineArmHor()
        {
            string pos = GetPropValue<string>(PropNamePosHorArm);
            int diam = GetPropValue<int>(PropNameArmHorDiam);
            int step = GetPropValue<int>(PropNameArmHorStep);
            int width = Height - 100;
            double len = getLengthHorArm(diam, Concrete.ClassB);
            var armHor = new BarRunningStep (diam, len, width, step, 2, pos, this, "Горизонтальная арматура");
            armHor.Calc();
            return armHor;                    
        }

        private Spring defineSpring()
        {
            string pos = GetPropValue<string>(PropNamePosSpring);
            int diam = GetPropValue<int>(PropNameSpringDiam);
            int stepHor = GetPropValue<int>(PropNameSpringStepHor);
            int stepVert = GetPropValue<int>(PropNameSpringStepVertic);
            int len = (Thickness - (2 * a)) + 2 * 75 + ArmVertic.Diameter;

            // ширина распределения шпилек по горизонтале
            int widthHor = Length;
            int widthVertic = Height;

            Spring sp = new Spring(diam, len, stepHor, stepVert, widthHor, widthVertic, pos, this);
            sp.Calc();
            return sp;
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
        private double getLengthHorArm(int diam, string classB)
        {
            double kLap = BarRunning.GetKLap(diam, classB);
            return Length * kLap;
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
    }
}