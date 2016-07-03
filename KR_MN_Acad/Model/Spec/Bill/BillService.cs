using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Bill
{
    /// <summary>
    /// Ведомость расхода стали - на одну конструкцию/схему
    /// </summary>
    public class BillService
    {
        public Database Db { get; }
        public List<ISpecElement> Elements { get; set; }
        public List<IBillMaterial> Materials { get; set; }
        public BillRow Row { get; set; }        

        public BillService(Database db, List<ISpecElement> elements)
        {
            Elements = elements;
            Materials = elements.OfType<IBillMaterial>().ToList();
            Db = db;                 
        }

        public Table CreateTable()
        {
            if (Materials.Count == 0) return null;
            Row = Calc();
            BillTable billSpec = new BillTable(this);
            return billSpec.CreateTable();
        }

        /// <summary>
        /// Расчет ведомости расхода стали по строкам спецификации (service.Groups)
        /// </summary>
        private BillRow Calc ()
        {
            // строка врс
            var row = new BillRow(Materials);
            row.Calc();
            return row;
        }       
    }
}
