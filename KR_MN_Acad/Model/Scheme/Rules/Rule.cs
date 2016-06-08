using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme.Rules
{
    public abstract class Rule : IRule
    {
        /// <summary>
        /// Дерево границ блоков
        /// </summary>
        protected RTree<ISchemeBlock> Tree;
        /// <summary>
        /// Блоки
        /// </summary>
        protected List<ISchemeBlock> Blocks;
        /// <summary>
        /// Запуск проверок
        /// </summary>        
        public abstract void Check(List<ISchemeBlock> blocks);

        protected void InitTree (List<ISchemeBlock> blocks)
        {
            Blocks = blocks;
            Tree = new RTree<ISchemeBlock>();
            foreach (var block in blocks)
            {
                Tree.Add(block.Rectangle, block);
            }            
        }        

        /// <summary>
        /// Наложение рабочих контуров блоков
        /// </summary>
        protected void Overlay()
        {
            HashSet<ISchemeBlock> checkeds = new HashSet<ISchemeBlock>();
            foreach (var item in Blocks)
            {
                checkeds.Add(item);
                var intersects = Tree.Intersects(item.Rectangle);                
                if (intersects.Any(i => !checkeds.Contains(i)))
                {
                    // Проверка действительного пересечения - по рабочему контуру блока (полилиния контура истинного положения блока, не граница)

                    Inspector.AddError($"Наложение блока: определено пересечение других блоков с этим блоком.",
                        item.IdBlref, System.Drawing.SystemIcons.Error);
                }
            }            
        }
    }
}
