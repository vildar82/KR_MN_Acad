using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    public interface IMaterial
    {
        /// <summary>
        /// Наименование, например: ⌀12 А500С, Бетон B30, Труба ⌀426х5
        /// </summary>
        string Name { get; }
        /// <summary>
        /// ГОСТ
        /// </summary>
        Gost Gost { get; }     
    }
}
