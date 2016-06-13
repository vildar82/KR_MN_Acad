using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Errors;
using AcadLib.Jigs;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
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
        public List<ISchemeBlock> Blocks { get; set; }
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
            // Проверка правил расположение блоков
            //CheckRules();               
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
            // Проверка правил расположение блоков
            //CheckRules();
            // Калькуляция всех элементов
            Groups = Calculate(false);
            // Проверка позиций
            CheckPositions();
            // Создание спецификаций.
            SpecTable spec = new SpecTable(this);
            var tableSpec = spec.CreateTable();
            // Создание ведомости расхода стали
            BillService bill = new BillService(this);
            var tableBill = bill.CreateTable();

			using (var t = Db.TransactionManager.StartTransaction())
			{
				var cs = Db.CurrentSpaceId.GetObject( OpenMode.ForWrite) as BlockTableRecord;
				List<ObjectId> idsTable = new List<ObjectId> ();
				var scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(Db);

				tableSpec.TransformBy(Matrix3d.Scaling(scale, tableSpec.Position));
				cs.AppendEntity(tableSpec);
				t.AddNewlyCreatedDBObject(tableSpec, true);
				idsTable.Add(tableSpec.Id);

				tableBill.TransformBy(Matrix3d.Scaling(scale, tableBill.Position));
				cs.AppendEntity(tableBill);
				t.AddNewlyCreatedDBObject(tableBill, true);
				idsTable.Add(tableBill.Id);
				tableBill.Position = new Point3d(tableSpec.Position.X, tableSpec.Position.Y - tableSpec.Height - 10 * scale, 0);

				if (!DragSel.Drag(Ed, idsTable.ToArray(), Point3d.Origin))
				{
					foreach (var id in idsTable)
					{
						var ent = id.GetObject( OpenMode.ForWrite);
						ent.Erase();
					}
				}					
				t.Commit();
			}
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

        private void CheckRules()
        {
            // Проверка правил расположения блоков и т.п.
            Options.Rule.Check(Blocks);
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
        private List<ISchemeBlock> FilterBlocks()
        {
            var blocks = new List<ISchemeBlock>();
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

                    ISchemeBlock block = SchemeBlockFactory.CreateBlock(blRef, blName, this);
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
            if (blocks.Count == 0)
            {                
                throw new Exception($"\nБлоки для схемы не определены.");
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