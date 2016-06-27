using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Openings
{
    /// <summary>
    /// Настройка спецификации блоков отверстий в плитах
    /// </summary>
    public class WallOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; }

        public WallOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.WallOpeningBlock.BlockName, typeof(Blocks.WallOpeningBlock) },
                { Blocks.WallSleeveBlock.BlockName, typeof(Blocks.WallSleeveBlock) }
            };
            TableService = new OpeningService(db);
            CheckDublicates = true;
        }
    }
}
