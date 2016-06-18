using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using static AcadLib.General;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Арматурная деталь - шпилька, скашка, пешка и т.п.
    /// </summary>
    public class BarDetail : Bar
    {
        public BarDetail(int diam, int len, int count, string prefix, string pos,
            ISchemeBlock block, string friendlyName) 
            : base(diam, len, pos, block, friendlyName)
        {
            Prefix = prefix;
            Type = Spec.GroupType.Details;
            Count = count;            
        }        

        public override int CompareTo(IElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return -1;
            var res = Prefix.CompareTo(other.Prefix);
            if (res != 0)
                return res;
            return base.CompareTo(other);
        }

        public override bool Equals(IElement other)
        {
            var det = other as BarDetail;
            if (det == null)
                return false;
            if (Prefix != other.Prefix)
                return false;
            return base.Equals(other);
        }

        public override string GetDesc()
        {
            return $"{SpecRow?.PositionColumn}, {Symbols.Diam}{Diameter}";
        }

        /// <summary>
        /// Установка значения в атрибут блока детали
        /// </summary>
        /// <param name="paramName">Тег атрибута</param>
        /// <param name="value">Значение</param>
        /// <param name="atrs">Список всех атрибутов</param>
        protected void SetDetailParameter(string paramName, string value, List<AttributeInfo> atrs)
        {
            var atrPos = atrs.FirstOrDefault(a => a.Tag.Equals(paramName, StringComparison.OrdinalIgnoreCase));
            var atrRef=  atrPos.IdAtr.GetObject(OpenMode.ForWrite) as AttributeReference;
            atrRef.TextString = value;
        }
    }
}
