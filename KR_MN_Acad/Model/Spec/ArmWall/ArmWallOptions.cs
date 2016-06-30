using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.ArmWall
{
    /// <summary>
    /// Настройка спецификации блоков схемы армирования стен
    /// </summary>
    public class ArmWallOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; }

        public ArmWallOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Constructions.Blocks.ColumnSquareSmallConstr.BlockName, typeof(Constructions.Blocks.ColumnSquareSmallConstr) }                
            };

            TableService = new SpecGroup.SpecGroupService(db);
            CheckDublicates = true;
        }
    }
}
