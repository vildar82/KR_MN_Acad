using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Торец стены - прямой
    /// </summary>
    public class WallJoinBlock : SchemeBlock
    {
        private SchemeService service;
        public const string WallJoinBlockName = "КР_Арм_Стена_Торец";

        const string PropNameConcrete = "Бетон";
        const string PropNameHeight = "Высота стены";
        const string PropNameLength = "Длина торца";
        const string PropNameThickness = "Толщина стены";
        const string PropNameOutline = "Выпуск";
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticCount = "КолВертикАрм";
        const string PropNameArmHorDiam = "ДиамГорАрм";
        const string PropNameArmHorStep = "ШагГорАрм";
        const string PropNameShackleDiam = "ДиамХомута";
        const string PropNameShackleStep = "ШагХомута";
        const string PropNameBracketDiam = "ДиамСкобы";
        const string PropNameBracketStep = "ШагСкобы";
        const string PropNameBracketLen = "ДлинаСкобы";
        const string PropNamePosVerticArm = "ПОЗВЕРТИКАРМ";
        const string PropNamePosHorArm = "ПОЗГОРАРМ";
        const string PropNamePosShackle = "ПОЗХОМУТА";
        const string PropNamePosBracket = "ПОЗСКОБЫ";
        const string PropNameDescHorArm = "ОПИСАНИЕГОРАРМ";
        const string PropNameDescVerticArm = "ОПИСАНИЕВЕРТИКАРМ";
        const string PropNameDescShackle = "ОПИСАНИЕХОМУТА";
        const string PropNameDescBracket = "ОПИСАНИЕСКОБЫ";

        /// <summary>
        /// Защитный салой бктона
        /// </summary>
        private const int a = 45;        

        /// <summary>
        /// Высота стены
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Длина стены
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Толщина стены
        /// </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// Выпуск стержней по вертикали
        /// </summary>
        public int Outline { get; set; }
        /// <summary>
        /// Кол вертик стержней
        /// </summary>
        public int ArmVerticCount { get; set; }
        /// <summary>
        /// Длина скобы
        /// </summary>
        public int BracketLength { get; set; }
        /// <summary>
        /// Распределенные вертикальные арматурные стержни
        /// </summary>
        public Bar ArmVertic { get; set; }
        /// <summary>
        /// Горизонтальные арматурные стержни - погоннаж
        /// </summary>
        public BarRunningStep ArmHor { get; set; }
        /// <summary>
        /// Шпильки
        /// </summary>
        public Shackle Shackle { get; set; }
        /// <summary>
        /// Хомуты
        /// </summary>
        public Bracket Bracket { get; set; }
        /// <summary>
        /// Бетон
        /// </summary>
        public ConcreteH Concrete { get; set; }

        public WallJoinBlock(BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
        {            
        }

        public override void Calculate()
        {
            // Определение параметров.
            // Расчет элементов схемы.
            try
            {
                defineFields();
                AddElements();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }        

        private void AddElements()
        {            
            AddElement(ArmVertic);
            AddElement(ArmHor);
            AddElement(Shackle);
            AddElement(Bracket);
            AddElement(Concrete);
        }

        public override void Numbering()
        {
            // ПозГорАрм
            FillProp(GetProperty(PropNamePosHorArm), ArmHor?.SpecRow.PositionColumn);
            // ПозВертикАрм
            FillProp(GetProperty(PropNamePosVerticArm), ArmVertic?.SpecRow.PositionColumn);
            // ПозХомута
            FillProp(GetProperty(PropNamePosShackle), Shackle?.SpecRow.PositionColumn);
            // ПозСкобы
            FillProp(GetProperty(PropNamePosBracket), Bracket?.SpecRow.PositionColumn);
            // ОписГорАрм
            FillProp(GetProperty(PropNameDescHorArm), ArmHor?.GetDesc());
            // ОписВертикАрм
            FillProp(GetProperty(PropNameDescVerticArm), ArmVertic?.GetDesc());
            // ОписХомута
            FillProp(GetProperty(PropNameDescShackle), Shackle?.GetDesc());
            // ОписСкобы
            FillProp(GetProperty(PropNameDescBracket), Bracket?.GetDesc());
        }

        private void defineFields()
        {
            Length = GetPropValue<int>(PropNameLength);
            Height = GetPropValue<int>(PropNameHeight);
            Thickness = GetPropValue<int>(PropNameThickness);
            Outline = GetPropValue<int>(PropNameOutline);
            BracketLength = GetPropValue<int>(PropNameBracketLen);
            ArmVerticCount = GetPropValue<int>(PropNameArmVerticCount);
            var concrete = GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor();
            // Хомут
            Shackle = defineShackle();
            // Скоба
            Bracket = defineBracket();
        }

        private Bar defineArmVertic()
        {
            if (ArmVerticCount == 0)
                return null;
            string pos = GetPropValue<string>(PropNamePosVerticArm);
            int diam = GetPropValue<int>(PropNameArmVerticDiam);                        
            int len = Height + Outline;
            var arm = new Bar(diam, len, pos, this, "Вертикальная арматура");
            arm.Count = ArmVerticCount;
            arm.Calc();
            return arm;
        }

        private BarRunningStep defineArmHor()
        {
            string pos = GetPropValue<string>(PropNamePosHorArm);
            int diam = GetPropValue<int>(PropNameArmHorDiam);
            int step = GetPropValue<int>(PropNameArmHorStep);
            int width = Height - 100;
            double len = getLengthHorArm(diam, Concrete.ClassB);
            var armHor = new BarRunningStep(diam, len, width, step, 2, pos, this, "Горизонтальная арматура");
            armHor.Calc();
            return armHor;
        }

        private Shackle defineShackle()
        {
            string pos = GetPropValue<string>(PropNamePosShackle);
            int diam = GetPropValue<int>(PropNameShackleDiam);
            if (diam == 0) return null;
            int step = GetPropValue<int>(PropNameShackleStep);
            // длина хомута
            int len = Shackle.GetLenShackle((Length-(2*a)+ArmVertic.Diameter), (Thickness-(2*a)+ArmVertic.Diameter));

            // ширина распределения шпилек
            int width = Height;            

            Shackle s = new Shackle(diam, len, step, width, pos, this);
            s.Calc();
            return s;
        }

        private Bracket defineBracket()
        {
            string pos = GetPropValue<string>(PropNamePosBracket);
            int diam = GetPropValue<int>(PropNameBracketDiam);
            if (diam == 0) return null;
            int step = GetPropValue<int>(PropNameBracketStep);
            // длина скобы
            int len = RoundHelper.RoundWhole(BracketLength * 2 + (Thickness - (2 * a) + ArmVertic.Diameter) + (Length - a + ArmVertic.Diameter * 0.5) * 2);
            // ширина распределения
            int width = Height;

            Bracket b = new Bracket(diam, len, step, width, pos, this);
            b.Calc();
            return b;
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>        
        private double getLengthHorArm(int diam, string classB)
        {
            double kLap = BarRunning.GetKLap(diam, classB);
            return Length * kLap;
        }        
    }
}
