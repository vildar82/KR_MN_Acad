using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    public class ArmatureRunningStep
    {
        /// <summary>
        /// Погонная арматура
        /// </summary>
        public ArmatureRunning ArmatureRun { get; set; }
        /// <summary>
        /// Шаг стержней
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }

        public ArmatureRunningStep(int diam, int length, int width, int step)
        {
            Width = width;
            Step = step;
            var meters = CalcMeters(length, width, step);
            ArmatureRun = new ArmatureRunning(diam, meters);
        }

        /// <summary>
        /// Расчет м.п.
        /// </summary>
        public double CalcMeters(int length, int width, int step)
        {
            return RoundHelper.RoundSpec(ConvertMmToMLength((width / (step + 1)) * length));
        }
    }
}
