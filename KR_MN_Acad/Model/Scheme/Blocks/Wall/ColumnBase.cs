using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока колонны квадратной до 400мм
    /// </summary>
    public abstract class ColumnBase : SchemeBlock
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

        public ColumnBase (BlockReference blRef, string blName, SchemeService service) : base (blRef, blName, service)
        {            
        }

        /// <summary>
        /// Определение базовых параметров колонны - выпуск, высота, бетон и т.д.
        /// </summary>
        /// <param name="width">Ширина колонны</param>
        /// <param name="thickness">Толщина колонны</param>
        protected void DefineBaseFields(int width, int thickness, bool defaultShackle)
        {
            Width = width;
            Thickness = thickness;
            Height = Convert.ToInt32(GetPropValue<double>(PropNameHeight));            
            Outline = Convert.ToInt32(GetPropValue<double>(PropNameOutline));
            var classB = GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(classB, Width, Thickness, Height, this);
            Concrete.Calc();
            AddElement(Concrete);
            // Определние вертикальной арматуры            
            ArmVertic = defineVerticArm();
            AddElement(ArmVertic);
            // Хомут
            if (defaultShackle)
            {
                Shackle = defineShackleByGab(width, thickness, Height, ArmVertic.Diameter, a, PropNameShackleDiam,
                    PropNameShacklePos, PropNameShackleStep);
                AddElement(Shackle);
            }            
        }        

        /// <summary>
        /// Вертикальные отдельные стержени
        /// </summary>
        /// <param name="count">Кол</param>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам поз</param>        
        protected Bar defineVerticArm ()
        {
            ArmVerticCount = GetPropValue<int>(PropNameArmVerticCount);
            return defineBar(ArmVerticCount, Height + Outline, PropNameArmVerticDiam, PropNameArmVerticPos, "Вертикальная арматура");
        }

        public override void Numbering ()
        {            
            // ВертикАрм         
            FillElemProp(ArmVertic, PropNameArmVerticPos, PropNameArmVerticDesc);
            // Хомут
            FillElemProp(Shackle, PropNameShacklePos, PropNameShackleDesc);
        }
    }
}