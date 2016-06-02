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
            Calculate();
        }        

        /// <summary>
        /// Выбор блоков и получение списка IdBlRefs 
        /// </summary>
        public List<ObjectId> SelectBlocks ()
        {
            return SelectService.Select();
        }    
        
        /// <summary>
        /// Фильтр выбранных блоков и определение блоков схемы армирования в соотв с настройками
        /// </summary>
        public List<SchemeBlock> FilterBlocks ()
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

                    SchemeBlock block = SchemeBlockFactory.CreateBlock(blRef, blName);
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
        private List<GroupScheme> Calculate()
        {
            List<GroupScheme> groups = new List<GroupScheme>();

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
                GroupScheme group = new GroupScheme(type);
            }

            return groups;
        }
    }
}
