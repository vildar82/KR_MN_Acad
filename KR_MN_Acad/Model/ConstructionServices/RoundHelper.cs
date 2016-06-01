using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.ConstructionServices
{
    public static class RoundHelper
    {
        /// <summary>
        /// Округление десятичного значения для спецификации
        /// До двух знаков, с округлением вверх
        /// </summary>        
        public static double RoundSpec(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
