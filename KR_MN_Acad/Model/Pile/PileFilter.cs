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
        public static List<Pile> Filter(List<ObjectId> selblocks, PileOptions pileOptions)
        {
            List<Pile> resVal = new List<Pile>();
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
                            resVal.Add(pile);
                            if (!string.IsNullOrEmpty(pile.Error))
                            {
                                Inspector.AddError($"В блоке сваи '{blName}' обнаружены ошибки: {pile.Error}",
                                blRef, System.Drawing.SystemIcons.Error);
                            }                            
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