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

        public Spring(int diam, int len, int stepHor, int stepVert, int count, string pos, ISchemeBlock block) 
            : base(diam, len, stepHor, count, "Ш-", pos, block, "Шпилька")
        {
            Class = ClassA240C;
            Gost = GostOld;
            StepHor = stepHor;
            StepVertic = stepVert;
        }
    }
}
