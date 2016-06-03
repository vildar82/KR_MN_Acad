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
    public class SchemeGroup
    {
        /// <summary>
        /// Имя группы
        /// </summary>
        public GroupType Type { get; set; }
        /// <summary>
        /// Елементы группы
        /// </summary>
        public List<SchemeRow> Rows { get; set; }
        public string Name { get; set; }

        public SchemeGroup(IGrouping<GroupType, IMaterial> type)
        {
            Rows = new List<SchemeRow>();
            Type = type.Key;
            Name = GetGroupName(Type);
            var rowsGroup = type.GroupBy(g => g.RowScheme, s=>s).OrderByDescending(o => o.Key.NameColumn, SchemeRow.Alpha);

            int pos = 1;
            foreach (var rowPos in rowsGroup)
            {
                var mater = rowPos.First().Copy();
                // суммирование материалов
                foreach (var item in rowPos.Skip(1))
                {
                    mater.Add(item);
                }
                SchemeRow row = mater.RowScheme;
                row.Materials = rowPos.ToList();
                row.SetPosition(pos);
                Rows.Add(row);
                pos++;
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
