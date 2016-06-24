using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Общий табличный сервис
    /// </summary>
    public abstract class TableService : ITableService
    {
        public abstract Table CreateTable (List<ISpecRow> rows);
        public abstract List<ISpecRow> GetSpecRows (List<ISpecElement> elements);
        public abstract void Numbering (List<ISpecElement> elements);
    }
}
