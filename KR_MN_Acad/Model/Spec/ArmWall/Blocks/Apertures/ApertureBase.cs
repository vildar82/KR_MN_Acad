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
    public abstract class ApertureBase : WallBase
    {        
        protected new const string PropNameArmHorDiam = "ДиамГорАрмФон";
        protected new const string PropNameArmHorStep = "ШагГорАрмФон";
        protected new const string PropNameArmVerticPos = "ПОЗВЕРТИКАРМПОГОННОЙ";
        protected new const string PropNameArmHorPos = "ПОЗГОРАРМПОГОННОЙ";
        protected new const string PropNameArmHorDesc = "ОПИСАНИЕГОРПОГОННОЙ";
        protected new const string PropNameArmVerticDesc = "ОПИСАНИЕВЕРТИКПОГОННОЙ";

        /// <summary>
        /// Высота проема
        /// </summary>
        protected string PropNameApertureHeight;
        /// <summary>
        /// Высота до низа проема (для дверей 0, для окна 900)
        /// </summary>
        protected string PropNameApertureHeightAbove;
        const string PropNameView = "Вид торцов";        
        const string PropNameApertureWidth = "ШиринаПроема";
        const string PropNameThickness = "ТолщинаСтены";
        const string PropNameAddArmDiam = "ДиамАрмУсиления";
        protected string PropNameAddArmHorCountTop;
        protected string PropNameAddArmHorCountBottom;
        const string PropNameArmVerticDiam = "ДиамВертикАрмФон";
        const string PropNameArmVerticStep = "ШагВертикАрмФон";        
        const string PropNameBracketLeftLength = "ДлинаСкобыСлева";
        const string PropNameBracketRightLength = "ДлинаСкобыСправа";
        const string PropNameShackleDiam = "ДиамХомута";
        const string PropNameShackleStep = "ШагХомута";

        const string PropNameAddArmVerticPos = "ПОЗВЕРТИКАРМУСИЛЕНИЯ";
        const string PropNameAddArmVerticDesc = "ОПИСАНИЕВЕРТИКАРМУСИЛЕНИЯ";
        const string PropNameAddArmHorPos = "ПОЗГОРАРМУСИЛЕНИЯ";
        const string PropNameAddArmHorDesc = "ОПИСАНИЕГОРАРМУСИЛЕНИЯ";
        const string PropNameBracketTopPos = "ПОЗСКОБЫВЕРХНЕЙ";
        const string PropNameBracketTopDesc = "ОПИСАНИЕСКОБЫВЕРХНЕЙ";
        const string PropNameBracketBottomPos = "ПОЗСКОБЫНИЖНЕЙ";
        const string PropNameBracketBottomDesc = "ОПИСАНИЕСКОБЫНИЖНЕЙ";
        const string PropNameBracketLeftPos = "ПОЗСКОБЫСЛЕВА";
        const string PropNameBracketLeftDesc = "ОПИСАНИЕСКОБЫСЛЕВА";
        const string PropNameBracketRightPos = "ПОЗСКОБЫСПРАВА";
        const string PropNameBracketRightDesc = "ОПИСАНИЕСКОБЫСПРАВА";
        protected string PropNameShackleTopPos = "ПОЗХОМУТАСВЕРХУ";
        protected string PropNameShackleTopDesc = "ОПИСАНИЕХОМУТАСВЕРХУ";
        const string PropNameShackleBottomPos = "ПОЗХОМУТАСНИЗУ";
        const string PropNameShackleBottomDesc = "ОПИСАНИЕХОМУТАСНИЗУ";
        const string PropNameArmDiagonalPos = "ПОЗДИАГАНАЛЬНОЙАРМ";
        const string PropNameArmDiagonalDesc = "ОПИСАНИЕДИАГАРМ";

        bool hasBottomArm;
        bool hasLeftEnd;
        bool hasRightEnd;
        int endsLength = 200;
        int addArmHorTopCount;
        int addArmHorBottomCount;

        public int Thickness { get; set; }
        public int ApertureWidth { get; set; }
        public int ApertureHeight { get; set; }
        /// <summary>
        /// Высота до проема
        /// </summary>
        public int ApertureHeightUnder { get; set; }
        public Bar AddArmVertic { get; set; }
        public Bar AddArmHor { get; set; }
        public Bracket BracketLeft { get; set; }
        public Bracket BracketRight { get; set; }
        public Bracket BracketTop { get; set; }
        public Shackle ShackleTop { get; set; }
        public Bracket BracketBottom { get; set; }
        public Shackle ShackleBottom { get; set; }
        public Bar BarDiagonal { get; set; }

        public ApertureBase (BlockReference blRef, string blName) : base(blRef, blName)
        {
        }

        public override void Calculate ()
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

        public override void Numbering ()
        {
            // ГорАрмФон      
            FillElemPropNameDesc(ArmHor, PropNameArmHorPos, PropNameArmHorDesc);
            // ВертикАрмФон       
            FillElemPropNameDesc(ArmVertic, PropNameArmVerticPos, PropNameArmVerticDesc);
            // Усиление вертик
            FillElemPropNameDesc(AddArmVertic, PropNameAddArmVerticPos, PropNameAddArmVerticDesc);
            // Усиление гор
            FillElemPropNameDesc(AddArmHor, PropNameAddArmHorPos, PropNameAddArmHorDesc);
            // Диаг стерждень
            FillElemPropNameDesc(BarDiagonal, PropNameArmDiagonalPos, PropNameArmDiagonalDesc);
            // Скоба слева
            FillElemPropNameDesc(BracketLeft, PropNameBracketLeftPos, PropNameBracketLeftDesc);
            // Скоба справа
            FillElemPropNameDesc(BracketRight, PropNameBracketRightPos, PropNameBracketRightDesc);
            // Скоба сверху
            FillElemPropNameDesc(BracketTop, PropNameBracketTopPos, PropNameBracketTopDesc);
            // Скоба снизу
            FillElemPropNameDesc(BracketBottom, PropNameBracketBottomPos, PropNameBracketBottomDesc);
            // Хомут Сверху
            FillElemPropNameDesc(ShackleTop, PropNameShackleTopPos, PropNameShackleTopDesc);
            // Хомут Снизу
            FillElemPropNameDesc(ShackleBottom, PropNameShackleBottomPos, PropNameShackleBottomDesc);
        }
        protected override void AddElements ()
        {
            AddElement(AddArmHor);
            AddElement(AddArmVertic);
            AddElement(BarDiagonal);
            AddElement(BracketLeft);
            AddElement(BracketRight);
            AddElement(BracketTop);
            AddElement(ShackleTop);
            AddElement(BracketBottom);
            AddElement(ShackleBottom);
            AddElement(ArmHor);
            AddElement(ArmVertic);            
            AddElement(Concrete);
        }

        private void defineFields ()
        {
            // определение наличия торцов
            defineEnds();

            ApertureWidth = Block.GetPropValue<int>(PropNameApertureWidth);
            ApertureHeight = Block.GetPropValue<int>(PropNameApertureHeight);
            ApertureHeightUnder = Block.GetPropValue<int>(PropNameApertureHeightAbove, isRequired:false);
            Height = Block.GetPropValue<int>(PropNameHeight);
            Thickness = Block.GetPropValue<int>(PropNameThickness);
            Outline = Block.GetPropValue<int>(PropNameOutline);

            // Бетон
            defineConcrete();

            // Определние вертикальной арматуры усиления
            defineAddVerticArm();

            // Определние горизонтальной арматуры усиления
            defineAddHorArm();

            // Определение стержней над проемом - Скоба, Хомут, Гор погонная, Вертик погонная если надо
            defineAbove();
            defineUnder();

            // Скобы
            defineSideBrackets();

            // Диагональные стержни
            int diamDiag = 10;
            int lapLenDiagonal = Armature.GetLapLength(diamDiag, Concrete);
            string posDiag = Block.GetPropValue<string>(PropNameArmDiagonalPos);
            int countDiagArm = 4 + (hasBottomArm ? 4 : 0);
            BarDiagonal = new Bar(diamDiag, lapLenDiagonal * 2, countDiagArm, posDiag, this, "Диагональные стержни проема");
            BarDiagonal.Calc();            
        }        

        private void defineSideBrackets ()
        {
            int widthRun = ApertureHeight -50;
            if (hasLeftEnd)
            {
                int brLenLeft = Block.GetPropValue<int>(PropNameBracketLeftLength);
                BracketLeft = defineBracket(PropNameArmHorDiam, PropNameBracketLeftPos, PropNameArmHorStep,
                    brLenLeft, Thickness, a, widthRun, AddArmVertic.Diameter, 1, "Скоба слева");
            }
            if (hasRightEnd)
            {
                int brLenRight = Block.GetPropValue<int>(PropNameBracketRightLength);
                BracketRight = defineBracket(PropNameArmHorDiam, PropNameBracketRightPos, PropNameArmHorStep,
                    brLenRight, Thickness, a, widthRun, AddArmVertic.Diameter, 1, "Скоба справа");
            }
        }

        private void defineAddHorArm ()
        {
            addArmHorTopCount = Block.GetPropValue<int>(PropNameAddArmHorCountTop);
            addArmHorBottomCount = Block.GetPropValue<int>(PropNameAddArmHorCountBottom, isRequired:false);
            int countAddArmHor = addArmHorTopCount + addArmHorBottomCount;
            int diamAddArmHor = Block.GetPropValue<int>(PropNameAddArmDiam);
            int lapLengthArmHor =  Armature.GetLapLength(diamAddArmHor, Concrete);
            int lengthAddArmHor = ApertureWidth + (hasLeftEnd? lapLengthArmHor : 0) +(hasRightEnd? lapLengthArmHor : 0);
            AddArmHor = defineBar(countAddArmHor, lengthAddArmHor, PropNameAddArmDiam, PropNameAddArmHorPos, "Горизонтальные стержни усиления проема");
        }

        private void defineAddVerticArm ()
        {
            int countAddVerticArm = (hasLeftEnd? 4:0) + (hasRightEnd? 4:0);
            int lenAddVerticArm = Height+Outline;
            AddArmVertic = defineBar(countAddVerticArm, lenAddVerticArm, PropNameAddArmDiam, PropNameAddArmVerticPos,
                "Вертикальные стержни усиления проема");
        }

        private void defineAbove ()
        {
            // Определение арматуры надпроемной части
            // Наличие вертик погонной арм - на выпуск
            
            int diamVerticArm = Block.GetPropValue<int>(PropNameArmVerticDiam);
            int stepVerticArm = Block.GetPropValue<int>(PropNameArmVerticStep);
            // Длина нахлести вертик арм (для скобы)
            int lapLengthVerticArm = Armature.GetLapLength(diamVerticArm, Concrete);
            int widthRun = ApertureWidth-100;
            if (widthRun <= 0) return;            

            int lengthVerticArm = Height - ApertureHeight + Outline -20;
            int bracketLen = lapLengthVerticArm;

            // Если высота стены над проемом больше двух длин нахлеста, то добавляется вертикальная погонная арматура
            if  (Height-ApertureHeight> lapLengthVerticArm*2)
            {
                // Добавление вертик погонной арм - без коэф нахлести погонной арм!!!                
                string posArmVertic = Block.GetPropValue<string>(PropNameArmVerticPos);
                ArmVertic = new BarRunning(diamVerticArm, lengthVerticArm, widthRun, stepVerticArm, 2, posArmVertic, this, "Вертикальная погонная арматура");
                ArmVertic.Calc();
            }
            else
            {
                // Скоба на всю высоту выпуска вертик стержней (без погонной арматуры)
                bracketLen = lengthVerticArm;
            }
            // Скоба верхняя
            int tBracket = Thickness - 2*a - diamVerticArm;
            string posBracket = Block.GetPropValue<string>(PropNameBracketTopPos);
            BracketTop = new Bracket(diamVerticArm, bracketLen, tBracket, stepVerticArm, widthRun, 1, posBracket,
                this, "Скоба верхняя");
            BracketTop.Calc();

            // Хомут
            int stepShackle = Block.GetPropValue<int>(PropNameShackleStep);
            int diamShackle = Block.GetPropValue<int>(PropNameShackleDiam);
            int wShackle = Thickness - 2*a + diamVerticArm + AddArmHor.Diameter*2;
            int hShackle = (addArmHorTopCount == 4 ? 100: 200) + AddArmHor.Diameter;
            string posShackle = Block.GetPropValue<string>(PropNameShackleTopPos);
            ShackleTop = new Shackle(diamShackle, wShackle, hShackle, stepShackle, widthRun, 1, posShackle, this);
            ShackleTop.Calc();

            // Горизонтальная погонная арм - Фоновая
            int stepHorArm = Block.GetPropValue<int>(PropNameArmHorStep);
            int widthHorArm = Height - ApertureHeight - a - (addArmHorTopCount == 4 ? 100: 200)  - stepHorArm - 50;
            if (widthHorArm > 0)
            {
                ArmHor = defineBarRunStep(ApertureWidth, widthHorArm, 2, PropNameArmHorDiam, PropNameArmHorPos,
                    PropNameArmHorStep, Concrete, "Горизонтальная фоновая арматура");
                ArmHor.Calc();
            }
        }

        private void defineUnder ()
        {
            // Определение арматуры снизу проема            
            if (ApertureHeightUnder < 200)
            {
                return;
            }
            hasBottomArm = true;

            int diamVerticArm = Block.GetPropValue<int>(PropNameArmVerticDiam);
            int stepVerticArm = Block.GetPropValue<int>(PropNameArmVerticStep);
            // Длина нахлести вертик арм (для скобы)
            int lapLengthVerticArm = Armature.GetLapLength(diamVerticArm, Concrete);
            int widthRun = ApertureWidth - 100;
            if (widthRun <= 0) return;

            int lengthVerticArm = ApertureHeightUnder  - 20;
            int bracketLen = lapLengthVerticArm;

            // Если высота подоконника больше двух длин нахлеста, то добавляется вертикальная погонная арматура
            if (ApertureHeightUnder > lapLengthVerticArm * 2)
            {
                // Добавление вертик погонной арм - без коэф нахлести погонной арм!!!   
                string posArmVertic = Block.GetPropValue<string>(PropNameArmVerticPos);
                var barRunArmVerticTemp =new BarRunning(diamVerticArm, lengthVerticArm, widthRun, stepVerticArm, 2, posArmVertic, this, "Вертикальная погонная арматура");
                barRunArmVerticTemp.Calc();
                if (ArmVertic == null)
                {
                    ArmVertic = barRunArmVerticTemp;
                }
                else
                {
                    // Добавить метры к существующей верхней вертик погонной арм 
                    var barRunArmVertic = (BarRunning)ArmVertic;
                    barRunArmVertic.Meters += barRunArmVerticTemp.Meters;
                    barRunArmVertic.Calc();
                }
            }
            else
            {
                // Скоба на всю высоту выпуска вертик стержней (без погонной арматуры)
                bracketLen = lengthVerticArm;
            }
            // Скоба нижняя
            int tBracket = Thickness - 2 * a - diamVerticArm;
            string posBracket = Block.GetPropValue<string>(PropNameBracketBottomPos);
            BracketBottom = new Bracket(diamVerticArm, bracketLen, tBracket, stepVerticArm, widthRun, 1, posBracket,
                this, "Скоба нижняя");
            BracketBottom.Calc();

            // Хомут
            int stepShackle = Block.GetPropValue<int>(PropNameShackleStep);
            int diamShackle = Block.GetPropValue<int>(PropNameShackleDiam);
            int wShackle = Thickness - 2 * a + diamVerticArm + AddArmHor.Diameter * 2;
            int hShackle = (addArmHorBottomCount == 4 ? 100 : 200) + AddArmHor.Diameter;
            string posShackle = Block.GetPropValue<string>(PropNameShackleBottomPos);
            ShackleBottom = new Shackle(diamShackle, wShackle, hShackle, stepShackle, widthRun, 1, posShackle, this);
            ShackleBottom.Calc();

            // Горизонтальная погонная арм - Фоновая
            int stepHorArm = Block.GetPropValue<int>(PropNameArmHorStep);
            int widthHorArm = ApertureHeightUnder - a - (addArmHorBottomCount == 4 ? 100 : 200) - stepHorArm - 50;
            if (widthHorArm > 0)
            {
                var armHorTemp = defineBarRunStep(ApertureWidth, widthHorArm, 2, PropNameArmHorDiam, PropNameArmHorPos,
                        PropNameArmHorStep, Concrete, "Горизонтальная фоновая арматура");
                armHorTemp.Calc();
                if (ArmHor == null)
                {
                    ArmHor = armHorTemp;
                }
                else
                {
                    ArmHor.Meters += armHorTemp.Meters;
                    ArmHor.Count += armHorTemp.Count;
                    ArmHor.Width += armHorTemp.Width;
                    ArmHor.Calc();
                }
            }
        }
        
        private void defineConcrete ()
        {
            var classB =Block.GetPropValue<string>(PropNameConcrete);
            double volume = 0;
            // Объем верхней части            
            volume += (Height - ApertureHeight) * ApertureWidth * Thickness;
            // Объем нижней части
            volume += ApertureHeightUnder * ApertureWidth * Thickness;
            // бетон в торцах
            if (hasLeftEnd)
            {
                volume += endsLength * Thickness * Height;
            }
            if (hasRightEnd)
            {
                volume += endsLength * Thickness * Height;
            }
            Concrete = new ConcreteH(classB, volume* 0.000000001, this);
            Concrete.Calc();
        }

        private void defineEnds ()
        {
            string view = Block.GetPropValue<string>(PropNameView);
            switch (view.ToLower())
            {
                case "оба":
                    hasLeftEnd = true;
                    hasRightEnd = true;
                    break;
                case "левый":
                    hasLeftEnd = true;
                    hasRightEnd = false;
                    break;
                case "правый":
                    hasLeftEnd = false;
                    hasRightEnd = true;
                    break;
                default:
                    break;
            }
        }
    }
}
