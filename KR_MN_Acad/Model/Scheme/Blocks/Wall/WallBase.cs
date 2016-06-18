using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;

namespace KR_MN_Acad.Scheme.Wall
{
    public abstract class WallBase : SchemeBlock
    {
        /// <summary>
        /// Отступ вертикальной арматуры от торца стены
        /// </summary>
        private int indentVerticArm;
        /// <summary>
        /// Защитный слой бетона до центра арматуры
        /// </summary>
        protected const int a = 45;
        protected const string PropNameHeight = "Высота стены";
        protected const string PropNameOutline = "Выпуск";
        protected const string PropNameConcrete = "Бетон";
        protected const string PropNameArmHorDiam = "ДиамГорАрм";
        protected const string PropNameArmHorStep = "ШагГорАрм";
        protected const string PropNamePosVerticArm = "ПОЗВЕРТИКАРМ";
        protected const string PropNamePosHorArm = "ПОЗГОРАРМ";
        protected const string PropNameDescHorArm = "ОПИСАНИЕГОРАРМ";
        protected const string PropNameDescVerticArm = "ОПИСАНИЕВЕРТИКАРМ";
        /// <summary>
        /// Высота стены
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Выпуск стержней по вертикали
        /// </summary>
        public int Outline { get; set; }
        /// <summary>
        /// Горизонтальные арматурные стержни - погоннаж
        /// </summary>
        public BarRunningStep ArmHor { get; set; }
        /// <summary>
        /// Бетон
        /// </summary>
        public ConcreteH Concrete { get; set; }

        public WallBase (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
        {
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>
        /// <returns></returns>
        protected double getLengthHorArm (int length, int diam, string classB)
        {
            double kLap = BarRunning.GetKLap(diam, classB);
            return length * kLap;
        }

        /// <summary>
        /// Определение ширины распределения вертикальных стержней в стене
        /// </summary>        
        protected int getWidthVerticArm (int step, int length)
        {
            // Вычесть отступ у торцов стены = шаг вертик стержней - а.
            indentVerticArm = step - a;
            return length - indentVerticArm * 2;
        }

        protected BarRunningStep defineArmHor (int length)
        {
            int diam = GetPropValue<int>(PropNameArmHorDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(PropNamePosHorArm);            
            int step = GetPropValue<int>(PropNameArmHorStep);
            int width = Height - 100;
            double len = getLengthHorArm(length, diam, Concrete.ClassB);
            var armHor = new BarRunningStep (diam, len, width, step, 2, pos, this, "Горизонтальная арматура");
            armHor.Calc();
            return armHor;
        }

        protected Bracket defineBracket (string propNameBracketDiam, string propNamePosBracket,
            string propNameBracketStep, int bracketLen, int thickness, int diamVerticArm)
        {
            int diam = GetPropValue<int>(propNameBracketDiam, false);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propNamePosBracket, false);
            int step = GetPropValue<int>(propNameBracketStep);            
            // ширина распределения
            int wBracket = Height;
            // ширина скобы
            int tBracket = thickness - 2 * a + diamVerticArm;
            Bracket b = new Bracket(diam, bracketLen, tBracket , step, wBracket, pos, this);
            b.Calc();
            return b;
        }

    }
}
