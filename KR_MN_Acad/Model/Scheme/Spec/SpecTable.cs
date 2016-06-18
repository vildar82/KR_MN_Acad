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
    /// Таблица спецификации материалов схемы армирования
    /// </summary>
    public class SpecTable
    {
        public ObjectId LayerId { get; private set; }
        SchemeService service;
        TableOptions options;
        List<SpecGroup> data;
        double scale;
        public SpecTable (SchemeService service)
        {
            this.service = service;
            options = service.Options.Table;
            data = service.Groups;
        }

        public Table CreateTable ()
        {
            //using (var t =service.Db.TransactionManager.StartTransaction())
            //{
                scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(service.Db);
                // Создание таблицы
                return getTable();
                //// Вставка таблицы
                //insertTable(table);
                //t.Commit();
            //}
        }

        private Table getTable()
        {
            Table table = new Table();
            table.SetDatabaseDefaults(service.Db);
            table.TableStyle = service.Db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется
            if (!string.IsNullOrEmpty(options.Layer))
            {
                LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(options.Layer));
                table.LayerId = LayerId;
            }
            else
            {
                LayerId = service.Db.Clayer;
            }
            								  
            table.SetSize(3, 6);
            table.SetBorders(LineWeight.LineWeight050);
            table.SetRowHeight(8);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
			rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 5;
            rowTitle.TextString = options.Title;

            // столбец ПОЗ
            var col = table.Columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            // столбец Обозн.
            col = table.Columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 60;            
            // столбец Наимен
            col = table.Columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 65;            
            // столбец Кол
            col = table.Columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 10;            
            // столбец Масса
            col = table.Columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 15;
            // столбец Примечание
            col = table.Columns[5];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 20;

            // Заголовок ПОЗ
            var cellColName = table.Cells[1, 0];
            cellColName.TextString = "Поз.";                        
            // Заголовок Обозн
            cellColName = table.Cells[1, 1];
            cellColName.TextString = "Обозначение";
            // Заголовок Наимен
            cellColName = table.Cells[1, 2];
            cellColName.TextString = "Наименование";            
            // Заголовок Кол
            cellColName = table.Cells[1, 3];
            cellColName.TextString = "Кол.";            
            // Заголовок Масса
            cellColName = table.Cells[1, 4];
            cellColName.TextString = "Масса, ед.,кг";            
            // Заголовок Примеч
            cellColName = table.Cells[1, 5];
            cellColName.TextString = "Приме- чание";                        

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 15;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            int row = 2;
            Cell cell;          
            foreach (var group in data)
            {
                int rows = group.Rows.Count;
                if (!string.IsNullOrEmpty(group.Name))
                {
                    rows++;
                    table.InsertRows(row, 8, rows);
                    cell = table.Cells[row, 2];
                    cell.TextString = $"{{\\L{group.Name}}}";
                    cell.Alignment = CellAlignment.MiddleCenter;
                    row++;
                }
                else
                {
                    table.InsertRows(row, 8, rows);
                }
                foreach (var item in group.Rows)
                {
                    cell = table.Cells[row, 0];
                    cell.TextString = item.PositionColumn;
                    cell.Alignment = CellAlignment.MiddleCenter;
                    cell.Borders.Horizontal.Margin = 0;

                    cell = table.Cells[row, 1];
                    cell.TextString = item.DocumentColumn;
                    cell.Alignment = CellAlignment.MiddleLeft;
                    cell.Borders.Horizontal.Margin = 1.5;

                    cell = table.Cells[row, 2];
                    cell.TextString = item.NameColumn;
                    cell.Alignment = CellAlignment.MiddleLeft;
                    cell.Borders.Horizontal.Margin = 1.5;

                    cell = table.Cells[row, 3];
                    cell.TextString = item.CountColumn;
                    cell.Alignment = CellAlignment.MiddleCenter;
                    cell.Borders.Horizontal.Margin = 0;

                    cell = table.Cells[row, 4];
                    cell.TextString = item.WeightColumn;
                    cell.Alignment = CellAlignment.MiddleCenter;

                    cell = table.Cells[row, 5];
                    cell.TextString = item.DescriptionColumn;
                    cell.Alignment = CellAlignment.MiddleCenter;

                    row++;
                }                
            }           

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table)
        {
            TableJig jigTable = new TableJig(table, scale, "\nВставка спецификации:");
            if (service.Ed.Drag(jigTable).Status == PromptStatus.OK)
            {
                var cs = service.Db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(table);
                service.Db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);
            }
        }
    }
}