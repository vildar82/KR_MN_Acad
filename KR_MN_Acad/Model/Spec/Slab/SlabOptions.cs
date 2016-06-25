using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Slab
{
    /// <summary>
    /// Настройка спецификации блоков отверстий в плитах
    /// </summary>
    public class SlabOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }

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
