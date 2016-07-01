using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Bill
{
    /// <summary>
    /// Ячейка материала в ВРС
    /// </summary>
    public class BillCell : IComparable<BillCell>, IEquatable<BillCell>
    {
        private string concatMaterial;
        private static AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;
        private List<IBillMaterial> cellMaterials;

        /// <summary>
        /// Материал одной ячейки
        /// </summary>
        public IBillMaterial BillMaterial { get; set; }                
        public double Amount { get; set; }        

        public BillCell(List<IBillMaterial> cellMaterials)
        {
            this.cellMaterials = cellMaterials;
            BillMaterial = cellMaterials.First();                        
            concatMaterial = BillMaterial.BillTitle + BillMaterial.BillGroup + BillMaterial.BillMark + BillMaterial.BillName;

            Amount = cellMaterials.Sum(s => s.Amount);
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
