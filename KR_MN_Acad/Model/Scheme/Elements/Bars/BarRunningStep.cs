using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    public class BarRunningStep : BarRunning
    {        
        /// <summary>
        /// Шаг стержней
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }

        public BarRunningStep(int diam, double length, int width, int step, string pos, 
            ISchemeBlock block, string friendlyName)
            : base(diam, pos, block, friendlyName)
        {
            Width = width;
            Step = step;
            Meters = CalcMeters(length, width, step);            
        }
                
        private double CalcMeters(double length, int width, int step)
        {
            return RoundHelper.RoundSpec(ConvertMmToMLength((width / (step + 1)) * length ));
        }

        public override string GetDesc()
        {
            return base.GetDesc() + ", шаг " + Step; 
        }
    }
}
