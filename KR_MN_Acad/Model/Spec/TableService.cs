using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using KR_MN_Acad.Spec.Elements;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Общий табличный сервис
    /// </summary>
    public abstract class TableService : ITableService
    {
        public static AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;

        protected List<ISpecElement> elements;        
        
        protected List<KeyValuePair<string, List<ISpecRow>>> groupRows;
        protected abstract Database Db { get; set; }
        protected virtual string Layer { get; set; } = KRHelper.LayerTable;
        protected abstract int NumColumns { get; set; }
        protected abstract int NumRows { get; set; }
        protected abstract string Title { get; set; }
        protected abstract void SetColumnsAndCap (ColumnsCollection columns);
        protected abstract void FillCells (Table table);
        protected abstract ISpecRow GetNewRow (string group, List<ISpecElement> list);
        protected abstract Dictionary<string, List<ISpecElement>> GroupsFirstForNumbering (IGrouping<GroupType, ISpecElement> indexTypeGroup);
        protected abstract Dictionary<string, List<ISpecElement>> GroupsSecondForNumbering (KeyValuePair<string, List<ISpecElement>> firstGroup);        

        /// <summary>
        /// Группировка элементов спецификации.
        /// Нумерация берется из объектов
        /// </summary>        
        public void CalcRows (List<ISpecElement> elements)
        {
            this.elements = elements;
            int numrows = 0;
            groupRows = new List<KeyValuePair<string, List<ISpecRow>>>();
            
            // группировка по именам групп
            var groupsGroup = elements.OrderBy(o => o.Index).GroupBy(g => g.Group).OrderBy(o=>o.Key.Index);
            foreach (var group in groupsGroup)
            {
                var rows = new List<ISpecRow>();
                // группировка по униеальности элементов
                var uniqElemGroup = group.GroupBy(g=>g).OrderBy(g=>g.Key.Mark, alpha);
                // Проверка уникальности марок элеметнов
                CheckUniqueMarks(uniqElemGroup);
                string groupName = group.First().Group.Name;
                if (!string.IsNullOrEmpty(groupName))
                    numrows++;
                foreach (var item in uniqElemGroup)
                {
                    var row = GetNewRow(groupName, item.ToList());
                    // Проверка одинаковости марки
                    CheckSomeMark(row.Elements);
                    rows.Add(row);
                    numrows++;
                }
                groupRows.Add(new KeyValuePair<string, List<ISpecRow>>(groupName, rows.ToList()));
            }
            NumRows = numrows + 2;
        }        

        /// <summary>
        /// Нумерация элементов
        /// </summary>
        /// <param name="blocks"></param>
        public void Numbering (List<ISpecElement> elements)
        {
            this.elements = elements;
            // Группировыка по именам групп
            var groupGroups = elements.GroupBy(g=>g.Group).OrderBy(o=>o.Key.Index);
            foreach (var group in groupGroups)
            {
                string groupName = group.First().Group.Name;

                int index = 1;
                var firstGroups = GroupsFirstForNumbering(group);

                foreach (var firstGroup in firstGroups)
                {
                    // Группировка по индексам - как располагать строки элементов в спецификации
                    var indexGroups = firstGroup.Value.GroupBy(g=>g.Index).OrderBy(o=>o.Key);
                    foreach (var indexGroup in indexGroups)
                    {
                        // группировка по назначению
                        var secGroups = GroupsSecondForNumbering(firstGroup);
                        if (secGroups.Skip(1).Any())
                        {
                            // Есть подгруппы вида - одинаковые размеры у отв но разное назначение - нумерация вида 1.1
                            int indexSecond = 1;
                            foreach (var secGroup in secGroups)
                            {
                                string indexSubgroup = index + "." + indexSecond;
                                var row = GetNewRow(groupName, secGroup.Value);
                                NumberingRow(row, indexSubgroup, index, indexSecond);
                                indexSecond++;
                            }
                        }
                        else
                        {
                            var row = GetNewRow(groupName, firstGroup.Value);
                            NumberingRow(row, index.ToString(), index, 0);
                        }
                    }
                    index++;
                }
            }
        }

        private void NumberingRow (ISpecRow row, string index, int indexFirst, int indexSecond)
        {
            string num =row.Elements.First().GetNumber(index);
            foreach (var item in row.Elements)
            {
                item.SetNumber(num, indexFirst, indexSecond);
            }
        }    

        /// <summary>
        /// Создание и заполнение тапбицы
        /// </summary>        
        public virtual Table CreateTable ()
        {
            var table = GetTable();
            return table;
        }

        private void CheckUniqueMarks (IOrderedEnumerable<IGrouping<ISpecElement, ISpecElement>> groups)
        {
            var markGroups = groups.GroupBy(g => g.Key.Mark, (k, g) =>
                new { Key = k, Error = g.Skip(1).Any(), Elements = g.SelectMany(s => s.Select(i => i)) }).
                Where(w=>w.Error);
            foreach (var item in markGroups)
            {
                foreach (var elem in item.Elements)
                {
                    Inspector.AddError($"Одинаковая марка у разных элементов - {elem.Mark}",
                        elem.SpecBlock.Block.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }
        }

        private void CheckSomeMark (List<ISpecElement> elements)
        {
            // Марка элементов должна быть одинаковой
            var groups = elements.GroupBy(g => g.Mark);
            if (groups.Skip(1).Any())
            {
                // Ошибка - разные марки
                foreach (var item in elements)
                {
                    Inspector.AddError($"Разная марка у одинаковых элементов: {item.GetParamInfo()}",
                        item.SpecBlock.Block.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }
        }

        /// <summary>
        /// Фильтрация всех элементов из блоков для спецификации
        /// </summary>        
        public virtual List<ISpecElement> FilterElements (List<ISpecBlock> blocks, bool isNumbering)
        {
            return blocks.SelectMany(b => b.Elements).ToList();
        }

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

        public virtual List<ISpecElement> GetElementsForBill (List<ISpecBlock> blocks)
        {
            return elements;
        }

        public virtual List<IDetail> GetDetails ()
        {
            var details = elements.OfType<IDetail>().GroupBy(g=>g.Mark).
                OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New).Select(s=>s.First()).ToList();
            return details;
        }        
    }
}
