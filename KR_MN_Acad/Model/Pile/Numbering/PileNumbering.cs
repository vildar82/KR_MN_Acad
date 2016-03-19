using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    static class PileNumbering
    {
        public static void Num(List<Pile> piles, Database db, int startNum)
        {
            using (var t = db.TransactionManager.StartTransaction())
            {
                int pos = startNum;
                foreach (var pile in piles)
                {
                    var atrPos = pile.PosAttrRef.IdAtr.GetObject(OpenMode.ForWrite, false, true) as AttributeReference;
                    atrPos.TextString = pos.ToString();
                    pos++;
                }
                t.Commit();
            }
        }
    }
}