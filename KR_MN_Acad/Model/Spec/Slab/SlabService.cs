using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Slab.Elements;

namespace KR_MN_Acad.Spec.Slab
{
    public class SlabService : TableService
    {
        private List<KeyValuePair<string, List<SlabRow>>> groupRows;
        protected override Database Db { get; set; }
        protected override int NumColumns { get; set; }
        protected override int NumRows { get; set; }
        protected override string Title { get; set; }
        public SlabService(Database db)
        {
            Db = db;
            Title = "Ведомость инженерных отверстий плиты";
            NumColumns = 5;
        }

        /// <summary>
        /// Группировка элементов спецификации.
        /// Нумерация берется из объектов
        /// </summary>        
        public override void CalcRows (List<ISpecBlock> blocks)
        {
            int numrows = 0;
            groupRows = new List<KeyValuePair<string, List<SlabRow>>> ();
            var rows = new List<SlabRow> ();
            // Все элементы спецификации
            var elements = blocks.SelectMany(b=>b.Elements.Cast<ISlabElement>());
            // группировка уникальности элементов
            var openingsGroup = elements.OrderBy(o=>o.Index).GroupBy(g=>g).OrderBy(g=>g.Key.Mark, alpha);
            // Проверка уникальности марок элеметнов
            CheckUniqueMarks(openingsGroup);
            string group = "";
            foreach (var item in openingsGroup)
            {
                var row = new SlabRow(group, item.ToList());
                // Проверка одинаковости марки
                row.CheckSomeMark();
                rows.Add(row);
                numrows++;
            }
            groupRows.Add(new KeyValuePair<string, List<SlabRow>>(group, rows.ToList()));
            NumRows = numrows+2;
        }        

        private void CheckUniqueMarks (IOrderedEnumerable<IGrouping<ISlabElement, ISlabElement>> groups)
        {
            var markGroups = groups.GroupBy(g => g.Key.Mark, (k, g) =>
                new { Key = k, Error = g.Skip(1).Any(), Elements = g.SelectMany(s => s.Select(i => i)) }).Where(w=>w.Error);
            foreach (var item in markGroups)
            {
                foreach (var elem in item.Elements)
                {
                    Inspector.AddError($"Одинаковая марка у разных элементов. Марка = {elem.Mark}", 
                        elem.SpecBlock.Block.IdBlRef, System.Drawing.SystemIcons.Error);
                }                
            }
        }

        /// <summary>
        /// Создание и заполнение тапбицы
        /// </summary>        
        public override Table CreateTable ()
        {   
            var table = GetTable();
            return table;
        }        

        /// <summary>
        /// Нумерация элементов
        /// </summary>
        /// <param name="blocks"></param>
        public override void Numbering (List<ISpecBlock> blocks)
        {   
            // Все элементы спецификации
            var elements = blocks.SelectMany(b=>b.Elements.Cast<ISlabElement>());
            // Проемы в плите
            var indexGroups = elements.GroupBy(g=>g.Index).OrderBy(o=>o.Key);
            foreach (var indexGroup in indexGroups)
            {
                var dimGroups = indexGroup.GroupBy(g=>g.Dimension).OrderByDescending(o=>o.Key, alpha);
                int index = 1;
                string group = "";
                foreach (var dimGroup in dimGroups)
                {
                    // группировка по назначению
                    var roleGroups = dimGroup.GroupBy(g=>g.Role).OrderBy(o=>o.Key);
                    if (roleGroups.Skip(1).Any())
                    {
                        // Есть подгруппы вида - одинаковые размеры у отв но разное назначение - нумерация вида 1.1
                        int indexRole = 1;
                        foreach (var item in roleGroups)
                        {
                            string indexSubgroup = index + "." + indexRole;
                            var row = new SlabRow(group, item.ToList());
                            row.Numbering(indexSubgroup);
                            indexRole++;
                        }
                    }
                    else
                    {
                        var row = new SlabRow(group, dimGroup.ToList());
                        row.Numbering(index.ToString());
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// Определение назначения отверстия по слою
        /// </summary>
        /// <param name="blLayer">Слой на котором расположен блок</param>
        /// <returns>Назначение: "АР", "ОВ" и т.д.</returns>
        public static string GetRole (IBlock block)
        {
            string role = "";
            switch (block.BlLayer)
            {
                case "КР_Отв._АР":
                    role = "АР";
                    break;
                case "КР_Отв._ВК":
                    role = "ВК";
                    break;
                case "КР_Отв._КЖ":
                    role = "КЖ";
                    break;
                case "КР_Отв._КР":
                    role = "КР";
                    break;
                case "КР_Отв._ОВ":
                    role = "ОВ";
                    break;
                case "КР_Отв._СС":
                    role = "СС";
                    break;
                case "КР_Отв._ТС":
                    role = "ТС";
                    break;
                case "КР_Отв._ЭОМ":
                    role = "ЭОМ";
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(role))
            {
                Inspector.AddError($"не определено назначение блока '{block.BlName}' по слою '{block.BlLayer}'",
                    block.IdBlRef, System.Drawing.SystemIcons.Error);
            }
            return role;
        }

        protected override void SetColumnsAndCap (ColumnsCollection columns)
        {
            // столбец Марка
            var col = columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 10;
            col[1, 0].TextString = "Марка отв.";
            // столбец Размеры
            col = columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            col[1, 1].TextString = "Размеры, мм";
            // столбец Назначение
            col = columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            col[1, 2].TextString = "Назначение";
            // столбец Кол
            col = columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            col[1, 3].TextString = "Кол-во, шт.";
            // столбец примечаение
            col = columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 30;
            col[1, 4].TextString = "Примечание";            
        }

        protected override void FillCells (Table table)
        {
            int row = 2;
            Cell cell;
            foreach (var group in groupRows)
            {   
                foreach (var item in group.Value)
                {
                    cell = table.Cells[row, 0];
                    cell.TextString = item.Mark;                    

                    cell = table.Cells[row, 1];
                    cell.TextString = item.Dimension;                    

                    cell = table.Cells[row, 2];
                    cell.TextString = item.Role;

                    cell = table.Cells[row, 3];
                    cell.TextString = item.Count.ToString();

                    cell = table.Cells[row, 4];
                    cell.TextString = item.Description;                    

                    row++;
                }
            }
        }
    }
}
