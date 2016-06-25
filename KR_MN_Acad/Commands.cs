using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using KR_MN_Acad.Model.Pile.Numbering;
using KR_MN_Acad.Spec;
using SpecBlocks;

[assembly: CommandClass(typeof(KR_MN_Acad.Commands))]
[assembly: ExtensionApplication(typeof(KR_MN_Acad.Commands))]

namespace KR_MN_Acad
{
    public class Commands: IExtensionApplication
    {
        const string groupPik = AutoCAD_PIK_Manager.Commands.Group;                       

        public void Initialize()
        {
            AcadLib.LoadService.LoadSpecBlocks();
            AcadLib.LoadService.LoadNetTopologySuite();
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_MonolithSpec), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_MonolithSpec()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                // Спецификация монолитных блоков                         
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecMonolith());
                specService.CreateSpec();                
            });            
        }
        /// <summary>
        /// Нумерация блоков монолита
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_MonolithNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_MonolithNumbering ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecMonolith());
                specService.Numbering();
            });
        }

        /// <summary>
        /// Спецификация проемов - Дверных и Оконных
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_AperturesSpec), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_AperturesSpec ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                // Спецификация проекмов
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecAperture());
                specService.CreateSpec();             
            });                
        }
        /// <summary>
        /// Нумерация блоков проемов
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_AperturesNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_AperturesNumbering ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecAperture());
                specService.Numbering();
            });
        }

        /// <summary>
        /// Спецификация отверстий в стене
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_OpeningsSpec), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_OpeningsSpec ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                // Спецификация отверстий
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecOpenings());
                specService.CreateSpec();                
            });            
        }
        /// <summary>
        /// Нумерация блоков отверстий в стене
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_OpeningsNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_OpeningsNumbering ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecOpenings());
                specService.Numbering();
            });
        }

        /// <summary>
        /// Спецификация отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SlabOpeningsSpec), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SlabOpeningsSpec ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                // Спецификация отверстий
                //SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecSlabOpenings());
                //specService.CreateSpec();   
                Spec.SpecService spec = new Spec.SpecService (doc, new Spec.Slab.SlabOptions(doc.Database));
                spec.Spec();
            });
        }
        /// <summary>
        /// Нумерация блоков отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_SlabOpeningsNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_SlabOpeningsNumbering ()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                //SpecBlocks.SpecService specService = new SpecBlocks.SpecService(new SpecSlabOpenings());
                //specService.Numbering();    
                Spec.SpecService spec = new Spec.SpecService (doc, new Spec.Slab.SlabOptions(doc.Database));
                spec.Numbering();
            });
        }

        /// <summary>
        /// Нумерация свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileNumbering), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_PileNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                PileNumberingService pileNumbService = new PileNumberingService();
                pileNumbService.Numbering();
                doc.Editor.WriteMessage("\nНумерация выполнена.");                
            });           
        }

        /// <summary>
        /// Параметры свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileOptions), CommandFlags.Modal)]
        public void KR_PileOptions()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                var pileOpt = Model.Pile.PileOptions.Load();
                pileOpt = pileOpt.PromptOptions();
                pileOpt.Save();
                doc.Editor.WriteMessage("\nНастойки сохранены.");
            });            
        }

        /// <summary>
        /// Спецификация свай
        /// </summary>
        [CommandMethod(groupPik,nameof(KR_PileCalc), CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void KR_PileCalc()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                // Выбор свай для нумерации
                var selblocks = doc.Editor.SelectBlRefs("\nВыбор блоков свай:");
                // фильтр блоков свай            
                var piles = Model.Pile.PileFilter.Filter(selblocks, Model.Pile.PileOptions.Load(), true);
                // Проверка дубликатов
                AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));
                // Расчет свай
                Model.Pile.Calc.PileCalcService pileCalcService = new Model.Pile.Calc.PileCalcService();
                pileCalcService.Calc(piles);                
            });            
        }

        
        /// <summary>
        /// Схема армирования стен - Нумерация
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_ArmWallNumbering), CommandFlags.Modal)]
        public void KR_ArmWallNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                var service = new Scheme.SchemeService(Scheme.Wall.WallSchemeOptions.GetSchemeOptions());
                service.Numbering();
            });
        }

        /// <summary>
        /// Схема армирования стен - спецификация
        /// </summary>
        [CommandMethod(groupPik, nameof(KR_ArmWallSpec), CommandFlags.Modal)]
        public void KR_ArmWallSpec()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                var service = new Scheme.SchemeService(Scheme.Wall.WallSchemeOptions.GetSchemeOptions());
                service.Spec();
            });
        }

        public void Terminate()
        {
        }
    }
}