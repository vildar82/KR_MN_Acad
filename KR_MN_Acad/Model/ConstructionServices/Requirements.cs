using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// конструктивные требования и условия
    /// </summary>
    public static class Requirements
    {
        /// <summary>
        /// Нужно ли гнуть вертикальный стержень
        /// </summary>        
        public static bool IsNeedToBentVerticArm (int diam)
        {
            return diam >= 20;
        }    
    }
}
