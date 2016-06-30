//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KR_MN_Acad.Spec.Constructions
//{
//    public class ConstructionRow : ISpecRow
//    {
//        public List<ISpecElement> Elements { get; set; }
//        public string Group { get; set; } = "";
//        /// <summary>
//        /// Марка - позиция
//        /// </summary>
//        public string Mark { get; set; } = "";
//        /// <summary>
//        /// Обозначение
//        /// </summary>
//        public string Designation { get; set; } = "";
//        /// <summary>
//        /// Наименование
//        /// </summary>
//        public string Name { get; set; } = "";
//        public string Count { get; set; } = "";
//        public string Weight { get; set; } = "";
//        public string Description { get; set; } = "";

//        public ConstructionRow (string group, List<ISpecElement> items)
//        {
//            Group = group;
//            Elements = items;
//            //var slabElems = items.Cast<IConstruction>();
//            var first = items.First() as IConstructionElement;// slabElems.First();
//            first.SumAndSetRow(this, items);
//        }
//    }
//}
