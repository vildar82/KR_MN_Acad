using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.ArmWall.Blocks;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Настройка спецификации блоков схемы армирования стен
    /// </summary>
    public class ConstructionOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; } = true;
        public bool HasBillTable { get; set; } = true;
        public bool HasDetailTable { get; set; } = true;
        public List<ITableService> OtherTableService { get; set; }

        public ConstructionOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { ColumnSquareSmallBlock.BlockName, typeof(ColumnSquareSmallBlock) },
                { ColumnSquareBigBlock.BlockName, typeof(ColumnSquareBigBlock) },
                { PylonBlock.BlockName, typeof(PylonBlock) }
            };
            TableService = new ConstructionTable(db);
        }
    }
}
