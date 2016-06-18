using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Скоба
    /// </summary>
    public class Bracket : BarDetail, IDetail
    {
        /// <summary>
        /// Шаг
        /// </summary>
        public int Step { get; set; }        
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Ширина скобы
        /// </summary>
        public int T { get; set; }
        /// <summary>
        /// Длина вылета скобы (рабочая область)
        /// </summary>
        public int L { get; set; }

        public string BlockNameDetail { get { return "КР_Деталь_Ск1"; } }

        /// <summary>
        /// Создание скобы
        /// </summary>
        /// <param name="d">Диаметр скобы</param>        
        /// <param name="h">Длина нахлеста скобы - вылет (от внутренней грани стержня)</param>
        /// <param name="t">Ширина скобы (по внутренней грани стержня)</param>        
        /// <param name="step">Шаг скобы</param>
        /// <param name="width">Ширина распределения</param>
        /// <param name="pos">Позиция (из атрибута блока)</param>
        /// <param name="block">Блок</param>
        public Bracket (int d, int h, int t, int step, int width, string pos, ISchemeBlock block) 
            : base(d, CalcLength(h, t, d), 1, "Ск-", pos, block, "Скоба")
        {
            T = t;
            L = h;
            Step = step;            
            Width = width;            
            Count = CalcCount();
        }

        /// <summary>
        /// Определение кол шпилек
        /// </summary>
        /// <returns></returns>
        private int CalcCount()
        {
            return BarDivision.CalcCountByStep(Width, Step);                        
        }

        public override string GetDesc()
        {
            return base.GetDesc() + $", ш.{Step}";
        }
        
        /// <summary>
        /// Опрределение длины скобы
        /// </summary>
        /// <param name="h">Длина нахлеста скобы - вылет (от внутренней грани стержня)</param>
        /// <param name="t">Ширина скобы (по внутренней грани стержня)</param>
        /// <param name="d">Диаметр скобы</param>        
        private static int CalcLength(int h, int t, int d)
        {
            return RoundHelper.RoundWhole(2 * h + t + 0.58 * d);            
        }

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>        
        public void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ВЫСОТА", T.ToString(), atrs);
            SetDetailParameter("ДЛИНА", L.ToString(), atrs);
        }
    }
}
