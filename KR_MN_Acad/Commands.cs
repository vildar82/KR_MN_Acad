using System;
using System.IO;
using System.Reflection;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using KR_MN_Acad.Spec;
using SpecBlocks;

[assembly: CommandClass(typeof(KR_MN_Acad.Commands))]
[assembly: ExtensionApplication(typeof(KR_MN_Acad.Commands))]

namespace KR_MN_Acad
{
    public class Commands : IExtensionApplication
    {
        public static AutoCAD_PIK_Manager.LogAddin Log { get; private set; } = new AutoCAD_PIK_Manager.LogAddin("Plugin KR_MN_Acad ");

        public void Initialize()
        {
            AcadLib.LoadService.LoadSpecBlocks();
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
                    // Проверка дубликатов.
                    AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация монолитных блоков                         
                    SpecService specService = new SpecService(new SpecMonolith());
                    specService.CreateSpec();

                    if (Inspector.HasErrors)
                    {
                        Inspector.Show();
                    }
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains("\nОтменено пользователем"))
                    {
                        Log.Error(ex, $"Command: KR-SpecMonolith. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod("PIK", "KR-SpecOpenings", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecOpeningsCommand()
        {
            Log.Info("Start Command: KR-SpecOpenings");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var DocLock = doc.LockDocument())
            {
                try
                {
                    // Проверка дубликатов.
                    AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация проекмов
                    SpecService specService = new SpecService(new SpecOpenings());
                    specService.CreateSpec();

                    if (Inspector.HasErrors)
                    {
                        Inspector.Show();
                    }
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains("\nОтменено пользователем"))
                    {
                        Log.Error(ex, $"Command: KR-SpecOpenings. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Спецификация монолитных блоков
        /// </summary>
        [CommandMethod("PIK", "KR-SpecHoles", CommandFlags.Modal | CommandFlags.NoPaperSpace | CommandFlags.NoBlockEditor)]
        public void SpecHoles()
        {
            Log.Info($"Start Command: {nameof(SpecHoles)}");
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (var DocLock = doc.LockDocument())
            {
                try
                {
                    // Проверка дубликатов.
                    AcadLib.Blocks.Dublicate.CheckDublicateBlocks.Check();
                    Inspector.Clear();

                    // Спецификация отверстий
                    SpecService specService = new SpecService(new SpecHoles());
                    specService.CreateSpec();

                    if (Inspector.HasErrors)
                    {
                        Inspector.Show();
                    }
                }
                catch (System.Exception ex)
                {
                    if (!ex.Message.Contains("\nОтменено пользователем"))
                    {
                        Log.Error(ex, $"Command: {nameof(SpecHoles)}. Doc {doc.Name}");
                    }
                    ed.WriteMessage($"\nОшибка - {ex.Message}");
                }
            }
        }

        public void Terminate()
        {
        }
    }
}