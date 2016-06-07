using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Хомут
    /// </summary>
    public class Shackle : BarDetail
    {
        /// <summary>
        /// Шаг
        /// </summary>
        public int Step { get; set; }        
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }        

        public Shackle(int diam, int len, int step, int width, string pos, ISchemeBlock block) 
            : base(diam, len, 1, "Х-", pos, block, "Хомут")
        {
            Class = ClassA240C;
            Gost = GostOld;
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
        /// Длина хомута - периметр + 75*2
        /// </summary>        
        public static int GetLenShackle(int len, int width)
        {
            return len * 2 + width * 2 + 75 * 2;
        }
    }
}
