using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Строка ведомости расхода стали
    /// </summary>
    public class BillRow
    {        
        /// <summary>
        /// Название - например: Секция1, Плита.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Материалы
        /// </summary>
        public List<BillCell> Cells { get; set; }

        BillService bilService;

        public BillRow(BillService bilService)
        {            
            Name = "Имя";
            this.bilService = bilService;
        }

        public void Calc()
        {
            Dictionary<BillCell, BillCell> dictCells = new Dictionary<BillCell, BillCell>();
            // все материалы IBillMaterial
            var specRows = bilService.Service.Groups.SelectMany(s => s.Rows).Where(t => t.SomeElement is IBillMaterial);
            foreach (var row in specRows)
            {
                BillCell cel = new BillCell(row);
                BillCell existCel;
                if (dictCells.TryGetValue(cel, out existCel))
                    existCel.Add(row);
                else
                    dictCells.Add(cel, cel);
            }
            // Добавление итоговых ячеек в кажд
            Cells = dictCells.Values.ToList();
            Cells.Sort();            
        }
    }
}
