using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    /// <summary>
    /// Нумерация свай
    /// </summary>
    public class PileNumberingService
    {
        public Document Doc { get; private set; }
        public Database Db { get; private set; }
        public Editor Ed { get; private set; }
        private Options NumberingOptions { get; set; }
        public PileOptions PileOptions { get; set; }

        public PileNumberingService()
        {
            Doc = Application.DocumentManager.MdiActiveDocument;
            Db = Doc.Database;
            Ed = Doc.Editor;
        }

        /// <summary>
        /// Нумерация свай
        /// </summary>
        public void Numbering()
        {
            // Форма настроек нумерации свай - порядок нумерации, имя блока сваи, имя атрибута номера сваи.
            NumberingOptions = new Options();
            NumberingOptions.LoadDefault();
            NumberingOptions = NumberingOptions.PromptOptions();
            PileOptions = new PileOptions() { PileBlockNameMatch = NumberingOptions.PileBlockNameMatch, PileAtrPos = NumberingOptions.PileAttrPos };

            // Выбор свай для нумерации
            var selblocks = Ed.SelectBlRefs("Выбор блоков свай для нумерации");

            // фильтр блоков свай            
            var piles = PileFilter.Filter(selblocks, PileOptions);

            // Сортировка             
            var pilesSort = PileSorting.Sort(piles, NumberingOptions);

            // Перенумерация
            PileNumbering.Num(pilesSort, Db, NumberingOptions.PileStartNum);
        }
    }
}