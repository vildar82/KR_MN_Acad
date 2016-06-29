using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Группы - подзаголовки в спецификациях
    /// </summary>
    public class GroupType
    {        
        /// <summary>
        /// Стержни
        /// </summary>
        public static readonly GroupType Bars = new GroupType("Стержни");
        /// <summary>
        /// Детали
        /// </summary>
        public static readonly GroupType Details = new GroupType("Детали");
        /// <summary>
        /// Гильзы
        /// </summary>
        public static readonly GroupType Sleeves = new GroupType("Гильзы");
        /// <summary>
        /// Закладные детали
        /// </summary>
        public static readonly GroupType EmbeddedDetails = new GroupType("Закладные детали");
        /// <summary>
        /// Материалы
        /// </summary>
        public static readonly GroupType Materials = new GroupType("Материалы");
        /// <summary>
        /// Балки монолитные
        /// </summary>
        public static readonly GroupType MonolithBeam = new GroupType("Балки монолоитные");
        /// <summary>
        /// Колонны монолитные
        /// </summary>
        public static readonly GroupType MonolithColumn = new GroupType("Колонны монолоитные" );
        /// <summary>
        /// Пилоны монолоитные
        /// </summary>
        public static readonly GroupType MonolithPylon = new GroupType("Пилоны монолоитные");
        /// <summary>
        /// Стены монолоитные
        /// </summary>
        public static readonly GroupType MonolithWall = new GroupType("Стены монолоитные");        

        public readonly string Name;
        private GroupType (string name)
        {
            Name = name;
        }       
    }
}
