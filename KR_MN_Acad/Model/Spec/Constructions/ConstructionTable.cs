using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Спецификация конструкции - элементов в одной сборочной конструкции - Колонне, Пилоне, Балке
    /// </summary>
    public class ConstructionTable : SpecGroup.SpecGroupService
    {
        private IConstructionBlock constrBlock;
        public ConstructionTable (Database db) : base(db)
        {   
            Title = "Спецификация конструкции";            
        }

        protected override IEnumerable<ISpecElement> FilterElements (IEnumerable<ISpecBlock> blocks, bool isNumbering)
        {
            if (!isNumbering)
            {
                // Заголовок таблицы по марке колонны в блоке (блок один, колонна одна).
                constrBlock = blocks.First() as IConstructionBlock;
                var constrElem = constrBlock.ConstructionElement;
                Title += $" {constrElem.FriendlyName} {constrElem.Mark}";                
                return constrBlock.Elementary;                
            }
            else
            {
                return base.FilterElements(blocks, isNumbering);
            }
        }

        public override List<IDetail> GetDetails ()
        {
            var details = constrBlock.Elementary.OfType<IDetail>().GroupBy(g=>g.Mark).
                OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New).Select(s=>s.First()).ToList();
            return details;
        }       
    }
}
