using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using KR_MN_Acad.Spec.Slab.Elements;

namespace KR_MN_Acad.Spec.Slab
{
    /// <summary>
    /// строка таблицы отверстий в плите
    /// </summary>
    public class SlabRow : ISpecRow
    {
        /// <summary>
        /// Элементы - должны быть одного типа!!!
        /// </summary>
        private List<ISlabElement> elements { get; set; }
        public string Group { get; set; }
        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }        

        public SlabRow (string group, List<ISlabElement> items)
        {
            Group = group;
            elements = items;            
            var first = elements.First();
            Mark = first.Mark;
            Dimension = first.Dimension;
            Role = first.Role;
            Count = items.Sum(s => s.Count);
            Description = first.Description;            
        }

        /// <summary>
        /// Нумерация элементов
        /// </summary>
        /// <param name="index"></param>
        public void Numbering (string index)
        {
            string num = elements.First().GetNumber(index);
            foreach (var item in elements)
            {
                item.SetNumber(num);
            }
        }

        /// <summary>
        /// Проверка одинаковости элементов в строке
        /// </summary>
        public void CheckSomeElements ()
        {            
            var groups = elements.GroupBy(g => g);
            if (groups.Skip(1).Any())
            {
                // Ошибка - разные элементы в одной строке
                foreach (var item in elements)
                {
                    Inspector.AddError($"Одинаковая марка у разных элементов. Марка = '{Mark}'",
                        item.SpecBlock.Block.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }
        }

        public void CheckSomeMark ()
        {
            // Марка элементов должна быть одинаковой
            var groups = elements.GroupBy(g => g.Mark);
            if (groups.Skip(1).Any())
            {
                // Ошибка - разные марки
                foreach (var item in elements)
                {
                    Inspector.AddError($"Разная марка у одинаковых элементов: {Dimension} {Role}",
                        item.SpecBlock.Block.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }
        }
    }
}
