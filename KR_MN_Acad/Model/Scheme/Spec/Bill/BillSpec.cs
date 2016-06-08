using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Jigs;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

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
        List<BillRow> data;

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
            data = bill.Rows;
        }        

        public void CreateTable()
        {
            using (var t = service.Db.TransactionManager.StartTransaction())
            {
                // Создание таблицы
                Table table = getTable();
                // Вставка таблицы
                insertTable(table);
                t.Commit();
            }
        }

        private Table getTable()
        {
            Table table = new Table();
            table.SetDatabaseDefaults(service.Db);
            table.TableStyle = service.Db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется
            if (!string.IsNullOrEmpty(options.Layer))
            {
                table.LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(options.Layer));
            }

            table.SetSize(3, 1);            
            table.SetRowHeight(8);
            table.SetColumnWidth(15);                        

            // все столбцы таблицы
            BillColumn.CalcColumns(data, this);
            // Добавление столбцов в таблицу
            BillColumn.AddColumns(table);
            // Заполнение расходов
            int row = rowNameIndex;
            foreach (var bilRow in data)
            {
                // Строка в ведомости расхода стали
                table.InsertRows(++row, 8, 1);                
                table.Cells[row, 0].TextString = bilRow.Name;
                foreach (var item in bilRow.Cells)
                {
                    var colBil = BillColumn.GetColumn(item);
                    table.Cells[row, colBil.Index].TextString = item.Amount.ToString();
                }
                row++;
            }

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
            static BillSpec spec;
            BillCell cell;
            IBillMaterial mater;
            public int Index;

            public BillColumn(BillCell item)
            {
                cell = item;
                mater = item.BillMaterial;            
            }

            /// <summary>
            /// Определение столбцов по данным
            /// </summary>            
            public static void CalcColumns (List<BillRow> rows, BillSpec spec)
            {
                BillColumn.spec = spec;
                cols = new Dictionary<BillCell, BillColumn>();

                // группировка по ячейкам
                var cells = rows.SelectMany(s => s.Cells).GroupBy(g => g).OrderBy(o=>o.Key);

                foreach (var item in cells)
                {                    
                    BillColumn col = new BillColumn(item.Key);
                    cols.Add(item.Key, col);
                }                
            }

            /// <summary>
            /// Добавление столбцов в ьаблицу
            /// </summary>
            public static void AddColumns(Table table)
            {
                table.InsertColumns(1, 15, cols.Count);                
                int colIndex = 1;                

                foreach (var item in cols)
                {
                    item.Value.Index = colIndex;                   

                    table.Cells[spec.rowNameIndex, colIndex].TextString = item.Value.mater.BillName;                    
                    table.Cells[spec.rowMarkIndex, colIndex].TextString = item.Value.mater.BillMark;
                    colIndex++;
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
