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
    /// Гнутый стержень - по вертикали
    /// </summary>
    public class BentBarDirect : BarDetail, IDetail
    {
        private const string blockNameDetail = "КР_Деталь_Гс2";
        private const string PREFIX = "Гс-";
        private const string friendlyName = "Гнутый стержень";

        public override int Index { get; set; } = 1;

        public override string BlockNameDetail { get; set; } = blockNameDetail;
        /// <summary>
        /// Длина 1
        /// </summary>
        public int LStart { get; set; }
        public int LEnd { get; set; }
        /// <summary>
        /// Длина перепада
        /// </summary>
        public int LDif { get; set; }
        /// <summary>
        /// Высота перепада
        /// </summary>
        public int HDif { get; set; }        

        /// <summary>
        /// Создание гнутого стержня по штукам
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="lStart">Длина 1 Гс</param>
        /// <param name="lEnd">Длина 2 Гс</param>
        /// <param name="lDif">Длина перепада</param>
        /// <param name="hDif">Высота перепада</param>        
        /// <param name="count">Кол</param>
        /// <param name="pos">Значение атрибута позиции из блока</param>
        /// <param name="block">Блок</param>
        public BentBarDirect (int diam, int lStart, int lEnd, int lDif, int hDif, int count, string pos, ISpecBlock block)
            : base(diam, getLength(lStart,lEnd, lDif, hDif, diam), count, PREFIX, pos, block, friendlyName)
        {            
            LStart = lStart;
            LEnd = lEnd;
            HDif = hDif;
            LDif = lDif;            
        }
        
        private static int getLength (int lStart, int lEnd, int lDif, int hDif, int diam)
        {
            var dif = Math.Sqrt(lDif * lDif + hDif * hDif);
            return RoundHelper.RoundWhole(lStart+lEnd + dif);
        }

        //public override string GetDesc ()
        //{
        //    return base.GetDesc() + $", шт.{Count}";
        //}

        public override void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", Mark, atrs);
            SetDetailParameter("ВЫСОТА", HDif.ToString(), atrs);
            SetDetailParameter("ДЛИНА1", LStart.ToString(), atrs);
            SetDetailParameter("ДЛИНА2", LDif.ToString(), atrs);
            SetDetailParameter("ДЛИНА3", LEnd.ToString(), atrs);
        }

        public override bool Equals (IDetail other)
        {
            var b = other as BentBarDirect;
            if (b == null) return false;
            var res = Mark == b.Mark && LStart == b.LStart && LEnd == b.LEnd && HDif == b.HDif && LDif == b.LDif;
            return res;
        }

        public override int CompareTo (IDetail other)
        {
            var b = other as BentBarDirect;
            if (b == null) return -1;
            var res = AcadLib.Comparers.AlphanumComparator.New.Compare(Mark,b.Mark);
            if (res != 0) return res;

            res = LStart.CompareTo(b.LStart);
            if (res != 0) return res;

            res = LEnd.CompareTo(b.LEnd);
            if (res != 0) return res;

            res = HDif.CompareTo(b.HDif);
            if (res != 0) return res;

            res = LDif.CompareTo(b.LDif);
            return res;
        }
    }
}
