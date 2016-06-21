using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Хомут
    /// </summary>
    public class Shackle : BarDetail, IDetail
    {
        /// <summary>
        /// Хвостик
        /// </summary>
        private int tail; // при диам 10-12 = 100

        /// <summary>
        /// Шаг
        /// </summary>
        public int Step { get; set; }        
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Длина хомута
        /// </summary>
        public int L { get; set; }
        /// <summary>
        /// Высота хомута
        /// </summary>
        public int H { get; set; }

        public string BlockNameDetail { get { return "КР_Деталь_Х1"; } }

        public Shackle(int diam, int width, int height, int step, int range, string pos, ISchemeBlock block) 
            : base(diam, GetLenShackle(width, height, diam), 1, "Х-", pos, block, "Хомут")
        {
            tail = getTail(diam);
            L = width;
            H = height;
            Class = ClassA240C;
            Gost = GostOld;
            Step = step;            
            Width = range;            
            Count = CalcCount();
        }

        private static int getTail (int diam)
        {
            return diam >= 10 ? 100 : 75;
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
        /// Длина хомута - периметр + 75*2
        /// </summary>        
        private static int GetLenShackle(int width, int height, int diam)
        {
            return width * 2 + height * 2 + getTail(diam) * 2;
        }

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>     
        public void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ВЫСОТА", H.ToString(), atrs);
            SetDetailParameter("ШИРИНА", L.ToString(), atrs);
            SetDetailParameter("ХВОСТ1", tail.ToString(), atrs);
            SetDetailParameter("ХВОСТ2", tail.ToString(), atrs);
        }
    }
}
