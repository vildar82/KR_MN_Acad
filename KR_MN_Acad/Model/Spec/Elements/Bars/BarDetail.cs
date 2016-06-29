using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using static AcadLib.General;

namespace KR_MN_Acad.Spec.Elements.Bars
{
    /// <summary>
    /// Арматурная деталь - шпилька, скашка, пешка и т.п.
    /// </summary>
    public abstract class BarDetail : Bar, IDetail
    {
        public abstract string BlockNameDetail { get; set; }
        public override int Index { get; set; } = 0;
        public override string Group { get; set; }= GroupType.Details.Name;

        /// <summary>
        /// Деталь по штукам
        /// </summary>        
        public BarDetail(int diam, int len, int count, string prefix, string pos,
            ISpecBlock block, string friendlyName) 
            : base(diam, len,count, pos, block, friendlyName)
        {
            this.prefix = prefix;
        }

        /// <summary>
        /// Деталь по распределению
        /// </summary>        
        public BarDetail (int diam, int len, int width, int step, int rows, string prefix, string pos,
            ISpecBlock block, string friendlyName)
            : base(diam, len, width, step, rows, pos, block, friendlyName)
        {
            this.prefix = prefix;
        }        

        public override int CompareTo (ISpecElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return -1;
            var res = prefix.CompareTo(det.prefix);
            if (res != 0) return res;

            res = BlockNameDetail.CompareTo(det.BlockNameDetail);
            if (res != 0) return res;

            res = base.CompareTo(other);
            if (res != 0) return res;

            res = this.CompareTo(det);

            return res;
        }

        public override bool Equals(ISpecElement other)
        {
            var det = other as BarDetail;
            if (det == null) return false;

            return prefix == det.prefix &&
                BlockNameDetail == det.BlockNameDetail &&
                base.Equals(other) &&
                this.Equals(det);           
        }

        public override int GetHashCode ()
        {
            return BlockNameDetail.GetHashCode();
        }

        public override string GetDesc()
        {
            return $"{Mark}, {Symbols.Diam}{Diameter}";
        }

        public abstract void SetDetailsParam (List<AttributeInfo> atrs);        

        /// <summary>
        /// Установка значения в атрибут блока детали
        /// </summary>
        /// <param name="paramName">Тег атрибута</param>
        /// <param name="value">Значение</param>
        /// <param name="atrs">Список всех атрибутов</param>
        public void SetDetailParameter(string paramName, string value, List<AttributeInfo> atrs)
        {
            var atrPos = atrs.FirstOrDefault(a => a.Tag.Equals(paramName, StringComparison.OrdinalIgnoreCase));
            var atrRef=  atrPos.IdAtr.GetObject(OpenMode.ForWrite) as AttributeReference;
            atrRef.TextString = value;
        }

        public abstract bool Equals (IDetail other);

        public abstract int CompareTo (IDetail other);        
    }
}
