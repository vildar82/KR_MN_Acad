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

        public BarDivision (int diam, int length, int width, int step, string pos, ISchemeBlock block, string friendlyName) 
            : base(diam, length, pos, block, friendlyName)
        {
            Width = width;
            Step = step;                        
        }        

        public override void Calc()
        {            
            // Кол стержней
            Count = Width / Step + 1;
            base.Calc();
        }

        public override string GetDesc()
        {
            return base.GetDesc() + ", шаг " + Step + ", шт. " + Count;
        }
    }
}