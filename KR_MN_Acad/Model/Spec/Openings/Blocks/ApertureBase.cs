using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec;
using KR_MN_Acad.Spec.Openings.Blocks;
using KR_MN_Acad.Spec.Openings.Elements;

namespace KR_MN_Acad.Openings.Blocks
{
    public abstract class ApertureBase : SpecBlock
    {
        protected string prefixMark;
        private const string propMark = "МАРКА";
        private const string propLength = "Ширина отв.";
        private const string propHeight = "Высота";
        private const string propElevation = "ОТМЕТКА_НИЗА";
        private const string propDesc = "ПРИМЕЧАНИЕ";

        protected string mark;
        protected int length;
        protected int height;
        protected double elev;
        protected string desc;

        Aperture opening;

        public ApertureBase (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        protected abstract Aperture GetAperture ();

        public override void Calculate ()
        {
            mark = Block.GetPropValue<string>(propMark);
            length = Block.GetPropValue<int>(propLength);
            height = Block.GetPropValue<int>(propHeight);
            elev = Block.GetPropValue<double>(propElevation);            
            desc = Block.GetPropValue<string>(propDesc, false);
            opening = GetAperture();
            AddElement(opening);
        }        

        public override void Numbering ()
        {
            // Запись марки в блок
            Block.FillPropValue(propMark, opening.Mark);
            // Обновление полей в блоке
            AcadLib.Field.UpdateField.Update(Block.IdBlRef);
        }
    }
}
