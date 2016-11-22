using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.SlabOpenings.Elements;

namespace KR_MN_Acad.Spec.SlabOpenings
{
    public class SlabService : TableService
    {        
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
                foreach (var item in group.Value.Cast<SlabRow>())
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

        protected override ISpecRow GetNewRow (string group, List<ISpecElement> items)
        {
            ISpecRow res = null;
            var specItems = items.Where(i => i is ISlabElement);
            if (specItems.Any())
            {
                res = new SlabRow(group, items);
            }            
            return res;
        }

        protected override Dictionary<string, List<ISpecElement>> GroupsFirstForNumbering (IGrouping<GroupType, ISpecElement> indexGroup)
        {            
            var dimGroups = indexGroup.Where(w=>w is ISlabElement).GroupBy(g=>((ISlabElement)g).Dimension).OrderByDescending(o=>o.Key, alpha);
            if (dimGroups.Any()                )
            {
                return dimGroups.ToDictionary(k => k.Key, i => i.ToList());
            }
            return new Dictionary<string, List<ISpecElement>>();            
        }

        protected override Dictionary<string, List<ISpecElement>> GroupsSecondForNumbering (KeyValuePair<string, List<ISpecElement>> firstGroup)
        {
            var roleGroups = firstGroup.Value.Where(w => w is ISlabElement).GroupBy(g=>((ISlabElement)g).Role).OrderBy(o=>o.Key);
            if (roleGroups.Any())
            {
                return roleGroups.ToDictionary(k => k.Key, i => i.ToList());
            }
            return new Dictionary<string, List<ISpecElement>>();
        }
    }
}
