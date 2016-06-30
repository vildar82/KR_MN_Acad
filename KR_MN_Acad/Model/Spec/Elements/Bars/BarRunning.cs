using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using static AcadLib.Units.UnitsConvertHelper;
using static AcadLib.General;
using KR_MN_Acad.Spec.SpecGroup;
using KR_MN_Acad.Spec.Materials;

namespace KR_MN_Acad.Spec.Elements.Bars
{
    /// <summary>
    /// Арматура измеряемая поганажом
    /// </summary>
    public class BarRunning : Bar
    {
        private static Dictionary<int, Dictionary<string , double>> KLapDict;
        /// <summary>
        /// Количество погонных метров
        /// </summary>
        public double Meters { get; set; }
        public override int Index { get; set; } = indexBarRunning;

        /// <summary>
        /// Погонные стержни по длине (м.)
        /// </summary>        
        public BarRunning(int diam, double meters, string pos, ISpecBlock block, string friendlyName) 
            : base(diam, 0, 1, pos, block, friendlyName)
        {
            Meters = RoundHelper.Round2Digits(meters);            
        }

        /// <summary>
        /// Погонные стержни с шагом и диапазоном распределения
        /// </summary>        
        public BarRunning (int diam, double length, int widthRun, int step, int rows, string pos,
            ISpecBlock block, string friendlyName)
            : base(diam,0,1, pos, block, friendlyName)
        {
            Rows = rows;
            Width = widthRun;
            Step = step;
            Meters = CalcMeters(length);            
        }        

        /// <summary>
        /// Перед вызовом
        /// </summary>
        public override void Calc()
        {
            // Масса ед
            Weight = RoundHelper.Round3Digits(WeightUnit);
            // Масса общая
            WeightTotal = RoundHelper.Round2Digits(Weight * Meters);
        } 

        public override string GetDesc()
        {
            // 3, ⌀12
            string desc = $"{Mark}, {Symbols.Diam}{Diameter}, L={Count}м.п.";
            if (Step != 0)
                desc += ", ш." + Step; ;
            return desc;
        }        

        public override string GetName()
        {
            return Name + " L=п.м.";
        }

        public override void SumAndSetRow (SpecGroupRow specGroupRow, List<ISpecElement> elems)
        {
            // Обозначения, Наименования, Кол, Массы ед, примечания
            specGroupRow.Description = Gost.Number;
            specGroupRow.Name = GetName();

            double metersTotal = 0;
            double weightTotal = 0;
            foreach (var item in elems)
            {
                var bar = item as BarRunning;
                metersTotal += bar.Meters;
                //weightTotal += bar.WeightTotal;
            }
            metersTotal = RoundHelper.RoundWhole(metersTotal);
            weightTotal = RoundHelper.Round2Digits( metersTotal * Weight);

            specGroupRow.Count = metersTotal.ToString();
            specGroupRow.Weight = Weight.ToString("N3");
            specGroupRow.Description = weightTotal.ToString();            
        }

        /// <summary>
        /// Определение коэфициента нахлеста для погонной арматуры
        /// </summary>
        /// <param name="classB">Класс бетона</param>
        /// <returns></returns>
        public static double GetKLap(int diam, Concrete concrete)
        {
            if (KLapDict == null) KLapDict = LoadKLap();
            double res = 1;
            Dictionary<string, double> dict;
            if (KLapDict.TryGetValue(diam, out dict))
            {
                dict.TryGetValue(concrete.ClassB, out res);
            }
            return res;
        }

        private static Dictionary<int, Dictionary<string, double>> LoadKLap()
        {
            return new Dictionary<int, Dictionary<string, double>>()
            {
                { 6, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.025 },
                        { Concrete.ClassB30, 1.023 }} },
                { 8, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.034 },
                        { Concrete.ClassB30, 1.031 }} },
                { 10, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.042 },
                        { Concrete.ClassB30, 1.039 }} },
                { 12, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.051 },
                        { Concrete.ClassB30, 1.047 }} },
                { 14, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.059 },
                        { Concrete.ClassB30, 1.054 }} },
                { 16, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.068 },
                        { Concrete.ClassB30, 1.062 }} },
                { 18, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.076 },
                        { Concrete.ClassB30, 1.070 }} },
                { 20, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.085 },
                        { Concrete.ClassB30, 1.078 }} },
                { 22, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.093 },
                        { Concrete.ClassB30, 1.085 }} },
                { 25, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.106 },
                        { Concrete.ClassB30, 1.097 }} },
                { 28, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.119 },
                        { Concrete.ClassB30, 1.109 }} },
                { 32, new Dictionary<string, double>() {
                        { Concrete.ClassB25, 1.136 },
                        { Concrete.ClassB30, 1.124 }} },
            };
        }
        private double CalcMeters (double length)
        {
            // Кол стержней в распределении
            Count = CalcCountByStep(Width, Step) * Rows;
            return RoundHelper.Round2Digits(ConvertMmToMLength(length) * Count);
        }
    }
}
