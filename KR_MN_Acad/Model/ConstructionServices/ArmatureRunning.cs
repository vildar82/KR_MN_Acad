using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// Арматура измеряемая поганажом
    /// </summary>
    public class ArmatureRunning : Armature
    {
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Шаг стержней
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Количество погонных метров
        /// </summary>
        public double Meters { get; set; }
        /// <summary>
        /// Масса общая
        /// </summary>
        public double WeightRunning { get; set; }

        public ArmatureRunning(int diam, int length, int width, int step) : base(diam, length)
        {
            Step = step;
            Width = width;
            CalcMeters();
            CalcWeight();
            PrepareConstTable();
        }

        /// <summary>
        /// Расчет м.п.
        /// </summary>
        private void CalcMeters()
        {
            Meters = RoundHelper.RoundSpec( ConvertMmToMLength((Width / (Step + 1)) * Length));
        }

        public override void CalcWeight()
        {
            // Масса ед
            Weight = RoundHelper.RoundSpec(WeightUnit);
            // Масса общая
            WeightRunning = RoundHelper.RoundSpec(Weight * Meters);
        }

        public override void PrepareConstTable()
        {            
            base.PrepareConstTable();
            NameColumn = $"∅{Diameter} {Class}, L=п.м.";                        
        }

        public override void PrepareFinalTable()
        {
            CountColumn = Meters.ToString();
            DescriptionColumn = RoundHelper.RoundSpec(WeightRunning * Meters).ToString();
        }
    }
}
