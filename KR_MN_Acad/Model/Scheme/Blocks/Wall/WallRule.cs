using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Правила для блоков схемы армирования стен
    /// </summary>
    public class WallRule : Rules.Rule
    {
        /// <summary>
        /// Проверка блоков
        /// </summary>
        /// <param name="blocks"></param>
        public override void Check(List<ISchemeBlock> blocks)
        {
            InitTree(blocks);
            
            // Проверка наложения блоков
            Overlay();
        }        
    }
}
