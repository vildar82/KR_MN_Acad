using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.ArmTypes
{
    /// <summary>
    /// Тип армирования стены
    /// </summary>
    public class ArmTypeWall
    {        
        /// <summary>
        /// Тип армирования - номер 
        /// </summary>
        public int Type { get; set; }        
        public int ArmVerticDiam { get; set; }        
        public int ArmVerticStep { get; set; }
        public int ArmHorDiam { get; set; }
        public int ArmHorStep { get; set; }
        public int SpringDiam { get; set; }
        public int SpringStepHor { get; set; }
        public int SpringStepVertic { get; set; }
    }
}
