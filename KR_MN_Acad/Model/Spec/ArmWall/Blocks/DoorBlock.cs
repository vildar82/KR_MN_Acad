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
    public class DoorBlock : WallBase
    {
        public const string BlockName = "КР_Арм_Стена_Дверь";

        protected new const string PropNameArmHorDiam = "ДиамГорАрмФон";
        protected new const string PropNameArmHorStep = "ШагГорАрмФон";
        protected new const string PropNameArmVerticPos = "ПОЗВЕРТИКАРМПОГОННОЙ";
        protected new const string PropNameArmHorPos = "ПОЗГОРПОГОННОЙ";
        protected new const string PropNameArmHorDesc = "ОПИСАНИЕГОРПОГОННОЙ";
        protected new const string PropNameArmVerticDesc = "ОПИСАНИЕВЕРТИКПОГОННОЙ";

        const string PropNameView = "Вид торцов";
        const string PropNameDoorHeight = "ВысотаПроема";
        const string PropNameDoorWidth = "ШиринаПроема";
        const string PropNameThickness = "ТолщинаСтены";
        const string PropNameAddArmDiam = "ДиамАрмУсиления";
        const string PropNameAddArmHorCount = "КолГорАрмУсиления";
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
        const string PropNameBracketLeftPos = "ПОЗСКОБЫСЛЕВА";
        const string PropNameBracketLeftDesc = "ОПИСАНИЕСКОБЫСЛЕВА";
        const string PropNameBracketRightPos = "ПОЗСКОБЫСПРАВА";
        const string PropNameBracketRightDesc = "ОПИСАНИЕСКОБЫСПРАВА";
        const string PropNameShacklePos = "ПОЗХОМУТА";
        const string PropNameShackleDesc = "ОПИСАНИЕХОМУТА";
        const string PropNameArmDiagonalPos = "ПОЗДИАГАНАЛЬНОЙАРМ";
        const string PropNameArmDiagonalDesc = "ОПИСАНИЕДИАГАРМ";

        bool hasLeftEnd;
        bool hasRightEnd;
        int endsLength = 200;

        public int Thickness { get; set; }
        public int DoorWidth { get; set; }
        public int DoorHeight { get; set; }
        
        public Bar AddArmVertic { get; set; }
        public Bar AddArmHor { get; set; }
        public Bracket BracketLeft { get; set; }
        public Bracket BracketRight { get; set; }
        public Bracket BracketTop { get; set; }
        public Shackle ShackleTop { get; set; }
        public Bar BarDiagonal { get; set; }

        public DoorBlock (BlockReference blRef, string blName) : base(blRef, blName)
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
            // Хомут
            FillElemPropNameDesc(ShackleTop, PropNameShacklePos, PropNameShackleDesc);
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
            AddElement(ArmHor);
            AddElement(ArmVertic);            
            AddElement(Concrete);
        }

        private void defineFields ()
        {
            // определение наличия торцов
            defineEnds();

            DoorWidth = Block.GetPropValue<int>(PropNameDoorWidth);
            DoorHeight = Block.GetPropValue<int>(PropNameDoorHeight);
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

            // Скобы
            defineSideBrackets();

            // Диагональные стержни
            int diamDiag = 10;
            int lapLenDiagonal = Armature.GetLapLength(diamDiag, Concrete);
            string posDiag = Block.GetPropValue<string>(PropNameArmDiagonalPos);
            BarDiagonal = new Bar(diamDiag, lapLenDiagonal * 2, 4, posDiag, this, "Диагональные стержни над проемом");
            BarDiagonal.Calc();            
        }

        private void defineSideBrackets ()
        {
            int widthRun = DoorHeight -50;
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
            int countAddArmHor = Block.GetPropValue<int>(PropNameAddArmHorCount);
            int diamAddArmHor = Block.GetPropValue<int>(PropNameAddArmDiam);
            int lapLengthArmHor =  Armature.GetLapLength(diamAddArmHor, Concrete);
            int lengthAddArmHor = DoorWidth + (hasLeftEnd? lapLengthArmHor : 0) +(hasRightEnd? lapLengthArmHor : 0);
            AddArmHor = defineBar(countAddArmHor, lengthAddArmHor, PropNameAddArmDiam, PropNameAddArmHorPos, "Горизонтальные стержни усиления над проемом");
        }

        private void defineAddVerticArm ()
        {
            int countAddVerticArm = (hasLeftEnd? 4:0) + (hasRightEnd? 4:0);
            int lenAddVerticArm = Height+Outline;
            AddArmVertic = defineBar(countAddVerticArm, lenAddVerticArm, PropNameAddArmDiam, PropNameAddArmVerticPos, "Вертикальные стержни усиления проема");
        }

        private void defineAbove ()
        {
            // Определение арматуры надпроемной части
            // Наличие вертик погонной арм - на выпуск
            
            int diamVerticArm = Block.GetPropValue<int>(PropNameArmVerticDiam);
            int stepVerticArm = Block.GetPropValue<int>(PropNameArmVerticStep);
            // Длина нахлести вертик арм (для скобы)
            int lapLengthVerticArm = Armature.GetLapLength(diamVerticArm, Concrete);
            int widthRun = DoorWidth-100;
            if (widthRun <= 0) return;            

            int lengthVerticArm = Height - DoorHeight + Outline -20;
            int bracketLen = lapLengthVerticArm;

            // Если высота стены над проемом больше двух длин нахлеста, то добавляется вертикальная погонная арматура
            if  (Height-DoorHeight> lapLengthVerticArm*2)
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
            int hShackle = (AddArmHor.Count == 4 ? 100: 200) + AddArmHor.Diameter;
            string posShackle = Block.GetPropValue<string>(PropNameShacklePos);
            ShackleTop = new Shackle(diamShackle, wShackle, hShackle, stepShackle, widthRun, 1, posShackle, this);
            ShackleTop.Calc();

            // Горизонтальная погонная арм - Фоновая
            int stepHorArm = Block.GetPropValue<int>(PropNameArmHorStep);
            int widthHorArm = Height - DoorHeight - a - (AddArmHor.Count == 4 ? 100: 200)  - stepHorArm - 50;
            if (widthHorArm > 0)
            {
                ArmHor = defineBarRunStep(DoorWidth, widthHorArm, 2, PropNameArmHorDiam, PropNameArmHorPos,
                    PropNameArmHorStep, Concrete, "Горизонтальная фоновая арматура");
                ArmHor.Calc();
            }
        }        

        private void defineConcrete ()
        {
            var classB =Block.GetPropValue<string>(PropNameConcrete);
            double volume = 0;
            // Объем верхней части            
            volume += (Height - DoorHeight) * DoorWidth * Thickness;
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
