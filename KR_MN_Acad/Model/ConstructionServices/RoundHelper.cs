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
        public static double Round2(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// Округление массы ед. кг. до 3 знаков, с округлением вверх
        /// </summary>        
        public static double Round3(double value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Округление вверх до целого
        /// </summary>        
        public static int RoundWhole(double value)
        {
            return (int)Math.Ceiling(Math.Round(value,1));
        }
    }
}
