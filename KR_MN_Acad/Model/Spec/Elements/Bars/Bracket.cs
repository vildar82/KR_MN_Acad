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
    /// Скоба
    /// </summary>
    public class Bracket : BarDetail, IDetail
    {
        private const string blockNameDetail = "КР_Деталь_Ск1";
        private const string PREFIX = "Ск-";
        private const string friendlyName = "Скоба";

        public override int Index { get; set; } = 3;

        public override string BlockNameDetail { get; set; } = blockNameDetail;

        /// <summary>
        /// Ширина скобы
        /// </summary>
        public int T { get; set; }
        /// <summary>
        /// Длина вылета скобы (рабочая область)
        /// </summary>
        public int L { get; set; }        

        /// <summary>
        /// Создание скобы по распределению
        /// </summary>
        /// <param name="d">Диаметр скобы</param>        
        /// <param name="h">Длина нахлеста скобы - вылет (от внутренней грани стержня)</param>
        /// <param name="t">Ширина скобы (по внутренней грани стержня)</param>        
        /// <param name="step">Шаг скобы</param>
        /// <param name="width">Ширина распределения</param>
        /// <param name="pos">Позиция (из атрибута блока)</param>
        /// <param name="block">Блок</param>
        public Bracket (int d, int h, int t, int step, int width, int rows, string pos, ISpecBlock block) 
            : base(d, CalcLength(h, t, d), width, step, rows, PREFIX, pos, block, friendlyName)
        {            
            T = RoundHelper.Round5(t);
            L = h;            
        }
        
        /// <summary>
        /// Опрределение длины скобы
        /// </summary>
        /// <param name="h">Длина нахлеста скобы - вылет (от внутренней грани стержня)</param>
        /// <param name="t">Ширина скобы (по внутренней грани стержня)</param>
        /// <param name="d">Диаметр скобы</param>        
        private static int CalcLength(int h, int t, int d)
        {
            return RoundHelper.RoundWhole(2 * h + RoundHelper.Round5(t) + 0.58 * d);            
        }

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>        
        public override void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", Mark, atrs);
            SetDetailParameter("ВЫСОТА", T.ToString(), atrs);
            SetDetailParameter("ДЛИНА", L.ToString(), atrs);
        }

        public override bool Equals (IDetail other)
        {
            var b = other as Bracket;
            if (b == null) return false;
            var res = Mark == b.Mark && L == b.L && T == b.T;
            return res;
        }

        public override int CompareTo (IDetail other)
        {
            var b = other as Bracket;
            if (b == null) return -1;
            var res = AcadLib.Comparers.AlphanumComparator.New.Compare(Mark,b.Mark);
            if (res != 0) return res;

            res = L.CompareTo(b.L);
            if (res != 0) return res;

            res = T.CompareTo(b.T);
            return res;
        }
    }
}
