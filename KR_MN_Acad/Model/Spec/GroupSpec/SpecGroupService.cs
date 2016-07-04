using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements;

namespace KR_MN_Acad.Spec.SpecGroup
{
    /// <summary>
    /// Групповая спецификация схемы
    /// </summary>
    public class SpecGroupService : TableService
    {        
        protected override Database Db { get; set; }
        protected override int NumColumns { get; set; }
        protected override int NumRows { get; set; }
        protected override string Title { get; set; }
        public SpecGroupService (Database db)
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
            col.Borders.Horizontal.Margin = 0;
            col[1, 0].TextString = "Поз.";
            // столбец Обозначение
            col = columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 60;
            col.Borders.Horizontal.Margin = 1.5;
            col[1, 1].TextString = "Обозначение";
            // столбец Наименование
            col = columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 65;
            col.Borders.Horizontal.Margin = 1.5;
            col[1, 2].TextString = "Наименование";
            // столбец Кол
            col = columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 10;
            col.Borders.Horizontal.Margin = 0;
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
                foreach (var item in group.Value.Cast<SpecGroupRow>())
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
            var res = new SpecGroupRow(group, items);
            return res;
        }       

        protected override Dictionary<string, List<ISpecElement>> GroupsFirstForNumbering (IGrouping<int, ISpecElement> indexTypeGroup)
        {
            var firstElement = indexTypeGroup.First() as IGroupSpecElement;
            if (!firstElement.IsDefaultGroupings)
            {
                return firstElement.GroupsBySize(indexTypeGroup);
            }
            {
                var uniqElems = indexTypeGroup.GroupBy(g=>g).OrderByDescending(o=>o.Key);
                return uniqElems.ToDictionary(k => k.Key.Key, i => i.ToList());
            }
        }

        protected override Dictionary<string, List<ISpecElement>> GroupsSecondForNumbering (KeyValuePair<string, List<ISpecElement>> firstGroup)
        {
            var firstElement = firstGroup.Value.First() as IGroupSpecElement;
            if (!firstElement.IsDefaultGroupings)
            {
                return firstElement.GroupsByArm(firstGroup.Value);
            }
            else
            {
                return new Dictionary<string, List<ISpecElement>>() {
                    { firstGroup.Key, firstGroup.Value }
                };
            }
        }

        public override List<IDetail> GetDetails ()
        {            
            var details = elements.OfType<IDetail>().GroupBy(g=>g.Mark).
                OrderBy(o => o.Key, AcadLib.Comparers.AlphanumComparator.New).Select(s=>s.First()).ToList();
            return details;
        }

        public override List<ISpecElement> GetElementsForBill (List<ISpecBlock> blocks)
        {
            List < ISpecElement > res = new List<ISpecElement> ();
            foreach (var block in blocks)
            {
                if (block is Constructions.IConstructionBlock)
                {
                    res.AddRange(((Constructions.IConstructionBlock)block).Elementary);
                }
                else
                {
                    res.AddRange(block.Elements);
                }
            }
            return res;
        }
    }
}
