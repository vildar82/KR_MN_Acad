using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Bill
{
    /// <summary>
    /// Строка ведомости расхода стали
    /// </summary>
    public class BillRow
    {
        private List<IBillMaterial> materials;

        /// <summary>
        /// Название - например: Секция1, Плита.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Материалы
        /// </summary>
        public List<BillCell> Cells { get; set; } = new List<BillCell>();             

        public BillRow(List<IBillMaterial> materials)
        {            
            Name = "Имя";
            this.materials = materials;
        }

        public void Calc()
        {
            var alpha = AcadLib.Comparers.AlphanumComparator.New;
            // группировка материалов по столбцам            
            var cellMaterGroups = materials.GroupBy(g=> new {
                title = g.BillTitle, titleIndex = g.BillTitleIndex, group = g.BillGroup,
                mark = g.BillMark, gost = g.Gost, name = g.BillName
            }).OrderBy(o=>o.Key.titleIndex).ThenBy(o=>o.Key.group).ThenBy(o=>o.Key.mark, alpha).ThenBy(o=>o.Key.name);

            foreach (var cell in cellMaterGroups)
            {
                BillCell cel = new BillCell(cell.ToList());
                Cells.Add(cel);
            }            
        }
    }
}
