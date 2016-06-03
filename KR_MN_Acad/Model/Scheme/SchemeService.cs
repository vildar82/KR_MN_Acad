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
using KR_MN_Acad.ConstructionServices.Materials;
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
        public List<SchemeGroup> Groups { get; set; }

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
        public void Numbering ()
        {
            // Выбор блоков
            IdBlRefs = SelectBlocks();
            // Определение блоков схемы армирования
            Blocks = FilterBlocks();
            // Калькуляция всех элементов
            Groups = Calculate();
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
            Groups = Calculate();
            // Заполнение позиций в блоках
            NumberingBlocks();
            List<string> rty;
            
            // Создание спецификации.
            SchemeTable table = new SchemeTable(this);
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
        private List<ObjectId> SelectBlocks ()
        {
            return SelectService.Select();
        }    
        
        /// <summary>
        /// Фильтр выбранных блоков и определение блоков схемы армирования в соотв с настройками
        /// </summary>
        private List<SchemeBlock> FilterBlocks ()
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
        private List<SchemeGroup> Calculate()
        {
            List<SchemeGroup> groups = new List<SchemeGroup>();

            // Все элементы 
            List<IMaterial> elems = new List<IMaterial>();
            foreach (var block in Blocks)
            {
                elems.AddRange(block.GetMaterials());
            }
            // Группировка элементов по типам
            var elemTypes = elems.GroupBy(g => g.Type);
            foreach (var type in elemTypes)
            {
                SchemeGroup group = new SchemeGroup(type);
                groups.Add(group);
            }

            return groups;
        }
    }
}
