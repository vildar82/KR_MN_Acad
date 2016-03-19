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
        public static List<KeyValuePair<Point3d, Pile>> Filter(List<ObjectId> selblocks, PileOptions pileOptions)
        {
            List<KeyValuePair<Point3d, Pile>> resVal = new List<KeyValuePair<Point3d, Pile>>();
            foreach (var idBlRef in selblocks)
            {
                using (var blRef = idBlRef.Open( OpenMode.ForRead, false, true) as BlockReference)
                {
                    string blName = blRef.GetEffectiveName();
                    if (Regex.IsMatch(blName, pileOptions.PileBlockNameMatch))
                    {
                        Pile pile = new Pile(blRef, blName, pileOptions);
                        var checkResult = pile.Check();
                        if (checkResult.Success)
                        {
                            resVal.Add(new KeyValuePair<Point3d, Pile>(blRef.Position, pile));
                        }
                        else
                        {
                            Inspector.AddError($"В блоке сваи '{blName}' обнаружены ошибки: {checkResult.Error}", 
                                blRef,System.Drawing.SystemIcons.Error);
                        }
                    }
                }
            }
            return resVal;
        }
    }
}