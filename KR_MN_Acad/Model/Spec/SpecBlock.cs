using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Materials;

namespace KR_MN_Acad.Spec
{
    public abstract class SpecBlock : ISpecBlock
    {        
        public IBlock Block { get; set; }
        public virtual List<ISpecElement> Elements { get; set; } = new List<ISpecElement>();
        public Error Error { get { return Block.Error; } }

        public SpecBlock(BlockReference blRef, string blName)
        {
            Block = new BlockBase(blRef, blName);
        }
        
        public abstract void Calculate ();
        public abstract void Numbering ();

        protected void FillElemPropNameDesc (ISpecElement elem, string posPropertyName, string descPropertyName)
        {
            if (elem == null) return;
            if (elem.Amount!=0)
            {
                // Поз
                Block.FillPropValue(posPropertyName, elem.Mark);
                // Опис            
                Block.FillPropValue(descPropertyName, elem.GetDesc());                
            }
            else
            {
                // Поз
                Block.FillPropValue(posPropertyName, "");
                // Опис            
                Block.FillPropValue(descPropertyName, "");
            }
        }

        protected void AddError(string err)
        {
            Block.AddError(err);
        }

        /// <summary>
        /// Определение отдельных стержней
        /// </summary>
        /// <param name="countArm">Кол-во стержней</param>
        /// <param name="length">Длина стержня</param>
        /// <param name="propDiam">Параметр диаметра стержня</param>
        /// <param name="propPos">Параметр позиции стержня</param>
        /// <param name="friendName">Человеческое имя стержня для описания ошибок - Горизонтальные стержни, Хомут, и т.п.</param>
        /// <returns></returns>
        protected Bar defineBar (int countArm, int length, string propDiam, string propPos, string friendName)
        {
            if (countArm == 0) return null;
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            var arm = new Bar(diam, length, countArm, pos, this, friendName);
            arm.Calc();
            return arm;
        }

        /// <summary>
        /// Определение распределенной арматуры
        /// </summary>
        /// <param name="length">Длина стержня</param>
        /// <param name="widthRun">Ширина распределения</param>
        /// <param name="step">Шаг</param>
        /// <param name="propDiam">Парам диаметра</param>        
        /// <param name="propPos">Параметр аозиции</param>
        /// <param name="rows">Рядов стержней</param>
        /// <param name="friendlyName">Пользовательское имя</param>        
        protected Bar defineBarDiv (int length, int widthRun, int step, string propDiam, string propPos,
            int rows, string friendlyName)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            var armDiv = new Bar(diam, length, widthRun, step, rows, pos, this, friendlyName);
            armDiv.Calc();
            return armDiv;
        }

        /// <summary>
        /// Определение погонной арм с шириной и шагом распределения
        /// </summary>
        /// <param name="length"></param>
        /// <param name="widthRun"></param>
        /// <param name="propDiam"></param>
        /// <param name="propPos"></param>
        /// <param name="propStep"></param>
        /// <param name="concrete"></param>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        protected BarRunning defineBarRunStep (int length, int widthRun, int rows, string propDiam, string propPos, string propStep,
            Concrete concrete, string friendlyName)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            int step = Block.GetPropValue<int>(propStep);
            double len = getLengthRunArm(length, diam, concrete);
            var arm = new BarRunning (diam, len, widthRun, step, rows, pos, this, friendlyName);
            arm.Calc();
            return arm;
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>
        /// <returns></returns>
        protected double getLengthRunArm (int length, int diam, Concrete concrete)
        {
            double kLap = BarRunning.GetKLap(diam, concrete);
            return length * kLap;
        }

        protected virtual void AddElement (ISpecElement elem)
        {
            if (elem != null && elem.Amount!=0)
            {
                Elements.Add(elem);
            }
        }

        /// <summary>
        /// Определение скобы
        /// </summary>
        /// <param name="propDiam">Парам диам скобы</param>
        /// <param name="propPos">Парам поз скобы</param>
        /// <param name="propStep">Парам шаг скобы</param>
        /// <param name="bracketLen">Длина вылета скобы</param>
        /// <param name="thicknessWall">Толщина стены</param>
        /// <param name="a">Защитный слой до центра раб арм</param>
        /// <param name="widthRun">Ширина распределения скобы (высота стены)</param>
        /// <param name="diamWorkArm">Диам раб арм</param>
        /// <returns></returns>
        protected Bracket defineBracket (string propDiam, string propPos,
            string propStep, int bracketLen, int thicknessWall, int a, int widthRun, int diamWorkArm, int rows = 1)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            int step = Block.GetPropValue<int>(propStep);
            // ширина скобы
            int tBracket = thicknessWall - 2 * a + diamWorkArm;
            Bracket b = new Bracket(diam, bracketLen, tBracket , step, widthRun,rows, pos, this);
            b.Calc();
            return b;
        }

        /// <summary>
        /// Определение хомута - по габаритам бетона
        /// </summary>
        /// <param name="width">Ширина бетона</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="range">Ширина распределения хомутов (по бетону, отступ 100 отнимается тут)</param>
        /// <param name="diamWorkArm">Раб диам</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам позиции</param>
        /// <param name="propStep">Парам шага</param>        
        protected Shackle defineShackleByGab (int width, int thickness, int range, int diamWorkArm, int a,
            string propDiam, string propPos, string propStep, int rows = 1)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            int step = Block.GetPropValue<int>(propStep);
            // длина хомута
            int lShackle =  width-(2*a)+diamWorkArm;
            int hShackle = thickness-(2*a)+diamWorkArm;
            Shackle s = new Shackle(diam, lShackle, hShackle, step, range-100, rows, pos, this);
            s.Calc();
            return s;
        }

        /// <summary>
        /// Хомут по длине хомута
        /// </summary>
        /// <param name="propShLen">Параметр Длины хомута по внутр граням стержней</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="range">Ширина распределения хомутов (по бетону, отступ 100 отнимается тут)</param>
        /// <param name="diamWorkArm">Диам раб арм</param>
        /// <param name="propDiam">Парам диаметра</param>
        /// <param name="propPos">Значение атр позиции</param>
        /// <param name="propStep">Параметр шага</param>                
        protected Shackle defineShackleByLen (string propShLen, int thickness, int a, int range, int diamWorkArm,
            string propDiam, string propPos, string propStep, int rows = 1)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            var shackleLen = Block.GetPropValue<int>(propShLen);            
            string pos = Block.GetPropValue<string>(propPos);
            int step = Block.GetPropValue<int>(propStep);
            int shackleH = thickness-(2*a)+diamWorkArm;
            var s = new Shackle(diam, shackleLen, shackleH, step, range-100,rows,  pos, this);
            s.Calc();
            return s;
        }

        /// <summary>
        /// определение шпильки
        /// </summary>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам поз</param>
        /// <param name="propStepHor">Параметр шага по гор</param>
        /// <param name="propStepVert">Параметр шага по вертикали</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="widthHor">Ширина распределения по гор</param>
        /// <param name="widthVert">Ширина распределения по верт</param>
        /// <returns></returns>
        protected Spring defineSpring (string propDiam, string propPos, string propStepHor, string propStepVert,
           int thickness, int a, int widthHor, int widthVert)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            int stepHor = Block.GetPropValue<int>(propStepHor);
            int stepVert = Block.GetPropValue<int>(propStepVert);
            // ширина распределения шпилек по горизонтале            
            var lRabSpring =  thickness - 2 * a;
            Spring sp = new Spring(diam, lRabSpring, stepHor, stepVert, widthHor, widthVert, pos, this);
            sp.Calc();
            return sp;
        }

        /// <summary>
        /// Шпилька с шагом по ширине распределения и кол рядов
        /// </summary>
        /// <param name="propDiam">Диаметр</param>
        /// <param name="propPos">Значение атр позиции</param>
        /// <param name="propStep">Параметр шага</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="width">Ширина распределения (по бетону)</param>
        /// <param name="propCount">Параметр рядов шпилек</param>        
        protected Spring defineSpring (string propDiam, string propPos, string propStep,
           int thickness, int a, int width, string propCount)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = Block.GetPropValue<string>(propPos);
            int step = Block.GetPropValue<int>(propStep);
            var rows = Block.GetPropValue<int>(propCount);
            // ширина распределения шпилек по горизонтале            
            var lRabSpring =  thickness - 2 * a;
            Spring sp = new Spring(diam, lRabSpring, step, width-100, rows, pos, this);
            sp.Calc();
            return sp;
        }


        /// <summary>
        /// определение гнутого стерженя
        /// </summary>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="bentL">Длина Гс</param>
        /// <param name="bentH">высота Гс</param>
        /// <param name="width">Ширина распределения</param>
        /// <param name="propStep">Парам шаг</param>
        /// <param name="propPos">Парам позиции (атр)</param>        
        protected BentBarLshaped defineBent (string propDiam, int bentL, int bentH, int width, string propStep,
            string propPos, int rows = 1)
        {
            int diam = Block.GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            int step = Block.GetPropValue<int>(propStep);
            string pos = Block.GetPropValue<string>(propPos);
            BentBarLshaped bent = new BentBarLshaped (diam, bentL, bentH, width, step, rows, pos, this);
            bent.Calc();
            return bent;
        }

        /// <summary>
        /// Вертикальные гнутые стержни на величину диаметра стержня
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="count">Кол шт.</param>
        /// <param name="height">Высота конструкции по бетоны</param>
        /// <param name="outline">Выпуск</param>
        /// <param name="propPos">Значение параметра позиции</param>        
        protected BentBarDirect defineBentDirect (int diam, int count, int height, int outline, string propPos)
        {
            int lstart = height;
            int lDif = 150;
            int lEnd = outline-lDif;
            int hDif = diam;
            string pos = Block.GetPropValue<string>(propPos);
            var bent = new BentBarDirect(diam, lstart, lEnd, lDif, hDif, count, pos, this);
            bent.Calc();
            return bent;
        }
    }
}
