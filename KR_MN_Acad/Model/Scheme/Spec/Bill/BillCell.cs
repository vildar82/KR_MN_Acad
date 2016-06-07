using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Ячейка материала в ВРС
    /// </summary>
    public class BillCell : IComparable<BillCell>, IEquatable<BillCell>
    {
        /// <summary>
        /// Материал одной ячейки
        /// </summary>
        public IBillMaterial BillMaterial { get; set; }
        public List<ISpecRow> SpecRows { get; set; }
        public BillRow BillRow { get; set; }
        public double Amount { get; set; }

        private string concatMaterial;
        private static AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;

        public BillCell(ISpecRow row)
        {            
            BillMaterial =(IBillMaterial) row.SomeElement;
            SpecRows = new List<ISpecRow>();
            Add(row);
            concatMaterial = BillMaterial.BillTitle + BillMaterial.BillGroup + BillMaterial.BillMark + BillMaterial.BillName;
        }

        public void Add(ISpecRow row)
        {
            SpecRows.Add(row);
            Amount += row.Amount;
        }

        public int CompareTo(BillCell other)
        {
            return alpha.Compare(concatMaterial, other.concatMaterial);
        }

        public bool Equals(BillCell other)
        {
            return concatMaterial.Equals(other.concatMaterial);
        }        

        public override int GetHashCode()
        {
            return concatMaterial.GetHashCode();
        }       
    }
}
