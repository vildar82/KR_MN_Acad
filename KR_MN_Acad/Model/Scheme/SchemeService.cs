using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Сервис расчета спецификации материалов к схемам армирования
    /// </summary>
    public class SchemeService
    {
        public Document Doc { get; set; }
        public Database Db { get; set; }
        public Editor Ed { get; set; }

        public SchemeOptions Options { get; set; }
        public List<ObjectId> IdBlRefs { get; set; }
        public List<SchemeBlock> Blocks { get; set; }
        public List<SpecGroup> Groups { get; set; }

        public SchemeService(SchemeOptions options)
        {
            Doc = Application.DocumentManager.MdiActiveDocument;
            Ed = Doc.Editor;
            Db = Doc.Database;
            Options = options;
        }

        /// <summary>
        /// Расчет и нумерация позиций в блоках схемы стен армирования
        /// </summary>
        public void Numbering()
        {
            // Выбор блоков
            IdBlRefs = SelectBlocks();
            // Определение блоков схемы армирования
            Blocks = FilterBlocks();
            // Калькуляция всех элементов
            Groups = Calculate(true);
            // Заполнение позиций в блоках
            NumberingBlocks();            
        }

        /// <summary>
        /// Спецификация материалов 
        /// </summary>
        public void Spec()
        {
            // Выбор блоков
            IdBlRefs = SelectBlocks();
            // Определение блоков схемы армирования
            Blocks = FilterBlocks();
            // Калькуляция всех элементов
            Groups = Calculate(false);
            // Проверка позиций
            CheckPositions();

            // Создание спецификации.
            SpecTable table = new SpecTable(this);
            table.CreateTable();
        }


        private void NumberingBlocks()
        {
            // Заполнение позиций в блоках
            using (var t = Db.TransactionManager.StartTransaction())
            {
                foreach (var item in Blocks)
                {
                    item.Numbering();
                }
                t.Commit();
            }
        }

        /// <summary>
        /// Выбор блоков и получение списка IdBlRefs 
        /// </summary>
        private List<ObjectId> SelectBlocks()
        {
            return SelectService.Select();
        }

        /// <summary>
        /// Фильтр выбранных блоков и определение блоков схемы армирования в соотв с настройками
        /// </summary>
        private List<SchemeBlock> FilterBlocks()
        {
            var blocks = new List<SchemeBlock>();
            var ids = IdBlRefs;
            if (ids == null || ids.Count == 0) return blocks;
            var db = Db;
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var idEnt in ids)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                    if (blRef == null) continue;

                    string blName = blRef.GetEffectiveName();

                    SchemeBlock block = SchemeBlockFactory.CreateBlock(blRef, blName, this);
                    if (block == null)
                    {
                        Ed.WriteMessage($"\nПропущен блок '{blName}'");
                        continue;
                    }
                    block.Calculate();
                    if (block.Error == null)
                        blocks.Add(block);
                    else
                        Inspector.Errors.Add(block.Error);
                }
                t.Commit();
            }
            return blocks;
        }

        /// <summary>
        /// Подсчет элементов схемы армирования
        /// </summary>
        private List<SpecGroup> Calculate(bool isNumbering)
        {
            List<SpecGroup> groups = new List<SpecGroup>();

            // Все элементы 
            List<IElement> elems = new List<IElement>();
            foreach (var block in Blocks)
            {
                elems.AddRange(block.GetElements());
            }
            // Группировка элементов по типам
            var elemTypes = elems.GroupBy(g => g.Type).OrderBy(g => g.Key);
            foreach (var type in elemTypes)
            {
                SpecGroup group = new SpecGroup(type);
                group.Calculate(isNumbering);
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// Проверка позиций
        /// </summary>
        private void CheckPositions()
        {
            foreach (var group in Groups)
            {
                if (!group.HasPosition) continue;
                // все элементы в строчке группы должны быть с одной позицией
                foreach (var row in group.Rows)
                {
                    var posGroups = row.Elements.OrderBy(e => e.PositionInBlock).GroupBy(e => e.PositionInBlock);
                    var firstPos = posGroups.First();
                    foreach (var errPos in posGroups.Skip(1))
                    {
                        var firstElem = errPos.First();
                        // Элементы с ошибочной позицией
                        string err = $"Ошибка позиции элемента '{firstElem.FriendlyName}' в блоке '{firstElem.Block.BlName}':" +
                            $" стоит позиция '{firstElem.PositionInBlock}', но уже задана позиция '{firstPos.Key}' для этого элемента в другом блоке.";
                        foreach (var elem in errPos)
                        {
                            Inspector.AddError(err, elem.Block.IdBlref, System.Drawing.SystemIcons.Error);
                        }
                    }
                }
            }
        }
    }
}