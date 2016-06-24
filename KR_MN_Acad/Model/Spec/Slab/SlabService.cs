using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Slab
{
    public class SlabService : TableService
    {
        public override Table CreateTable (List<ISpecRow> rows)
        {
            throw new NotImplementedException();
        }

        public override List<ISpecRow> GetSpecRows (List<ISpecElement> elements)
        {
            throw new NotImplementedException();
        }

        public override void Numbering (List<ISpecElement> elements)
        {
            throw new NotImplementedException();
        }
    }
}
