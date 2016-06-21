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
        /// <summary>
        /// Кол. рядов арматуры
        /// </summary>
        public int Rows { get; set; } = 1;

        /// <summary>
        /// Погонные стержни с шагом и диапазоном распределения
        /// </summary>
        /// <param name="diam">Диам</param>
        /// <param name="length">Длина стержня</param>
        /// <param name="widthRun">Щирина распределения</param>
        /// <param name="step">Шаг распределенич</param>
        /// <param name="rows">Рядов стержней</param>
        /// <param name="pos">Позиция (из атр блока)</param>
        /// <param name="block">Блок</param>
        /// <param name="friendlyName"></param>
        public BarRunningStep(int diam, double length, int widthRun, int step, int rows, string pos, 
            ISchemeBlock block, string friendlyName)
            : base(diam, pos, block, friendlyName)
        {
            Rows = rows;
            Width = widthRun;
            Step = step;            
            Meters = CalcMeters(length);            
        }
                
        private double CalcMeters(double length)
        {
            // Кол стержней в распределении
            int count = BarDivision.CalcCountByStep(Width, Step)*Rows;
            return RoundHelper.Round2(ConvertMmToMLength(length)* count);
        }

        public override string GetDesc()
        {
            return base.GetDesc() + ", ш." + Step; 
        }
    }
}
