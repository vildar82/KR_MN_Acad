using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Jigs;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using KR_MN_Acad.Scheme.Materials;

namespace KR_MN_Acad.Scheme.Spec
{
    /// <summary>
    /// Ведомость расхода стали
    /// </summary>
    public class BillSpec
    {
        SchemeService service;
        BillService bill;
        TableOptions options;
        BillRow data;

        //readonly int rowTitleIndex = 1;
        //readonly int rowGroupIndex = 2;
        readonly int rowMarkIndex = 1;
        protected readonly int rowNameIndex = 2;

        LineWeight lwBold;

        public BillSpec(BillService billService)
        {
            bill = billService;
            service = bill.Service;
            options = billService.Service.Options.Table;
            data = bill.Row;
        }        

        public Table CreateTable()
        {
            //using (var t = service.Db.TransactionManager.StartTransaction())
            //{
                // Создание таблицы
                return getTable();
                // Вставка таблицы
                //insertTable(table);
                //t.Commit();
            //}
        }

        private Table getTable ()
        {
            Table table = new Table();
            table.SetDatabaseDefaults(service.Db);
            table.TableStyle = service.Db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется
            if (!string.IsNullOrEmpty(options.Layer))
            {
                table.LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(options.Layer));
            }

            table.SetSize(4, 1);
            table.SetRowHeight(8);
            table.SetColumnWidth(15);

            // Добавление столбцов в таблицу
            BillColumn.AddColumns(table, data, this);
            // Заполнение расходов
            int row = rowNameIndex+1;
            // Строка в ведомости расхода стали                
            table.Cells[row, 0].TextString = data.Name;
            foreach (var item in data.Cells)
            {
                var colBil = BillColumn.GetColumn(item);
                table.Cells[row, colBil.Index].TextString = item.Amount.ToString();
            }

            row++;

            foreach (var col in table.Columns)
            {
                col.Alignment = CellAlignment.MiddleCenter;
            }

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 5;
            rowTitle.TextString = "ВРС";

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            //rowHeaders.Height = 15;
            lwBold = rowHeaders.Borders.Top.LineWeight.Value;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            table.SetBorders(lwBold);

            var rowName = table.Rows[rowNameIndex];
            rowName.Borders.Bottom.LineWeight = lwBold;
            var rowMark = table.Rows[rowMarkIndex];
            rowMark.Borders.Bottom.LineWeight = lwBold;

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table)
        {
            TableJig jigTable = new TableJig(table, AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(service.Db), "\nВставка таблицы:");
            if (service.Ed.Drag(jigTable).Status == PromptStatus.OK)
            {
                var cs = service.Db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(table);
                service.Db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);
            }
        }

        class BillColumn
        {
            static Dictionary<BillCell, BillColumn> cols;            
            BillCell cell;
            IBillMaterial mater;
            public int Index;            

            public BillColumn(BillCell item)
            {
                cell = item;
                mater = item.BillMaterial;            
            }            

            /// <summary>
            /// Добавление столбцов в ьаблицу
            /// </summary>
            public static void AddColumns(Table table,BillRow row, BillSpec spec)
            {
                // группировка по ячейкам
                var cells = row.Cells.GroupBy(g => g).OrderBy(o=>o.Key);

                var concretes = spec.service.Groups.FirstOrDefault(g => g.Type == GroupType.Materials)?.Rows.Where(r=>r.SomeElement is Concrete).GroupBy(g=>((Concrete)g.SomeElement).ClassB);

                table.InsertColumns(1, 15, cells.Count()+ concretes.Count() + 1); // +1 - столбец Всего стали
                int colIndex = 1;

                cols = new Dictionary<BillCell, BillColumn>();                

                foreach (var item in cells)
                {
                    BillColumn col = new BillColumn(item.Key);
                    cols.Add(item.Key, col);
                    col.Index = colIndex;                    
                    table.Cells[spec.rowMarkIndex, colIndex].TextString = col.mater.BillMark;
                    table.Cells[spec.rowNameIndex, colIndex].TextString = col.mater.BillName;
                    colIndex++;
                }
                var mCells = CellRange.Create(table, 1, colIndex, spec.rowNameIndex, colIndex);
                table.MergeCells(mCells);
                table.Cells[1, colIndex].TextString = "Всего";
                table.Cells[spec.rowNameIndex + 1, colIndex].TextString = row.Cells.Sum(s=>s.Amount).ToString();

                // всего бетона
                foreach (var concrete in concretes)
                {
                    colIndex++;                    
                    mCells = CellRange.Create(table, 1, colIndex, spec.rowNameIndex, colIndex);
                    table.MergeCells(mCells);
                    string unitsConcrete = ((Concrete)concrete.First().SomeElement).Units;
                    table.Cells[1, colIndex].TextString = $"Расход бетона класса {concrete.Key}, {unitsConcrete}";
                    table.Cells[spec.rowNameIndex + 1, colIndex].TextString = concrete.Sum(c=>c.Amount).ToString();
                }
            }

            public static BillColumn GetColumn(BillCell item)
            {
                BillColumn col;
                cols.TryGetValue(item,out col);
                return col;
            }
        }
    }
}
