using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Распределенные стержни на участке
    /// </summary>
    public class BarDivision : Bar
    {        
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Шаг распределения
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Кол. рядов арматуры
        /// </summary>
        public int Rows { get; set; } = 1;

        public BarDivision (int diam, int length, int width, int step, int rows, string pos, ISchemeBlock block, string friendlyName) 
            : base(diam, length, pos, block, friendlyName)
        {
            Rows = rows;
            Width = width;
            Step = step;                        
        }        

        public override void Calc()
        {
            // Кол стержней
            Count = CalcCountByStep(Width, Step) * Rows;
            base.Calc();
        }

        public override string GetDesc()
        {
            // 3, ⌀12, L=3050, ш.200
            return base.GetDesc() + ", ш" + Step;
        }

        /// <summary>
        /// Определение кол-ва стержней по ширине и шагу
        /// Округление вверх + 1
        /// </summary>
        /// <param name="width">Ширина распределения</param>
        /// <param name="step">Шаг</param>
        /// <returns>Кол стержней в распределении</returns>
        public static int CalcCountByStep (double width, double step)        
        {
            return (int)Math.Ceiling(width / step) + 1;                        
        }
    }
}