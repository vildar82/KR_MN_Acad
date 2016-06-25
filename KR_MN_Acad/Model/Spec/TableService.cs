using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Общий табличный сервис
    /// </summary>
    public abstract class TableService : ITableService
    {
        protected AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;
        protected abstract Database Db { get; set; }
        protected virtual string Layer { get; set; } = "КР_Таблицы";
        protected abstract int NumColumns { get; set; }
        protected abstract int NumRows { get; set; }
        protected abstract string Title { get; set; }

        public abstract void CalcRows (List<ISpecBlock> blocks);
        public abstract Table CreateTable ();
        public abstract void Numbering (List<ISpecBlock> blocks);

        protected abstract void SetColumnsAndCap (ColumnsCollection columns);
        protected abstract void FillCells (Table table);

        /// <summary>
        /// перед вызовом необходимо заполнить свойства - Title, NumRows, NumColumns
        /// </summary>
        /// <returns></returns>
        protected Table GetTable ()
        {
            Table table = new Table();
            table.SetDatabaseDefaults(Db);
            table.TableStyle = Db.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется            

            if (!string.IsNullOrEmpty(Layer))
            {
                var layerId = AcadLib.Layers.LayerExt.GetLayerOrCreateNew(new AcadLib.Layers.LayerInfo(Layer));
                table.LayerId = layerId;
            }            

            table.SetSize(NumRows, NumColumns);
            table.SetBorders(LineWeight.LineWeight050);
            table.SetRowHeight(8);

            // Название таблицы
            var rowTitle = table.Cells[0, 0];
            rowTitle.Alignment = CellAlignment.MiddleCenter;
            rowTitle.TextHeight = 5;
            rowTitle.TextString = Title;

            // Заполнение шапки столбцов и их ширин
            SetColumnsAndCap(table.Columns);            

            // Строка заголовков столбцов
            var rowHeaders = table.Rows[1];
            rowHeaders.Height = 15;
            var lwBold = rowHeaders.Borders.Top.LineWeight;
            rowHeaders.Borders.Bottom.LineWeight = lwBold;

            // Заполнение строк
            FillCells(table);            

            var lastRow = table.Rows.Last();
            lastRow.Borders.Bottom.LineWeight = lwBold;

            table.GenerateLayout();
            return table;
        }        
    }
}
