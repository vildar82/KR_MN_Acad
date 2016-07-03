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

namespace KR_MN_Acad.Spec.Bill
{
    /// <summary>
    /// Ведомость расхода стали
    /// </summary>
    public class BillTable
    {   
        BillRow data;
        Database db;
        BillService service;

        readonly int rowTitleIndex = 1;
        readonly int rowGroupIndex = 2;
        readonly int rowMarkIndex = 3;
        readonly int rowGostIndex = 4;
        readonly int rowNameIndex = 5;        

        public BillTable(BillService service)
        {
            db = service.Db;                     
            data = service.Row;
            this.service = service;                       
        }

        public Table CreateTable ()
        {
            // Создание таблицы
            return getTable();
        }

        private Table getTable ()
        {
            Table table = new Table();
            table.SetDatabaseDefaults(db);
            table.TableStyle = db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется            
            table.LayerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(KRHelper.LayerTable));            

            table.SetSize(7, 1);
            table.SetRowHeight(8);
            table.SetColumnWidth(15);

            // Добавление столбцов в таблицу
            BillColumn.AddColumns(table, data, this, service);
            // Заполнение расходов
            int row = rowNameIndex+1;
            // Строка в ведомости расхода стали                
            table.Cells[row, 0].TextString = data.Name;

            //foreach (var item in data.Cells)
            //{
            //    var colBil = BillColumn.GetColumn(item);
            //    table.Cells[row, colBil.Index].TextString = item.Amount.ToString();
            //}

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
            var lwBold = rowHeaders.Borders.Top.LineWeight.Value;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            table.SetBorders(lwBold);

            var rowItem = table.Rows[rowTitleIndex];
            rowItem.Borders.Bottom.LineWeight = lwBold;
            rowItem = table.Rows[rowGroupIndex];
            rowItem.Borders.Bottom.LineWeight = lwBold;            
            rowItem = table.Rows[rowMarkIndex];
            rowItem.Borders.Bottom.LineWeight = lwBold;
            rowItem = table.Rows[rowGostIndex];
            rowItem.Borders.Bottom.LineWeight = lwBold;
            rowItem = table.Rows[rowNameIndex];
            rowItem.Borders.Bottom.LineWeight = lwBold;

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }        

        class BillColumn
        {
            static Dictionary<BillCell, BillColumn> cols;            
            BillCell cell;
            IBillMaterial mater;
            public int Index;            

            public BillColumn(BillCell item, int index)
            {
                cell = item;
                mater = item.BillMaterial;
                this.Index = index;
            }            

            /// <summary>
            /// Добавление столбцов в ьаблицу
            /// </summary>
            public static void AddColumns(Table table,BillRow row, BillTable spec, BillService service)
            {
                // группировка по ячейкам
                var cells = row.Cells.GroupBy(g => g).OrderBy(o=>o.Key);                
                cols = new Dictionary<BillCell, BillColumn>();

                // Марка конструкции
                mergeRows(table, 0, 1, spec.rowNameIndex);
                table.Cells[1, 0].TextString = "Марка конструкции";
                table.Columns[0].Width = 40;

                // группировка по заголовку
                int curTitleColFirst = 1;
                int curTitleColLast =1;                
                foreach (var title in cells.GroupBy(g=>g.Key.BillMaterial.BillTitle))
                {
                    int curGroupColFirst = curTitleColLast;
                    int curGroupColLast =0;
                    // группировка по группе материалов - Арматура класса, Прокат марки
                    foreach (var group in title.GroupBy(g=>g.Key.BillMaterial.BillGroup))
                    {
                        int curMarkColFirst = curGroupColFirst;
                        int curMarkColLast =0;
                        // группировка по марке 
                        foreach (var mark in group.GroupBy(g=>g.Key.BillMaterial.BillMark))
                        {
                            int curGostColFirst = curMarkColFirst;
                            int curGostColLast =0;
                            // группировка по госту
                            foreach (var gost in mark.GroupBy(g=>g.Key.BillMaterial.BillGOST))
                            {
                                // вставка столбцов для набора ячеек одного госта + итого
                                var countCell = gost.Count();
                                curGostColLast = curGostColFirst + countCell;
                                table.InsertColumns(curGostColFirst, 15, countCell + 1);                                

                                // заполенение ячеек
                                int curNameCol = curGostColFirst;
                                foreach (var name in gost)
                                {
                                    table.Cells[spec.rowNameIndex, curNameCol].TextString = name.Key.BillMaterial.BillName;
                                    table.Cells[spec.rowNameIndex+1, curNameCol].TextString = name.Key.Amount.ToString();
                                    BillColumn col = new BillColumn(name.Key, curNameCol);
                                    cols.Add(name.Key, col);
                                    curNameCol++;
                                }
                                // итого
                                table.Cells[spec.rowNameIndex, curNameCol].TextString = "Итого";
                                table.Cells[spec.rowNameIndex+1, curNameCol].TextString = gost.Sum(s => s.Key.Amount).ToString();
                                // Объединение строки госта
                                mergeColumns(table, spec.rowGostIndex, curGostColFirst, curGostColLast);
                                table.Cells[spec.rowGostIndex, curGostColFirst].TextString = gost.Key;
                                curGostColFirst = curGostColLast + 1;
                            }
                            curMarkColLast= curGostColLast;
                            // Объединение строки марки
                            mergeColumns(table, spec.rowMarkIndex, curMarkColFirst, curMarkColLast);
                            table.Cells[spec.rowMarkIndex, curMarkColFirst].TextString = mark.Key;
                            curMarkColFirst = curMarkColLast + 1;
                        }
                        curGroupColLast = curMarkColLast;
                        // Объединение строки группы
                        mergeColumns(table, spec.rowGroupIndex, curGroupColFirst, curGroupColLast);
                        table.Cells[spec.rowGroupIndex, curGroupColFirst].TextString = group.Key;
                        curGroupColFirst = curGroupColLast + 1;
                    }
                    curTitleColLast = curGroupColLast;
                    // Объединение строки заголовка
                    mergeColumns(table, spec.rowTitleIndex, curTitleColFirst, curTitleColLast);
                    table.Cells[spec.rowTitleIndex, curTitleColFirst].TextString = title.Key;
                    curTitleColFirst = curTitleColLast + 1;
                    curTitleColLast = curTitleColFirst;
                }                

                var colIndex = curTitleColLast;
                table.InsertColumns(colIndex, 15, 1);
                var mCells = CellRange.Create(table, 1, colIndex, spec.rowNameIndex, colIndex);
                table.MergeCells(mCells);
                table.Cells[1, colIndex].TextString = "Всего";
                table.Cells[spec.rowNameIndex + 1, colIndex].TextString = row.Cells.Sum(s=>s.Amount).ToString();

                // всего бетона
                var concretes = service.Elements.OfType<Concrete>().GroupBy(g=>g.ClassB).OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New);
                    //.FirstOrDefault(g => g.Type == GroupType.Materials)?.Rows.Where(r=>r.SomeElement is Concrete)?.GroupBy(g=>((Concrete)g.SomeElement).ClassB);
                if (concretes != null)
                {
                    foreach (var concrete in concretes)
                    {
                        colIndex++;
                        table.InsertColumns(colIndex, 15, 1);
                        mCells = CellRange.Create(table, 1, colIndex, spec.rowNameIndex, colIndex);
                        table.MergeCells(mCells);
                        string unitsConcrete = concrete.First().Units;// ((Concrete)concrete.First().SomeElement).Units;
                        table.Cells[1, colIndex].TextString = $"Расход бетона класса {concrete.Key}, {unitsConcrete}";
                        table.Cells[spec.rowNameIndex + 1, colIndex].TextString = concrete.Sum(c => c.Volume).ToString();
                    }
                }
            }            

            private static void mergeColumns (Table table, int row, int colIndexFirst, int colIndexLast, CellAlignment align = CellAlignment.MiddleCenter)
            {
                if (colIndexLast <= colIndexFirst) return;
                var mCells = CellRange.Create(table, row, colIndexFirst, row, colIndexLast);
                table.MergeCells(mCells);
                mCells.Alignment = align;
            }

            private static void mergeRows (Table table, int col, int rowIndexFirst, int rowIndexLast, CellAlignment align = CellAlignment.MiddleCenter)
            {
                if (rowIndexLast <= rowIndexFirst) return;
                var mCells = CellRange.Create(table, rowIndexFirst, col, rowIndexLast, col);
                table.MergeCells(mCells);
                mCells.Alignment = align;
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
