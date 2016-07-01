using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Спецификация конструкции - элементов в одной сборочной конструкции - Колонне, Пилоне, Балке
    /// </summary>
    public class ConstructionTable : SpecGroup.SpecGroupService
    {   
        public ConstructionTable (Database db) : base(db)
        {   
            Title = "Спецификация конструкции";            
        }

        protected override IEnumerable<ISpecElement> FilterElements (IEnumerable<ISpecBlock> blocks, bool isNumbering)
        {
            if (!isNumbering)
            {
                // Заголовок таблицы по марке колонны в блоке (блок один, колонна одна).
                var block = blocks.First() as IConstructionBlock;
                var constrElem = block.ConstructionElement;
                Title += $" {constrElem.FriendlyName} {constrElem.Mark}";                
                return block.Elementary;                
            }
            else
            {
                return base.FilterElements(blocks, isNumbering);
            }
        }
    }
}
