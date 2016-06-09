using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Шпилька
    /// </summary>
    public class Spring : BarDetail
    {
        /// <summary>
        /// Шаг шпилек по гориз
        /// </summary>
        public int StepHor { get; set; }
        /// <summary>
        /// Шаг шпилек по вертик
        /// </summary>
        public int StepVertic { get; set; }
        /// <summary>
        /// Ширина распределения шпилек по горизонтали
        /// </summary>
        public int WidthHor { get; set; }
        /// <summary>
        /// Ширина распределения шпилек по вертикали
        /// </summary>
        public int WidthVertic { get; set; }

        public Spring(int diam, int len, int stepHor, int stepVert, int widthHor,int widthVertic, string pos, ISchemeBlock block) 
            : base(diam, len, 1, "Ш-", pos, block, "Шпилька")
        {
            Class = ClassA240C;
            Gost = GostOld;
            StepHor = stepHor;
            StepVertic = stepVert;
            WidthHor = widthHor;
            WidthVertic = widthVertic;
            Count = CalcCount();
        }

        /// <summary>
        /// Определение кол шпилек
        /// </summary>
        /// <returns></returns>
        private int CalcCount()
        {
            int countVert = (int)Math.Ceiling((double)WidthVertic / StepVertic);
            int countHor = (int)Math.Ceiling((double)WidthHor / StepHor);
            return countVert * countHor;
        }

        public override string GetDesc()
        {
            return base.GetDesc() + $", ш.{StepHor}х{StepVertic}";
        }
    }
}
