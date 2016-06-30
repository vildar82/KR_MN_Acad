using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Спецификация конструкции - элементов в одной сборочной конструкции - Колонне, Пилоне, Балке
    /// </summary>
    public class ConstructionService : SpecGroup.SpecGroupService
    {   
        public ConstructionService (Database db) : base(db)
        {   
            Title = "Спецификация конструкции";            
        }
    }
}
