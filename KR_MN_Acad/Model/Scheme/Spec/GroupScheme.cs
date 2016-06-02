using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices.Materials;

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
    public class GroupScheme
    {
        /// <summary>
        /// Имя группы
        /// </summary>
        public GroupType Type { get; set; }
        /// <summary>
        /// Елементы группы
        /// </summary>
        public List<RowScheme> Rows { get; set; }

        public GroupScheme(IGrouping<GroupType, IMaterial> type)
        {
            Rows = new List<RowScheme>();
            Type = type.Key;
            var rowsGroup = type.GroupBy(g => g).OrderBy(o => o);

            int pos = 1;
            foreach (var rowPos in rowsGroup)
            {
                var mater = rowPos.First().Copy();
                // суммирование материалов
                foreach (var item in rowPos)
                {
                    mater.Add(item);
                }
                RowScheme row = mater.GetRow();
                row.Materials = rowPos.ToList();
                row.SetPosition(pos);
                Rows.Add(row);
            }
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
    }
}
