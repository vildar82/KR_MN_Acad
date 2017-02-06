using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Model.Pile.Calc.Spec
{
    public class SpecTable
    {
        PileOptions options;
        Database db;
        Editor ed;
        Document doc;
        List<SpecRow> specRows;

        public SpecTable(List<SpecRow> specRows)
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;
            this.options = PileCalcService.PileOptions;
            this.specRows = specRows;
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

            int rows = specRows.Count + 2;
            table.SetSize(rows, 7);
            table.SetBorders(LineWeight.LineWeight050);
            table.SetRowHeight(800);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 500;
            rowTitle.TextString = "Спецификация к схеме расположения свай";

            // столбец Условн обозн.
            var col = table.Columns[0];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 1500;            
            // столбец Номеров свай.
            col = table.Columns[1];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 6000;            
            // столбец Обозначения
            col = table.Columns[2];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 4500;            
            // столбец Наименование
            col = table.Columns[3];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 2500;            
            // столбец Кол
            col = table.Columns[4];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 1200;
            // столбец Масса
            col = table.Columns[5];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 1200;
            // столбец Примечания
            col = table.Columns[6];
            col.Alignment = CellAlignment.MiddleCenter;
            col.Width = 1600;

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
            // Заголовок Обозначения
            cellColName = table.Cells[1, 2];
            cellColName.TextString = "Обозначение";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Наименование
            cellColName = table.Cells[1, 3];
            cellColName.TextString = "Наименование";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Кол
            cellColName = table.Cells[1, 4];
            cellColName.TextString = "Кол-во, шт";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;
            // Заголовок Масса
            cellColName = table.Cells[1, 5];
            cellColName.TextString = "Масса ед., кг";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;                        
            // Заголовок Примечание
            cellColName = table.Cells[1, 6];
            cellColName.TextString = "Примеча-\nние";
            cellColName.TextHeight = 250;
            //cellColName.Alignment = CellAlignment.MiddleCenter;                        

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 1500;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            int row = 2;
            foreach (var sr in specRows)
            {
                table.Rows[row].TextHeight = 250;

                var cellBlock = table.Cells[row, 0];
                cellBlock.BlockTableRecordId = sr.IdBtr;
                cellBlock.SetBlockAttributeValue(sr.IdAtrDefPos, "");
                var blockContent = cellBlock.Contents[0];
                blockContent.IsAutoScale = false;
                blockContent.Scale = 1;

                
                table.Cells[row, 0].SetBlockAttributeValue(sr.IdAtrDefPos, "");
                table.Cells[row, 1].TextString = sr.Nums;
                table.Cells[row, 2].TextString = sr.DocLink;
                table.Cells[row, 3].TextString = sr.Name;
                table.Cells[row, 4].TextString = sr.Count.ToString();
                table.Cells[row, 5].TextString = sr.Weight.ToString();
                table.Cells[row, 6].TextString = sr.Description;
                row++;
            }
            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }

        private void insertTable(Table table)
        {
            TableJig jigTable = new TableJig(table, 1, "Вставка спецификации свай");
            if (ed.Drag(jigTable).Status == PromptStatus.OK)
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                cs.AppendEntity(table);
                db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);
            }
        }
    }
}
