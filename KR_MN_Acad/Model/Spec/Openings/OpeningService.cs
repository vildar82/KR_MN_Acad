using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Openings.Elements;

namespace KR_MN_Acad.Spec.Openings
{
    public class OpeningService : TableService
    {        
        protected override Database Db { get; set; }
        protected override int NumColumns { get; set; }
        protected override int NumRows { get; set; }
        protected override string Title { get; set; }
        public OpeningService (Database db)
        {            
            Db = db;
            Title = "Ведомость инженерных отверстий";
            NumColumns = 6;
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
            // столбец Отметка
            col = columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            col[1, 2].TextString = "Отм. низа проема, м";
            // столбец Назначение
            col = columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;
            col[1, 3].TextString = "Назначение";
            // столбец Кол
            col = columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            col[1, 4].TextString = "Кол-во, шт.";
            // столбец примечаение
            col = columns[5];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 30;
            col[1, 5].TextString = "Примечание";            
        }

        protected override void FillCells (Table table)
        {
            int row = 2;
            Cell cell;
            foreach (var group in groupRows)
            {   
                foreach (var item in group.Value.Cast<OpeningRow>())
                {
                    cell = table.Cells[row, 0];
                    cell.Borders.Horizontal.Margin = 0;
                    cell.TextString = item.Mark;                    

                    cell = table.Cells[row, 1];
                    cell.TextString = item.Dimension;

                    cell = table.Cells[row, 2];
                    cell.TextString = item.Elevation;

                    cell = table.Cells[row, 3];
                    cell.TextString = item.Role;

                    cell = table.Cells[row, 4];
                    cell.TextString = item.Count.ToString();

                    cell = table.Cells[row, 5];
                    cell.TextString = item.Description;                    

                    row++;
                }
            }
        }

        protected override ISpecRow GetNewRow (string group, List<ISpecElement> items)
        {
            var res = new OpeningRow(group, items);
            return res;
        }        

        protected override Dictionary<string, List<ISpecElement>> GroupsFirstForNumbering (IGrouping<GroupType, ISpecElement> indexGroup)
        {
            var dimRoleGroups = indexGroup.GroupBy(g=>((IOpeningElement)g).Dimension+((IOpeningElement)g).Role).OrderByDescending(o=>o.Key, alpha);
            return dimRoleGroups.ToDictionary(k => k.Key, i => i.ToList());
        }

        protected override Dictionary<string, List<ISpecElement>> GroupsSecondForNumbering (KeyValuePair<string, List<ISpecElement>> firstGroup)
        {
            var elevGroups = firstGroup.Value.GroupBy(g=>((IOpeningElement)g).Elevation).OrderBy(o=>o.Key);
            return elevGroups.ToDictionary(k => k.Key, i => i.ToList());
        }
    }
}
