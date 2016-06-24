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
    /// Гнутый стержень - Г-образный
    /// </summary>
    public class BentBarLshaped : BarDetail, IDetail
    {
        private const string blockNameDetail = "КР_Деталь_Гс1";
        private const string prefix = "Гс-";
        private const string friendlyName = "Гнутый стержень";

        private string descEnd;        
        /// <summary>
        /// Длина 
        /// </summary>
        public int L { get; set; }
        /// <summary>
        /// Высота
        /// </summary>
        public int H { get; set; }

        /// <summary>
        /// Создание гнутого стержня - по шагу
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="lGs">Длина Гс</param>
        /// <param name="hGs">Высота Гс</param>        
        /// <param name="count">Кол</param>
        /// <param name="pos">Значение атрибута позиции из блока</param>
        /// <param name="block">Блок</param>
        public BentBarLshaped (int diam, int lGs, int hGs, int count, string pos, ISchemeBlock block)
            : base(diam, getLength(lGs, hGs, diam), count, prefix, pos, block, friendlyName)
        {
            BlockNameDetail = blockNameDetail;
            H = hGs;
            L = lGs;
            descEnd = ", шт." + count;
        }

        /// <summary>
        /// Гнутый стержень распределенный по ширина с шагом
        /// </summary>        
        public BentBarLshaped (int diam, int lGs, int hGs, int width, int step, int rows, string pos, ISchemeBlock block)
            : base(diam, getLength(lGs, hGs, diam), width, step, rows,prefix, pos, block, friendlyName )
        {
            BlockNameDetail = blockNameDetail;
            H = hGs;
            L = lGs;
            descEnd = ", ш." + step;
        }

        /// <summary>
        /// Определение длины гнутого стержня.
        /// Округление до 1.
        /// </summary>
        /// <param name="l">Длина загиба</param>
        /// <param name="h">Высота загиба</param>
        /// <param name="diam">Диаметр</param>        
        private static int getLength (int l, int h, int diam)
        {
            return RoundHelper.RoundWhole(l + h - 1.15*diam);
        }

        public override string GetDesc ()
        {
            return base.GetDesc() + descEnd;
        }

        public override void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ВЫСОТА", H.ToString(), atrs);
            SetDetailParameter("ДЛИНА", L.ToString(), atrs);
        }

        public override bool Equals (IDetail other)
        {
            var b = other as BentBarLshaped;
            if (b == null) return false;
            return L == b.L && H == b.H;
        }

        public override int CompareTo (IDetail other)
        {
            var b = other as BentBarLshaped;
            if (b == null) return -1;
            var res = L.CompareTo(b.L);
            if (res != 0) return res;
            return H.CompareTo(b.H);
        }
    }
}
