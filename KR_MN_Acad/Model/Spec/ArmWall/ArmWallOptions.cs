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
        public bool CheckDublicates { get; set; } = true;
        public bool HasBillTable { get; set; } = true;
        public bool HasDetailTable { get; set; } = true;

        public ArmWallOptions (Database db)
        {
            TypesBlock = new Dictionary<string, Type>() {
                { Blocks.WallBlock.BlockName, typeof(Blocks.WallBlock) },
                { Blocks.WallEndBlock.BlockName, typeof(Blocks.WallEndBlock) },
                { Blocks.WallEndCornerBlock.BlockName, typeof(Blocks.WallEndCornerBlock) },
                { Blocks.WallEndTBlock.BlockName, typeof(Blocks.WallEndTBlock) },
                { Blocks.ColumnSquareSmallBlock.BlockName, typeof(Blocks.ColumnSquareSmallBlock) },
                { Blocks.ColumnSquareBigBlock.BlockName, typeof(Blocks.ColumnSquareBigBlock) },
                { Blocks.PylonBlock.BlockName, typeof(Blocks.PylonBlock) },
                { Blocks.SleeveBlock.BlockName, typeof(Blocks.SleeveBlock) },
                { Blocks.AddHorArmBlock.BlockName, typeof(Blocks.AddHorArmBlock) },
                { Blocks.AddVerticLShapedArmBlock.BlockName, typeof(Blocks.AddVerticLShapedArmBlock) },
                { Blocks.DoorBlock.BlockName, typeof(Blocks.DoorBlock) }                
            };

            TableService = new SpecGroup.SpecGroupService(db);            
        }
    }
}
