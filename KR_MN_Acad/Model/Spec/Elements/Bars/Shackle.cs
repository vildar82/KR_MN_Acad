using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Spec.Elements.Bars
{
    /// <summary>
    /// Хомут
    /// </summary>
    public class Shackle : BarDetail, IDetail
    {
        private const string blockNameDetail = "КР_Деталь_Х1";
        private const string PREFIX = "Х-";
        private const string friendlyName = "Хомут";

        public override string BlockNameDetail { get; set; } = blockNameDetail;

        /// <summary>
        /// Хвостик
        /// </summary>
        private int tail; // при диам 10-12 = 100        
        /// <summary>
        /// Длина хомута
        /// </summary>
        public int L { get; set; }
        /// <summary>
        /// Высота хомута
        /// </summary>
        public int H { get; set; }        

        /// <summary>
        /// Хомут
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="wShackle">Ширина хомута (по внутр гряням)</param>
        /// <param name="hShackle">Высота хомута</param>
        /// <param name="step">Шаг</param>
        /// <param name="range">Ширина распределения</param>
        /// <param name="pos">Значение атр позиции из блока</param>
        /// <param name="block">Блок</param>
        public Shackle(int diam, int wShackle, int hShackle, int step, int range, int rows, string pos, ISpecBlock block) 
            : base(diam, GetLenShackle(wShackle, hShackle, diam),range, step, rows, PREFIX, pos, block, friendlyName)
        {            
            tail = getTail(diam);
            L = RoundHelper.Round5(wShackle);
            H = RoundHelper.Round5(hShackle);
            Class = ClassA240C;
            Gost = GostOld;            
        }

        private static int getTail (int diam)
        {
            return diam >= 10 ? 100 : 75;
        }

        /// <summary>
        /// Длина хомута - периметр + 75*2
        /// </summary>        
        private static int GetLenShackle(int width, int height, int diam)
        {
            return RoundHelper.Round5(width) * 2 + RoundHelper.Round5(height) * 2 + getTail(diam) * 2;
        }

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>     
        public override void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", Mark, atrs);
            SetDetailParameter("ВЫСОТА", H.ToString(), atrs);
            SetDetailParameter("ШИРИНА", L.ToString(), atrs);
            SetDetailParameter("ХВОСТ1", tail.ToString(), atrs);
            SetDetailParameter("ХВОСТ2", tail.ToString(), atrs);
        }

        public override bool Equals (IDetail other)
        {
            var s = other as Shackle;
            if (s == null) return false;
            return L == s.L && H == s.H && tail == s.tail;
        }

        public override int CompareTo (IDetail other)
        {
            var s = other as Shackle;
            if (s == null) return -1;
            var res = L.CompareTo(s.L);
            if (res != 0) return res;
            res = H.CompareTo(s.H);
            if (res != 0) return res;
            return tail.CompareTo(s.tail);
        }
    }
}
