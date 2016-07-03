using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.SlabOpenings
{
    /// <summary>
    /// Настройка спецификации блоков отверстий в плитах
    /// </summary>
    public class SlabOptions : ISpecOptions
    {
        public static readonly GroupType GroupOpening = new GroupType { Name="", Index=0 };
        public static readonly GroupType GroupSleeve = new GroupType { Name="", Index=1 };

        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; } = true;
        public bool HasBillTable { get; set; } = false;
        public bool HasDetailTable { get; set; } = false;

        public SlabOptions(Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.SlabOpeningBlock.BlockName, typeof(Blocks.SlabOpeningBlock) },
                { Blocks.SlabSleeveBlock.BlockName, typeof(Blocks.SlabSleeveBlock) }
            };

            TableService = new SlabService(db);            
        }
    }
}
