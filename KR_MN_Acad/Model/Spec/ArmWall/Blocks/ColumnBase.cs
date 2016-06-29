using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;

namespace KR_MN_Acad.Spec.ArmWall
{
    /// <summary>
    /// Описание блока колонны квадратной до 400мм
    /// </summary>
    public abstract class ColumnBase : SpecBlock
    {
        /// <summary>
        /// Защитный слой бетона до центра арматуры
        /// </summary>
        protected const int a = 50;

        protected const string PropNameConcrete = "Бетон";
        protected const string PropNameOutline = "Выпуск";
        protected const string PropNameHeight = "Высота";
        protected const string PropNameArmVerticCount = "КолВертикАрм";
        protected const string PropNameArmVerticDiam = "ДиамВертикАрм";        
        protected const string PropNameShackleDiam = "ДиамХомута";
        protected const string PropNameShackleStep = "ШагХомута";
        protected const string PropNameArmVerticPos = "ПОЗВЕРТИКАРМ";
        protected const string PropNameArmVerticDesc = "ОПИСАНИЕВЕРТИКАРМ";
        protected const string PropNameShacklePos = "ПОЗХОМУТА";
        protected const string PropNameShackleDesc = "ОПИСАНИЕХОМУТА";

        public ColumnBase (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        /// <summary>
        /// Высота колонны
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Выпуск стержней по вертикали
        /// </summary>
        public int Outline { get; set; }
        /// <summary>
        /// Ширина колонны
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Толщина колонны
        /// </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// Бетон
        /// </summary>
        public ConcreteH Concrete { get; set; }
        /// <summary>
        /// Кол вертик стержней
        /// </summary>
        public int ArmVerticCount { get; set; }
        /// <summary>
        /// Вертикальные арматурные стержни
        /// </summary>
        public Bar ArmVertic { get; set; }
        /// <summary>
        /// Хомут
        /// </summary>
        public Shackle Shackle { get; set; }        

        /// <summary>
        /// Определение базовых параметров колонны - выпуск, высота, бетон и т.д.
        /// </summary>
        /// <param name="width">Ширина колонны</param>
        /// <param name="thickness">Толщина колонны</param>
        protected void DefineBaseFields(int width, int thickness, bool defaultShackle)
        {
            Width = width;
            Thickness = thickness;
            Height = Block.GetPropValue<int>(PropNameHeight);            
            Outline = Block.GetPropValue<int>(PropNameOutline);
            var classB = Block.GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(classB, Width, Thickness, Height, this);
            Concrete.Calc();            
            // Определние вертикальной арматуры            
            ArmVertic = defineVerticArm();            
            // Хомут
            if (defaultShackle)
            {
                Shackle = defineShackleByGab(width, thickness, Height, ArmVertic.Diameter, a, PropNameShackleDiam,
                    PropNameShacklePos, PropNameShackleStep);                
            }            
        }

        protected virtual void AddElements ()
        {
            AddElement(ArmVertic);
            AddElement(Shackle);            
            AddElement(Concrete);
        }

        /// <summary>
        /// Вертикальные отдельные стержени
        /// </summary>
        /// <param name="count">Кол</param>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам поз</param>        
        protected Bar defineVerticArm ()
        {
            ArmVerticCount = Block.GetPropValue<int>(PropNameArmVerticCount);
            return defineBar(ArmVerticCount, Height + Outline, PropNameArmVerticDiam, PropNameArmVerticPos, "Вертикальная арматура");
        }

        public override void Numbering ()
        {            
            // ВертикАрм         
            FillElemPropNameDesc(ArmVertic, PropNameArmVerticPos, PropNameArmVerticDesc);
            // Хомут
            FillElemPropNameDesc(Shackle, PropNameShacklePos, PropNameShackleDesc);
        }
    }
}