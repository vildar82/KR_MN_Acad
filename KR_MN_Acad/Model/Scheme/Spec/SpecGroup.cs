using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices.Materials;
using KR_MN_Acad.Scheme.Elements;

namespace KR_MN_Acad.Scheme.Spec
{
    public enum GroupType
    {
        Unknown,
        /// <summary>
        /// Стержни
        /// </summary>
        Armatures,
        /// <summary>
        /// Детали
        /// </summary>
        Details,
        /// <summary>
        /// Закладные детали
        /// </summary>
        EmbeddedDetails,
        /// <summary>
        /// Материалы
        /// </summary>
        Materials
    }    

    /// <summary>
    /// Группа в спецификации материалов схемы армирования
    /// </summary>
    public class SpecGroup
    {    
        /// <summary>
        /// Имя группы
        /// </summary>
        public GroupType Type { get; set; }
        /// <summary>
        /// Елементы группы
        /// </summary>
        public List<ISpecRow> Rows { get; set; }
        public string Name { get; set; }
        public List<IElement> Elements { get; set; }

        public SpecGroup(IGrouping<GroupType, IElement> elems)
        {
            Type = elems.Key;
            Name = GetGroupName(Type);
            Elements = elems.ToList();
        }

        public static string GetGroupName(GroupType type)
        {
            switch (type)
            {
                case GroupType.Unknown:
                    return "";
                case GroupType.Armatures:
                    return "Стержни";
                case GroupType.Details:
                    return "Детали";
                case GroupType.EmbeddedDetails:
                    return "Закладные детали";
                case GroupType.Materials:
                    return "Материалы";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Калькуляция группы - сортировка по элементам и назначение позиций
        /// </summary>
        public void Calculate()
        {
            Rows = new List<ISpecRow>();
            // Группировка элементов по типам
            var someElems = Elements.GroupBy(g => g).OrderBy(o => o.Key);
            int posIndex = 1;
            foreach (var item in someElems)
            {
                // Составить строчку таблицы
                ISpecRow row = new SpecRow(item.Key.Prefix+posIndex, item.ToList());
                row.Calculate();
                Rows.Add(row);
                posIndex++;
            }
        }
    }
}
