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
        public static AutoCAD_PIK_Manager.LogAddin Log { get; private set; } = new AutoCAD_PIK_Manager.LogAddin("Plugin KR_MN_Acad ");

        public void Initialize()
        {
            AcadLib.LoadService.LoadSpecBlocks();
            AcadLib.LoadService.LoadNetTopologySuite();
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod("PIK", "KR-SpecMonolith", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecMonolithCommand()
        {
            Log.Info("Start Command: KR-SpecMonolith");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var DocLock = doc.LockDocument())
            {
                try
                {
                    //// Проверка дубликатов.
                    //AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация монолитных блоков                         
                    SpecService specService = new SpecService(new SpecMonolith());
                    specService.CreateSpec();
                    
                    Inspector.Show();                    
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                    {
                        Log.Error(ex, $"Command: KR-SpecMonolith. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Спецификация проемов
        /// </summary>
        [CommandMethod("PIK", "KR-SpecApertures", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecOpeningsCommand()
        {
            Log.Info("Start Command: KR-SpecApertures");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var DocLock = doc.LockDocument())
            {
                try
                {
                    //// Проверка дубликатов.
                    //AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация проекмов
                    SpecService specService = new SpecService(new SpecAperture());
                    specService.CreateSpec();
                    
                    Inspector.Show();
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                    {
                        Log.Error(ex, $"Command: KR-SpecApertures. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Спецификация отверстий
        /// </summary>
        [CommandMethod("PIK", "KR-SpecOpenings", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecOpenings()
        {
            Log.Info($"Start Command: KR-SpecOpenings");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var DocLock = doc.LockDocument())
            {
                try
                {
                    //// Проверка дубликатов.
                    //AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация отверстий
                    SpecService specService = new SpecService(new SpecOpenings());
                    specService.CreateSpec();
                    
                    Inspector.Show();                    
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                    {
                        Log.Error(ex, $"Command: SpecOpenings. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Нумерация свай
        /// </summary>
        [CommandMethod("PIK", "KR-PileNumbering", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void PileNumberingCommand()
        {
            Log.Info($"Start Command: KR-PileNumbering");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;            
            Editor ed = doc.Editor;
            try
            {
                //// Проверка дубликатов.
                //AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                Inspector.Clear();

                PileNumberingService pileNumbService = new PileNumberingService();
                pileNumbService.Numbering();

                ed.WriteMessage("\nНумерация выполнена.");

                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                {
                    Log.Error(ex, $"Command: KR-PileNumbering. Doc {doc.Name}");
                }
                ed.WriteMessage($"\nОшибка - {ex.Message}");
            }
        }

        /// <summary>
        /// Параметры свай
        /// </summary>
        [CommandMethod("PIK", "KR-PileOptions", CommandFlags.Modal)]
        public void PileOptionsCommand()
        {
            Log.Info($"Start Command: KR-PileOptions");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            try
            {
                var pileOpt = Model.Pile.PileOptions.Load();
                pileOpt = pileOpt.PromptOptions();
                pileOpt.Save();
                ed.WriteMessage("\nНастойки сохранены.");
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                {
                    Log.Error(ex, $"Command: KR-PileOptions. Doc {doc.Name}");
                }
                ed.WriteMessage($"\nОшибка - {ex.Message}");
            }
        }

        /// <summary>
        /// Нумерация свай
        /// </summary>
        [CommandMethod("PIK", "KR-PileCalc", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void PileCalcCommand()
        {
            Log.Info($"Start Command: KR-PileCalc");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            try
            {
                //// Проверка дубликатов.
                //AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                Inspector.Clear();

                // Выбор свай для нумерации
                var selblocks = ed.SelectBlRefs("Выбор блоков свай для нумерации");

                // фильтр блоков свай            
                var piles = Model.Pile.PileFilter.Filter(selblocks, Model.Pile.PileOptions.Load());

                // Проверка дубликатов
                AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check(piles.Select(p => p.IdBlRef));

                Model.Pile.Calc.PileCalcService pileCalcService = new Model.Pile.Calc.PileCalcService();
                pileCalcService.Calc(piles);

                Inspector.Show();
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.Contains(AcadLib.General.CanceledByUser))
                {
                    Log.Error(ex, $"Command: KR-PileCalc. Doc {doc.Name}");
                }
                ed.WriteMessage($"\nОшибка - {ex.Message}");
            }            
        }

        public void Terminate()
        {
        }
    }
}