using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Гнутый стержень
    /// </summary>
    public class BentBar : BarDetail, IDetail
    {
        public string BlockNameDetail { get { return "КР_Деталь_Гс1"; } }
        /// <summary>
        /// Длина 
        /// </summary>
        public int L { get; set; }
        /// <summary>
        /// Высота
        /// </summary>
        public int H { get; set; }

        /// <summary>
        /// Создание гнутого стержня
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="l">Длина Гс</param>
        /// <param name="h">Высота Гс</param>        
        /// <param name="count">Кол</param>
        /// <param name="pos">Значение атрибута позиции из блока</param>
        /// <param name="block">Блок</param>
        public BentBar (int diam, int l, int h, int count , string pos, ISchemeBlock block)
            : base(diam, getLength(l, h, diam), count, "Гс-", pos, block, "Гнутый стержень")
        {
            H = h;
            L = l;            
        }

        private static int getLength (int l, int h, int diam)
        {
            return l + h;
        }        

        public void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ВЫСОТА", H.ToString(), atrs);
            SetDetailParameter("ДЛИНА", L.ToString(), atrs);
        }
    }
}
