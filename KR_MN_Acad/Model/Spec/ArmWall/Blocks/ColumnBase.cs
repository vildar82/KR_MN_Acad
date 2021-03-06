﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Spec.Constructions.Elements;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Описание блока колонны квадратной до 400мм
    /// </summary>
    public abstract class ColumnBase : SpecBlock, Constructions.IConstructionBlock
    {
        private const string propMark = "МАРКА";        
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
        protected const string PropNamePosVerticBentDirect = "ПОЗВЕРТИКГС";
        protected const string PropNameDescVerticBentDirect = "ОПИСАНИЕВЕРТИКГС";
        protected const string PropNameAddLength = "ВысотаУсиления";
        protected const string PropNameAddStep = "ШагУсиления";

        public virtual List<ISpecElement> Elementary { get; set; } = new List<ISpecElement>();
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
        //public Column Column { get; set; }

        /// <summary>
        /// Гнутые вертикальные стержни
        /// </summary>
        public BentBarDirect BentBarDirect { get; set; }

        public ISpecElement ConstructionElement { get; set; }

        public ColumnBase (Autodesk.AutoCAD.DatabaseServices.BlockReference blRef, string blName) : base(blRef, blName)
        {
        }        

        protected virtual ISpecElement GetConstruction (string mark)
        {
            var col = new Column(Width, Thickness, Height, mark, this, Elementary);
            col.Calc();
            return col;
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
            Height = Block.GetPropValue<int>(PropNameHeight);            
            Outline = Block.GetPropValue<int>(PropNameOutline);
            var classB = Block.GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(classB, Width, Thickness, Height, this);
            Concrete.Calc();            
            // Определние вертикальной арматуры            
            ArmVertic = defineVerticArm();

            // Если диам вертик арм >= 20, то 4 стержня гнутся.
            checkBentBarDirect(ArmVertic, 4);

            // Хомут
            if (defaultShackle)
            {                
                Shackle = defineShackleByGab(width, thickness, Height, ArmVertic.Diameter, a, PropNameShackleDiam,
                    PropNameShacklePos, PropNameShackleStep);
                int countShackle = GetCountShackle(Shackle.Rows);
                if (countShackle- Shackle.Count > 0)
                {
                    Shackle.AddCount(countShackle- Shackle.Count);
                }
            }
            AddElementarys();            
        }

        

        public override void Calculate ()
        {
            // Нумерация элементов в конструкции            
            SpecGroup.SpecGroupService service = new SpecGroup.SpecGroupService (Block.IdBlRef.Database);
            service.Numbering(Elementary);
            NumberingElementary();                        

            string mark = Block.GetPropValue<string> (propMark);
            ConstructionElement = GetConstruction(mark);                        
            base.AddElement(ConstructionElement);
            base.AddElement(Concrete);
        }

        protected override void AddElement (ISpecElement elem)
        {
            throw new InvalidOperationException("Для конструкций нужно использовать AddElementary метод.");
        }        

        public override void Numbering ()
        {
            Block.FillPropValue(propMark, ConstructionElement.Mark);
        }

        private void AddElementarys ()
        {
            AddElementary(ArmVertic);
            AddElementary(Shackle);
            AddElementary(Concrete);
            AddElementary(BentBarDirect);
        }

        protected void AddElementary(ISpecElement elem)
        {
            if (elem != null && elem.Amount != 0)
            {
                Elementary.Add(elem);
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
            ArmVerticCount = Block.GetPropValue<int>(PropNameArmVerticCount);
            return defineBar(ArmVerticCount, Height + Outline, PropNameArmVerticDiam, PropNameArmVerticPos, "Вертикальная арматура");
        }

        protected virtual void NumberingElementary()
        {
            // ВертикАрм         
            FillElemPropNameDesc(ArmVertic, PropNameArmVerticPos, PropNameArmVerticDesc);
            // Хомут
            FillElemPropNameDesc(Shackle, PropNameShacklePos, PropNameShackleDesc);
            // ВертикГс
            FillElemPropNameDesc(BentBarDirect, PropNamePosVerticBentDirect, PropNameDescVerticBentDirect);
        }

        /// <summary>
        /// Определение вертикальных гнутых стержней.        
        /// </summary>
        protected void checkBentBarDirect (Bar armVertic, int countBent)
        {
            if (Requirements.IsNeedToBentVerticArm(armVertic.Diameter))
            {
                //armVertic.Count -= countBent;
                armVertic.AddCount(-countBent);
                //if (armVertic.Count == 0) Elements.Remove(armVertic);
                BentBarDirect = defineBentDirect(armVertic.Diameter, countBent, Height, Outline, PropNamePosVerticBentDirect);
            }
        }

        /// <summary>
        /// Определение количества хомутов в колонне, с учетом усиленной части
        /// </summary>
        /// <returns></returns>
        protected int GetCountShackle (int rows)
        {
            int resCount = 0;
            var addLength = Block.GetPropValue<int>(PropNameAddLength);
            int step = Block.GetPropValue<int>(PropNameShackleStep);
            if (addLength>0)
            {
                // Кол в усиленной части
                int addStep = Block.GetPropValue<int>(PropNameAddStep);
                var addCount = Bar.CalcCountByStep((addLength-50), addStep);
                // Кол в верхней части
                int topCount = Bar.CalcCountByStep((Height-addLength-50-step), step);
                resCount = addCount + topCount;
            }
            else
            {
                // Без усиления
                resCount = Bar.CalcCountByStep(Height - 100, step);
            }
            return resCount * rows;
        }
    }
}