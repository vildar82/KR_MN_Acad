using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Constructions
{
    public interface IConstructionBlock
    {
        List<ISpecElement> Elementary { get; }
        ISpecElement ConstructionElement { get; }
    }
}
