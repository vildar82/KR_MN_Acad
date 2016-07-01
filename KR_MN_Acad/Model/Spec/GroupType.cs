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
    public class GroupType : IEquatable<GroupType>, IComparable<GroupType>
    {
        public static readonly GroupType None = new GroupType("",0);
        /// <summary>
        /// Стержни
        /// </summary>
        public static readonly GroupType Bars = new GroupType("Стержни",1);
        /// <summary>
        /// Балки монолитные
        /// </summary>
        public static readonly GroupType MonolithBeam = new GroupType("Балки монолоитные",2);
        /// <summary>
        /// Колонны монолитные
        /// </summary>
        public static readonly GroupType MonolithColumn = new GroupType("Колонны монолоитные",3);
        /// <summary>
        /// Пилоны монолоитные
        /// </summary>
        public static readonly GroupType MonolithPylon = new GroupType("Пилоны монолоитные",4);
        /// <summary>
        /// Стены монолоитные
        /// </summary>
        public static readonly GroupType MonolithWall = new GroupType("Стены монолоитные",5);
        /// <summary>
        /// Детали
        /// </summary>
        public static readonly GroupType Details = new GroupType("Детали",6);
        /// <summary>
        /// Гильзы
        /// </summary>
        public static readonly GroupType Sleeves = new GroupType("Гильзы",7);
        /// <summary>
        /// Закладные детали
        /// </summary>
        public static readonly GroupType EmbeddedDetails = new GroupType("Закладные детали",8);
        /// <summary>
        /// Материалы
        /// </summary>
        public static readonly GroupType Materials = new GroupType("Материалы",9);        

        public string Name { get; set; }
        public int Index { get; set; }
        private GroupType (string name, int index)
        {
            Name = name;
            Index = index;      
        }       

        public GroupType()
        {

        }

        public bool Equals (GroupType other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Index == other.Index && Name == other.Name;
        }

        public int CompareTo (GroupType other)
        {
            var res = Index.CompareTo(other.Index);
            if (res != 0) return res;

            res = Name.CompareTo(other.Name);
            return res;
        }

        public override int GetHashCode ()
        {
            return Index.GetHashCode();
        }        
    }
}
