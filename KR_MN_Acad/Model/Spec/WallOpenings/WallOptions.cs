using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.WallOpenings
{
    /// <summary>
    /// Настройка спецификации блоков отверстий в плитах
    /// </summary>
    public class WallOptions : ISpecOptions
    {
        public static readonly GroupType GroupOpening = new GroupType { Name="", Index=0 };
        public static readonly GroupType GroupSleeve = new GroupType { Name="", Index=1 };

        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; } = true;
        public bool HasBillTable { get; set; } = false;
        public bool HasDetailTable { get; set; } = false;

        public List<ITableService> OtherTableService { get; set; }

        public WallOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.WallOpeningBlock.BlockName, typeof(Blocks.WallOpeningBlock) },
                { Blocks.WallSleeveBlock.BlockName, typeof(Blocks.WallSleeveBlock) }
            };
            TableService = new OpeningService(db);
            // Групповая спецификация для Гильз
            OtherTableService = new List<ITableService>();
            OtherTableService.Add (new SpecGroup.SpecGroupService(db));
        }
    }
}
