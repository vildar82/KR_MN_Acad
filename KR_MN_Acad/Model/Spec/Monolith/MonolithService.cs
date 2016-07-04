using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec.Monolith
{
    public class MonolithService : TableService
    {        
        protected override Database Db { get; set; }
        protected override int NumColumns { get; set; }
        protected override int NumRows { get; set; }
        protected override string Title { get; set; }
        public MonolithService (Database db)
        {            
            Db = db;
            Title = "Спецификация к схеме расположения элементов замаркированных на данном листе";
            NumColumns = 6;
        }        

        protected override void SetColumnsAndCap (ColumnsCollection columns)
        {
            // столбец Марка
            var col = columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            col[1, 0].TextString = "Марка";
            // столбец Обозначение
            col = columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 60;
            col[1, 1].TextString = "Обозначение";
            // столбец Наименование
            col = columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 65;
            col[1, 2].TextString = "Наименование";
            // столбец Кол
            col = columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 10;
            col[1, 3].TextString = "Кол.";
            // столбец Масса, ед. кг
            col = columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            col[1, 4].TextString = "Масса, ед. кг";
            // столбец примечаение
            col = columns[5];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            col[1, 5].TextString = "Примечание";            
        }

        protected override void FillCells (Table table)
        {
            int row = 2;
            Cell cell;
            foreach (var group in groupRows)
            {
                var groupName = group.Value.First().Group;
                if (!string.IsNullOrEmpty(groupName))
                {
                    table.Cells[row, 2].TextString = $"{{\\L{groupName}}}";
                    row++;
                }
                foreach (var item in group.Value.Cast<MonolithRow>())
                {
                    cell = table.Cells[row, 0];
                    cell.TextString = item.Mark;                    

                    cell = table.Cells[row, 1];
                    cell.TextString = item.Designation;
                    cell.Alignment = CellAlignment.MiddleLeft;

                    cell = table.Cells[row, 2];
                    cell.TextString = item.Name;
                    cell.Alignment = CellAlignment.MiddleLeft;

                    cell = table.Cells[row, 3];
                    cell.TextString = item.Count.ToString();

                    cell = table.Cells[row, 4];
                    cell.TextString = item.Weight;

                    cell = table.Cells[row, 5];
                    cell.TextString = item.Description;                    

                    row++;
                }
            }
        }

        protected override ISpecRow GetNewRow (string group, List<ISpecElement> items)
        {
            var res = new MonolithRow(group, items);
            return res;
        }       

        protected override Dictionary<string, List<ISpecElement>> GroupsFirstForNumbering (IGrouping<GroupType, ISpecElement> indexGroup)
        {
            var uniqElems = indexGroup.GroupBy(g=>g).OrderByDescending(o=>o.Key);
            return uniqElems.ToDictionary(k => ((Elements.IConstruction)k.Key).Name, i => i.ToList());
        }

        protected override Dictionary<string, List<ISpecElement>> GroupsSecondForNumbering (KeyValuePair<string, List<ISpecElement>> firstGroup)
        {
            return new Dictionary<string, List<ISpecElement>>() {
                { firstGroup.Key, firstGroup.Value }
            };
        }
    }
}
