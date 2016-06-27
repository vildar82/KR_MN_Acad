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
    public class ApertureOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; }

        public ApertureOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.DoorBlock.BlockName, typeof(Blocks.DoorBlock) },
                { Blocks.WindowBlock.BlockName, typeof(Blocks.WindowBlock) }
            };
            TableService = new OpeningService(db);
            CheckDublicates = true;
        }
    }
}
