using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Скоба
    /// </summary>
    public class Bracket : BarDetail
    {
        /// <summary>
        /// Шаг
        /// </summary>
        public int Step { get; set; }        
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }        
        
        public Bracket(int diam, int len, int step, int width, string pos, ISchemeBlock block) 
            : base(diam, len, 1, "Ск-", pos, block, "Скоба")
        {            
            Step = step;            
            Width = width;            
            Count = CalcCount();
        }

        /// <summary>
        /// Определение кол шпилек
        /// </summary>
        /// <returns></returns>
        private int CalcCount()
        {
            return BarDivision.CalcCountByStep(Width, Step);                        
        }

        public override string GetDesc()
        {
            return base.GetDesc() + $", ш.{Step}";
        }
        
        /// <summary>
        /// Опрределение длины скобы
        /// </summary>
        /// <param name="h">Длина нахлеста скобы - вылет (от внутренней грани стержня)</param>
        /// <param name="t">Ширина скобы (по внутренней грани стержня)</param>
        /// <param name="d">Диаметр скобы</param>        
        public static int CalcLength(int h, int t, int d)
        {
            return RoundHelper.RoundWhole(2 * h + t + 0.58 * d);            
        }
    }
}
