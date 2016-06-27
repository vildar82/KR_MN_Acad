using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Monolith
{
    /// <summary>
    /// Настройка спецификации блоков монолита
    /// </summary>
    public class MonolithOptions : ISpecOptions
    {        
        public Dictionary<string, Type> TypesBlock { get; set; }
        public ITableService TableService { get; set; }
        public bool CheckDublicates { get; set; }

        public MonolithOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.BeamBlock.BlockName, typeof(Blocks.BeamBlock) },
                { Blocks.ColumnBlock.BlockName, typeof(Blocks.ColumnBlock) },
                { Blocks.PilonBlock.BlockName, typeof(Blocks.PilonBlock) },
                { Blocks.WallBlock.BlockName, typeof(Blocks.WallBlock) },
            };

            TableService = new MonolithService(db);
            CheckDublicates = true;
        }
    }
}
