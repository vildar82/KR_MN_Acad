using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
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
        public override GroupType Group { get; set; }= GroupType.Details;

        public override bool IsDefaultGroupings { get; set; } = false;

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

        public override void SetNumber (string num, int indexFirst, int indexSecond)
        {
            int index = indexSecond == 0 ? indexFirst : indexSecond;
            Mark = prefix + index;
        }

        public override int CompareTo (ISpecElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return -1;
            var res = prefix.CompareTo(det.prefix);
            if (res != 0) return res;

            res = base.CompareTo(other);
            if (res != 0) return res;

            res = BlockNameDetail.CompareTo(det.BlockNameDetail);
            if (res != 0) return res;            

            res = this.CompareTo(det);

            return res;
        }

        public override bool Equals(ISpecElement other)
        {
            var det = other as BarDetail;
            if (det == null) return false;
            if (ReferenceEquals(this, det)) return true;

            return prefix == det.prefix &&
                BlockNameDetail == det.BlockNameDetail &&
                base.Equals(other) &&
                this.Equals(det);           
        }

        public override int GetHashCode ()
        {
            var res = BlockNameDetail.GetHashCode();
            return res;
        }

        //public override string GetDesc ()
        //{
        //    //return $"{Mark}, {Symbols.Diam}{Diameter}";
        //    return base.GetDesc() + $", шт.{Count}";
        //}

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
            if (atrPos != null)
            {
                var atrRef=  atrPos.IdAtr.GetObject(OpenMode.ForWrite) as AttributeReference;
                atrRef.TextString = value;
            }
            else
            {
                Inspector.AddError($"В блоке детали {BlockNameDetail} не определен парметр {paramName}.");
            }
        }

        public abstract bool Equals (IDetail other);

        public abstract int CompareTo (IDetail other);

        public override Dictionary<string, List<ISpecElement>> GroupsFirst (IGrouping<GroupType, ISpecElement> indexTypeGroup)
        {
            var bars = indexTypeGroup.Cast<BarDetail>().GroupBy(g=>g.prefix).OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New);
            return bars.ToDictionary(k => k.Key, i => i.Cast<ISpecElement>().ToList());
        }
        public override Dictionary<string, List<ISpecElement>> GroupsSecond (List<ISpecElement> value)
        {
            var bars = value.GroupBy(g=>g).OrderBy(o=>o.Key);
            return bars.ToDictionary(k => k.Key.Key, i => i.Cast<ISpecElement>().ToList());
        }
    }
}
