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
    /// Торец стены - Т-образный
    /// </summary>
    public class WallEndTBlock : WallBase
	{        
        public const string BlockName = "КР_Арм_Стена_Торец_Т-образный";
                
        const string PropNameLength = "Длина торца";
        const string PropNameThickness = "Толщина стены";        
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticCount = "КолВертикАрм";                
        const string PropNameBracketDiam = "ДиамСкобы";
        const string PropNameBracketStep = "ШагСкобы";
        const string PropNameBracketLen = "ДлинаСкобы";        
        const string PropNamePosShackle = "ПОЗХОМУТА";
        const string PropNamePosBracket = "ПОЗСКОБЫ";        
        const string PropNameDescShackle = "ОПИСАНИЕХОМУТА";
        const string PropNameDescBracket = "ОПИСАНИЕСКОБЫ";

        protected new const string PropNameArmHorDiam = "ДиамГорАрм1";
        protected new const string PropNameArmHorStep = "ШагГорАрм1";        
        protected new const string PropNamePosHorArm = "ПОЗГОРАРМ1";
        protected new const string PropNameDescHorArm = "ОПИСАНИЕГОРАРМ1";
        protected const string PropNameArmHorDiam2 = "ДиамГорАрм2";
        protected const string PropNameArmHorStep2 = "ШагГорАрм2";
        protected const string PropNamePosHorArm2 = "ПОЗГОРАРМ2";
        protected const string PropNameDescHorArm2 = "ОПИСАНИЕГОРАРМ2";

        /// <summary>
        /// Длина стены
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Толщина стены
        /// </summary>
        public int Thickness { get; set; }        
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
        public BarRunningStep ArmHor2 { get; set; }
        /// <summary>
        /// Скоба
        /// </summary>
        public Bracket Bracket { get; set; }        

        public WallEndTBlock (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
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
            AddElement(ArmHor2);
            AddElement(Bracket);
            AddElement(Concrete);
        }

        public override void Numbering()
        {
            // ГорАрм
            FillElemProp(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
            // ГорАрм2
            FillElemProp(ArmHor2, PropNamePosHorArm2, PropNameDescHorArm2);
            // ВертикАрм
            FillElemProp(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);            
            // Скобы
            FillElemProp(Bracket, PropNamePosBracket, PropNameDescBracket);            
        }

        private void defineFields()
        {
            Length = GetPropValue<int>(PropNameLength);
            Height = GetPropValue<int>(PropNameHeight);
            Thickness = GetPropValue<int>(PropNameThickness);
            Outline = GetPropValue<int>(PropNameOutline);            
            ArmVerticCount = GetPropValue<int>(PropNameArmVerticCount);
            var concrete = GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры1
            ArmHor = defineArmHor(Length, PropNameArmHorDiam, PropNamePosHorArm, PropNameArmHorStep);
            // Определние горизонтальной арматуры2
            ArmHor2 = defineArmHor(Thickness, PropNameArmHorDiam2, PropNamePosHorArm2, PropNameArmHorStep2);
            // Скоба
            BracketLength = GetPropValue<int>(PropNameBracketLen, false);
            Bracket = defineBracket(PropNameBracketDiam, PropNamePosBracket, PropNameBracketStep,
               BracketLength, Thickness, ArmVertic.Diameter);
        }

        private Bar defineArmVertic()
        {
            if (ArmVerticCount == 0)
                return null;
            int diam = GetPropValue<int>(PropNameArmVerticDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(PropNamePosVerticArm);            
            int len = Height + Outline;
            var arm = new Bar(diam, len, pos, this, "Вертикальная арматура");
            arm.Count = ArmVerticCount;
            arm.Calc();
            return arm;
        }                
    }
}
