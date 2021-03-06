﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using static AcadLib.Units.UnitsConvertHelper;
using static AcadLib.General;
using KR_MN_Acad.Spec.Bill;

namespace KR_MN_Acad.Spec.Materials
{
    /// <summary>
    /// арматурный стержень
    /// </summary>
    public abstract class Armature : IMaterial, IBillMaterial
    {
        public const string ClassA500C = "А500С";
        public const string ClassA240C = "А240С";
        public const string GostNewNumber = "ГОСТ Р 52544-2006";//http://docs.cntd.ru/document/gost-r-52544-2006
        public const string GostNewName = "Прокат арматурный свариваемый периодического профиля классов А500С и В500С для армирования железобетонных конструкций.";
        public const string GostOldNumber = "ГОСТ 5781-82";//http://docs.cntd.ru/document/gost-5781-82
        public const string GostOldName = "Сталь горячекатаная для армирования железобетонных конструкций.";
        public static Gost GostNew = new Gost (GostNewNumber, GostNewName);
        public static Gost GostOld = new Gost (GostOldNumber, GostOldName);

        public const int DefaultDiameter = 10;
        public const string DefaultClass = ClassA500C;
        public static readonly Gost DefaultGost = GostNew;       

        /// <summary>
        /// Наименование - "⌀12 A500C"
        /// </summary>
        public string Name { get { return Symbols.Diam + Diameter + " " + Class; } }
        /// <summary>
        /// Диаметр
        /// </summary>
        public int Diameter { get; set; }
        /// <summary>
        /// Длина
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Масса 1 п.м.,кг
        /// </summary>
        public double WeightUnit { get; private set; }
        /// <summary>
        /// Площадь поперечного сечения, см2
        /// </summary>
        public double Area { get; private set; }
        /// <summary>
        /// ГОСТ
        /// </summary>
        public Gost Gost { get; set; } = DefaultGost;
        /// <summary>
        /// Класс арматуры
        /// </summary>
        public string Class { get; set; } = DefaultClass;
        public string BillTitle { get; set; } = "Изделия арматурные, кг";
        public int BillTitleIndex { get; } = 0;
        public string BillGroup { get; set; } = "Арматура класса";
        public string BillMark { get { return Class; } }
        public string BillGOST { get { return Gost.Number; } }
        public string BillName { get { return Symbols.Diam + Diameter; } }
        public double Amount { get; set; }

        /// <summary>
        /// Дефолтный конструктор по диаметру, остальные дефолтные значения
        /// </summary>
        /// <param name="diameter"></param>
        public Armature(int diameter, int len)
        {
            Length = RoundHelper.Round5(len);
            Diameter = diameter;
            defineBaseParams();            
        }

        /// <summary>
        /// Длина нахлеста арматуры
        /// </summary>        
        public static int GetLapLength (int diam, Concrete concrete)
        {
            int factor = 0;
            switch (concrete.ClassB)
            {
                case Concrete.ClassB25:
                    factor = 50;
                    break;
                case Concrete.ClassB30:
                    factor = 46;
                    break;
            }
            return factor * diam;
        }

        private void defineBaseParams()
        {
            switch (Diameter)
            {
                case 6:
                    WeightUnit = 0.222;
                    Area = 0.283;
                    break;
                case 8:
                    WeightUnit = 0.395;
                    Area = 0.503;
                    break;
                case 10:
                    WeightUnit = 0.617;
                    Area = 0.785;
                    break;
                case 12:
                    WeightUnit = 0.888;
                    Area = 1.131;
                    break;
                case 14:
                    WeightUnit = 1.210;
                    Area = 1.540;
                    break;
                case 16:
                    WeightUnit = 1.580;
                    Area = 2.010;
                    break;
                case 18:
                    WeightUnit = 2.000;
                    Area = 2.540;
                    break;
                case 20:
                    WeightUnit = 2.470;
                    Area = 3.140;
                    break;
                case 22:
                    WeightUnit = 2.980;
                    Area = 3.800;
                    break;
                case 25:
                    WeightUnit = 3.850;
                    Area = 4.910;
                    break;
                case 28:
                    WeightUnit = 4.830;
                    Area = 6.160;
                    break;
                case 32:
                    WeightUnit = 6.310;
                    Area = 8.040;
                    break;
                case 36:
                    WeightUnit = 7.990;
                    Area = 10.180;
                    break;
                case 40:
                    WeightUnit = 9.870;
                    Area = 12.570;
                    break;
                case 45:
                    WeightUnit = 12.480;
                    Area = 15.000;
                    break;
                case 50:
                    WeightUnit = 15.410;
                    Area = 19.630;
                    break;
                case 55:
                    WeightUnit = 18.650;
                    Area = 23.760;
                    break;
                case 60:
                    WeightUnit = 22.190;
                    Area = 28.270;
                    break;
                case 70:
                    WeightUnit = 30.210;
                    Area = 38.480;
                    break;
                case 80:
                    WeightUnit = 39.460;
                    Area = 50.270;
                    break;
                default:
                    break;
            }
        }        
    }
}