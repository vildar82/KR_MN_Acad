using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.ArmWall.Blocks;
using KR_MN_Acad.Spec.Constructions.Elements;

namespace KR_MN_Acad.Spec.Constructions.Blocks
{
    /// <summary>
    /// Колонна как конструцкция - одиночная запись в групповой спецификации
    /// </summary>
    public class ColumnSquareSmallConstr : ColumnSquareSmallBlock
    {
        private List<ISpecElement> Elementary; // элементы конструкции
        private const string propMark = "МАРКА";
        public Elements.Column Column { get; set; }

        public override List<ISpecElement> Elements { get; } = new List<ISpecElement>();

        public ColumnSquareSmallConstr (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        public override void Calculate ()
        {
            base.Calculate();            
            // Нумерация элементов в конструкции
            SpecGroup.SpecGroupService service = new SpecGroup.SpecGroupService (Block.IdBlRef.Database);
            service.Numbering(new List<ISpecBlock>() { this });
            base.Numbering();
            Elementary = Elements;

            string mark = Block.GetPropValue<string> (propMark);
            Column = new Elements.Column(Side, Side, Height, mark, this, Elementary);
            Column.Calc();
            Elements.Clear();
            AddElement(Column);
        }

        public override void Numbering ()
        {            
            Block.FillPropValue(propMark, Column.Mark);
        }
    }       
}
