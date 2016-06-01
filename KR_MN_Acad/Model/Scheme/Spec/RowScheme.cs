using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Строчка в спецификации материалов схемы армирования
    /// </summary>
    public abstract class RowScheme : IComparable<RowScheme>, IEquatable<RowScheme>
    {
        /// <summary>
        /// Позиция - столбец
        /// </summary>
        public string PositionColumn { get; protected  set; }
        /// <summary>
        /// Обозначение - столбец
        /// </summary>
        public string DocumentColumn { get; protected set; }
        /// <summary>
        /// Наименование - столбец
        /// </summary>
        public string NameColumn { get; protected set; }
        /// <summary>
        /// Кол.
        /// </summary>
        public string CountColumn { get; protected set; }
        /// <summary>
        /// Масса ед., кг
        /// </summary>
        public string WeightColumn { get; protected set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string DescriptionColumn { get; protected set; }
        /// <summary>
        /// Группа элемента
        /// </summary>
        public GroupType Type { get; protected set; }   
        /// <summary>
        /// Префикс позиции - используется только для группировки и сравнения строк.
        /// </summary>
        public string PositionPrefix { get; protected set; }     
        /// <summary>
        /// Запись постоянных значений таблицы - обозн, наимен, масса ед.
        /// </summary>
        public abstract void PrepareConstTable();
        /// <summary>
        /// Финальное заполнение значений таблицы - кол., примечание
        /// </summary>
        public abstract void PrepareFinalTable();
        /// <summary>
        /// Установка позиции
        /// </summary>
        /// <param name="value"></param>
        public abstract void SetPosition(int value);
        /// <summary>
        /// Суммирование элементов
        /// </summary>
        /// <param name="elem"></param>
        public abstract void Add(RowScheme elem);

        public RowScheme(GroupType type, string posPrefix)
        {
            PositionPrefix = posPrefix;
            Type = type;
        }

        public int CompareTo(RowScheme other)
        {
            var result = Type.CompareTo(other.Type);
            if (result != 0) return result;

            result = string.Compare(PositionPrefix, other.PositionPrefix, true);
            if (result != 0) return result;

            result = string.Compare(DocumentColumn,other.DocumentColumn, true);
            if (result != 0) return result;

            result = string.Compare(NameColumn, other.NameColumn, true);
            if (result != 0) return result;

            return 0;            
        }

        public bool Equals(RowScheme other)
        {
            return this.CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ PositionPrefix.GetHashCode();
        }
    }
}
