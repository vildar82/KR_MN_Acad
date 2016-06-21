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
    public class WallEndBlock : WallBase
	{        
        public const string BlockName = "КР_Арм_Стена_Торец";
                
        const string PropNameLength = "Длина торца";
        const string PropNameThickness = "Толщина стены";        
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticCount = "КолВертикАрм";        
        const string PropNameShackleDiam = "ДиамХомута";
        const string PropNameShackleStep = "ШагХомута";
        const string PropNameBracketDiam = "ДиамСкобы";
        const string PropNameBracketStep = "ШагСкобы";
        const string PropNameBracketLen = "ДлинаСкобы";        
        const string PropNamePosShackle = "ПОЗХОМУТА";
        const string PropNamePosBracket = "ПОЗСКОБЫ";        
        const string PropNameDescShackle = "ОПИСАНИЕХОМУТА";
        const string PropNameDescBracket = "ОПИСАНИЕСКОБЫ";	          
        
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
        /// вертикальные арматурные стержни
        /// </summary>
        public Bar ArmVertic { get; set; }        
        /// <summary>
        /// Хомут
        /// </summary>
        public Shackle Shackle { get; set; }
        /// <summary>
        /// Скоба
        /// </summary>
        public Bracket Bracket { get; set; }        

        public WallEndBlock(BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
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
            // ГорАрм
            FillElemProp(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
            // ВертикАрм
            FillElemProp(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);
            // Хомут
            FillElemProp(Shackle, PropNamePosShackle, PropNameDescShackle);
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
            ArmVertic = defineVerticArm(ArmVerticCount, PropNameArmVerticDiam, PropNamePosVerticArm);
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor(Length, PropNameArmHorDiam, PropNamePosHorArm, PropNameArmHorStep);
            // Хомут
            Shackle = defineShackle(Length, Thickness, Height, ArmVertic.Diameter, a, PropNameShackleDiam, PropNamePosShackle,
               PropNameShackleStep);
            // Скоба
            BracketLength = GetPropValue<int>(PropNameBracketLen, false);
            Bracket = defineEndBracket(PropNameBracketDiam, PropNamePosBracket, PropNameBracketStep,
               BracketLength, Thickness, ArmVertic.Diameter);
        }

        //private Bar defineArmVertic()
        //{
        //    if (ArmVerticCount == 0)
        //        return null;
        //    int diam = GetPropValue<int>(PropNameArmVerticDiam);
        //    if (diam == 0) return null;
        //    string pos = GetPropValue<string>(PropNamePosVerticArm);            
        //    int len = Height + Outline;
        //    var arm = new Bar(diam, len, pos, this, "Вертикальная арматура");
        //    arm.Count = ArmVerticCount;
        //    arm.Calc();
        //    return arm;
        //}        

        //private Shackle defineEndShackle ()
        //{
        //    int diam = GetPropValue<int>(PropNameShackleDiam, false);
        //    if (diam == 0) return null;
        //    string pos = GetPropValue<string>(PropNamePosShackle, false);
        //    int step = GetPropValue<int>(PropNameShackleStep);
        //    // ширина распределения шпилек
        //    int range = Height;
        //    // длина хомута
        //    int lShackle =  Length-(2*a)+ArmVertic.Diameter;
        //    int hShackle = Thickness-(2*a)+ArmVertic.Diameter;
        //    Shackle s = new Shackle(diam, lShackle, hShackle, step, range, pos, this);
        //    s.Calc();
        //    return s;            
        //}
    }
}
