using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Model.Pile.Calc.HightMark
{
    public class HightMarkTable
    {
        PileOptions options;
        Database db;
        Editor ed;
        Document doc;
        List<HightMarkRow> hmRows;

        public HightMarkTable(PileOptions options, List<HightMarkRow> hmRows)
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;
            this.options = options;
            this.hmRows = hmRows;
        }

        /// <summary>
        /// Создание таблицы спецификации блоков, с запросом выбора блоков у пользователя.
        /// Таблица будет вставлена в указанное место пользователем в текущем пространстве.
        /// </summary>
        public void CreateTable()
        {
            using (var t = db.TransactionManager.StartTransaction())
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
            table.SetDatabaseDefaults(db);
            table.TableStyle = db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется
            if (!string.IsNullOrEmpty(options.TableLayer))
            {
                table.LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(options.TableLayer));
            }

            int rows = hmRows.Count + 2;
            table.SetSize(rows, 6);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 5;
            rowTitle.TextString = "ТАБЛИЦА ОТМЕТОК СВАЙ";

            // столбец Условн обозн.
            var col = table.Columns[0];
            col.Width = 20;
            // столбец Номеров свай.
            col = table.Columns[1];
            col.Width = 50;
            // столбец Верха сваи после забивки.
            col = table.Columns[2];
            col.Width = 30;
            // столбец Верха сваи после срубки.
            col = table.Columns[3];
            col.Width = 30;
            // столбец Низ ростверка.
            col = table.Columns[4];
            col.Width = 30;
            // столбец Острие сваи.
            col = table.Columns[5];
            col.Width = 30;

            // Заголовок Условн обозн
            var cellColName = table.Cells[1, 0];
            cellColName.TextString = "Условн. обозн.";
            cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Номер сваи
            cellColName = table.Cells[1, 1];
            cellColName.TextString = "Номера свай на схеме";
            cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Верх сваи после забивки
            cellColName = table.Cells[1, 2];
            cellColName.TextString = "Верх сваи после забивки (м.)";
            cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Верх сваи после срубки
            cellColName = table.Cells[1, 3];
            cellColName.TextString = "Верх сваи после срубки (м.)";
            cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Низ ростверка
            cellColName = table.Cells[1, 4];
            cellColName.TextString = "Низ ростверка (м.)";
            cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Отметка острия сваи
            cellColName = table.Cells[1, 5];
            cellColName.TextString = "Отметка острия сваи (м.)";
            cellColName.Alignment = CellAlignment.MiddleCenter;                        

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 15;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            int row = 2;
            foreach (var hmr in hmRows)
            {
                table.Cells[row, 0].TextString = hmr.View;
                table.Cells[row, 1].TextString = hmr.Nums;
                table.Cells[row, 2].TextString = hmr.TopPileAfterBeat.ToString();
                table.Cells[row, 3].TextString = hmr.TopPileAfterCut.ToString();
                table.Cells[row, 4].TextString = hmr.BottomGrillage.ToString();
                table.Cells[row, 5].TextString = hmr.PilePike.ToString();                
            }
            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table)
        {  
            TableJig jigTable = new TableJig(table, 1 / db.Cannoscale.Scale, "Вставка таблицы отметок свай");
            if (ed.Drag(jigTable).Status == PromptStatus.OK)
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(table);
                db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);
            }
        }
    }
}
