using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;
using KR_MN_Acad.Spec.Materials;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    public class WindowBlock : ApertureBase
    {
        public const string BlockName = "КР_Арм_Стена_Окно";

        public WindowBlock (BlockReference blRef, string blName) : base(blRef, blName)
        {
            PropNameApertureHeight = "ВысотаОкна";
            PropNameApertureHeightAbove = "ВысотаПодоконника";
            PropNameAddArmHorCountTop = "КолГорАрмУсиленияСверху";
            PropNameAddArmHorCountBottom = "КолГорАрмУсиленияСнизу";
        }
    }
}
