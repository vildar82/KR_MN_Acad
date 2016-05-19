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
        const string commandSpecMonolith = "KR-SpecMonolith";
        const string commandSpecApertures = "KR-SpecApertures";
        const string commandSpecOpenings = "KR-SpecOpenings";
        const string commandSpecSlabOpenings = "KR-SpecSlabOpenings";
        const string commandMonolithNumbering = "KR-MonolithNumbering";
        const string commandAperturesNumbering = "KR-AperturesNumbering";
        const string commandOpeningsNumbering = "KR-OpeningsNumbering";
        const string commandSlabOpeningsNumbering = "KR-SlabOpeningsNumbering";        
        const string commandPileNumbering = "KR-PileNumbering";
        const string commandPileOptions = "KR-PileOptions";
        const string commandPileCalc = "KR-PileCalc";        

        public void Initialize()
        {
            AcadLib.LoadService.LoadSpecBlocks();
            AcadLib.LoadService.LoadNetTopologySuite();
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod(groupPik, commandSpecMonolith, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecMonolithCommand()
        {
            AcadLib.CommandStart.Start(doc =>
            {                
                Inspector.Clear();
                // Спецификация монолитных блоков                         
                SpecService specService = new SpecService(new SpecMonolith());
                specService.CreateSpec();
                Inspector.Show();
            });            
        }

        /// <summary>
        /// Спецификация проемов - Дверных и Оконных
        /// </summary>
        [CommandMethod(groupPik, commandSpecApertures, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecAperturesCommand()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                // Спецификация проекмов
                SpecService specService = new SpecService(new SpecAperture());
                specService.CreateSpec();
                Inspector.Show();
            });                
        }

        /// <summary>
        /// Спецификация отверстий в стене
        /// </summary>
        [CommandMethod(groupPik, commandSpecOpenings, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecOpenings()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                // Спецификация отверстий
                SpecService specService = new SpecService(new SpecOpenings());
                specService.CreateSpec();
                Inspector.Show();
            });            
        }

        /// <summary>
        /// Спецификация отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, commandSpecSlabOpenings, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecSlabOpenings()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                // Спецификация отверстий
                SpecService specService = new SpecService(new SpecSlabOpenings());
                specService.CreateSpec();
                Inspector.Show();                                
            });
        }

        /// <summary>
        /// Нумерация блоков монолита
        /// </summary>
        [CommandMethod(groupPik, commandMonolithNumbering, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void MonolithNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                SpecService specService = new SpecService(new SpecMonolith());
                specService.Numbering();
                Inspector.Show();
            });
        }

        /// <summary>
        /// Нумерация блоков проемов
        /// </summary>
        [CommandMethod(groupPik, commandAperturesNumbering, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void AperturesNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                SpecService specService = new SpecService(new SpecAperture());
                specService.Numbering();
                Inspector.Show();
            });
        }

        /// <summary>
        /// Нумерация блоков отверстий в стене
        /// </summary>
        [CommandMethod(groupPik, commandOpeningsNumbering, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void OpeningsNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                SpecService specService = new SpecService(new SpecOpenings());
                specService.Numbering();
                Inspector.Show();
            });
        }

        /// <summary>
        /// Нумерация блоков отверстий в плите
        /// </summary>
        [CommandMethod(groupPik, commandSlabOpeningsNumbering, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SlabOpeningsNumbering()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                SpecService specService = new SpecService(new SpecSlabOpenings());
                specService.Numbering();
                Inspector.Show();
            });
        }        

        /// <summary>
        /// Нумерация свай
        /// </summary>
        [CommandMethod(groupPik, commandPileNumbering, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void PileNumberingCommand()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                PileNumberingService pileNumbService = new PileNumberingService();
                pileNumbService.Numbering();
                doc.Editor.WriteMessage("\nНумерация выполнена.");
                Inspector.Show();
            });           
        }

        /// <summary>
        /// Параметры свай
        /// </summary>
        [CommandMethod(groupPik, commandPileOptions, CommandFlags.Modal)]
        public void PileOptionsCommand()
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
        /// Нумерация свай
        /// </summary>
        [CommandMethod(groupPik, commandPileCalc, CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void PileCalcCommand()
        {
            AcadLib.CommandStart.Start(doc =>
            {
                Inspector.Clear();
                // Выбор свай для нумерации
                var selblocks = doc.Editor.SelectBlRefs("Выбор блоков свай для нумерации");
                // фильтр блоков свай            
                var piles = Model.Pile.PileFilter.Filter(selblocks, Model.Pile.PileOptions.Load());
                // Проверка дубликатов
                AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));
                // Расчет свай
                Model.Pile.Calc.PileCalcService pileCalcService = new Model.Pile.Calc.PileCalcService();
                pileCalcService.Calc(piles);
                // Вывод ошибок если есть.
                Inspector.Show();
            });            
        }

        public void Terminate()
        {
        }
    }
}