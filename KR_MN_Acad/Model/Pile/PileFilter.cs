using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace KR_MN_Acad.Model.Pile
{
    public static class PileFilter
    {
        public static List<Pile> Filter(List<ObjectId> selblocks, PileOptions pileOptions, bool checkNum)
        {
            var resVal = new List<Pile>();
            var db = HostApplicationServices.WorkingDatabase;
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var idBlRef in selblocks)
                {
                    using (var blRef = idBlRef.GetObject(OpenMode.ForRead, false, true) as BlockReference)
                    {
                        var blName = blRef.GetEffectiveName();
                        if (Regex.IsMatch(blName, pileOptions.PileBlockNameMatch))
                        {
                            var pile = new Pile(blRef, blName);
                            if (pile.Error == null)
                            {
                                resVal.Add(pile);
                            }
                            else
                            {
                                Inspector.AddError(pile.Error);
                            }
                        }
                    }
                }
                t.Commit();
            }
            return resVal;
        }
    }
}