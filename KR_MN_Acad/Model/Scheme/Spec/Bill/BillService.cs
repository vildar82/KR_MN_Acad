using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Расчет расхода стали
    /// </summary>
    public class BillService
    {
        public BillRow Row { get; set; }
        public SchemeService Service { get; set; }

        public BillService(SchemeService service)
        {
            Service = service;            
        }

        public Table CreateTable()
        {
            Calc();
            BillTable billSpec = new BillTable(this);
            return billSpec.CreateTable();
        }

        /// <summary>
        /// Расчет ведомости расхода стали по строкам спецификации (service.Groups)
        /// </summary>
        private void Calc()
        {
            // строка врс
            Row = new BillRow(this);
            Row.Calc();
        }       
    }
}
