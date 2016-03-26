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

        public HightMarkTable(List<HightMarkRow> hmRows)
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;
            this.options = PileCalcService.PileOptions;
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
            table.TableStyle = db.GetTableStylePIK("ПИК100"); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется
            if (!string.IsNullOrEmpty(options.TableLayer))
            {
                table.LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(options.TableLayer));
            }

            int rows = hmRows.Count + 2;
            table.SetSize(rows, 6);
            table.SetRowHeight(800);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 500;
            rowTitle.TextString = "ТАБЛИЦА ОТМЕТОК СВАЙ";

            // столбец Условн обозн.
            var col = table.Columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 1500;
            // столбец Номеров свай.
            col = table.Columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 7000;
            // столбец Верха сваи после забивки.
            col = table.Columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 2500;
            // столбец Верха сваи после срубки.
            col = table.Columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 2500;
            // столбец Низ ростверка.
            col = table.Columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 2500;
            // столбец Острие сваи.
            col = table.Columns[5];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 2500;

            // Заголовок Условн обозн
            var cellColName = table.Cells[1, 0];
            cellColName.TextString = "Условн. обозн.";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Номер сваи
            cellColName = table.Cells[1, 1];
            cellColName.TextString = "Номера свай на схеме";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Верх сваи после забивки
            cellColName = table.Cells[1, 2];
            cellColName.TextString = "Верх сваи после забивки, м";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Верх сваи после срубки
            cellColName = table.Cells[1, 3];
            cellColName.TextString = "Верх сваи после срубки, м";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Низ ростверка
            cellColName = table.Cells[1, 4];
            cellColName.TextString = "Низ ростверка, м";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Отметка острия сваи
            cellColName = table.Cells[1, 5];
            cellColName.TextString = "Отметка острия сваи, м";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;                        

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 1500;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            int row = 2;
            foreach (var hmr in hmRows)
            {
                //table.Cells[row, 0].TextString = hmr.View;
                table.Rows[row].TextHeight = 250;

                var cellBlock = table.Cells[row, 0];                
                cellBlock.BlockTableRecordId = hmr.IdBtr;
                cellBlock.SetBlockAttributeValue(hmr.IdAtrDefPos, "");
                var blockContent = cellBlock.Contents[0];
                blockContent.IsAutoScale = false;
                blockContent.Scale = 1;                                                
                
                table.Cells[row, 1].TextString = hmr.Nums;
                table.Cells[row, 2].TextString = hmr.TopPileAfterBeat.ToString("0.000");
                table.Cells[row, 3].TextString = hmr.TopPileAfterCut.ToString("0.000");
                table.Cells[row, 4].TextString = hmr.BottomGrillage.ToString("0.000");
                table.Cells[row, 5].TextString = hmr.PilePike.ToString("0.000");
                row++;             
            }
            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table)
        {  
            TableJig jigTable = new TableJig(table, 1, "Вставка таблицы отметок свай");
            if (ed.Drag(jigTable).Status == PromptStatus.OK)
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(table);
                db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);
            }
        }
    }
}
