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
    public class WallEndCornerBlock : WallBase
	{        
        public const string BlockName = "КР_Арм_Стена_Г-стык";
        
		const string PropNameThickness1 = "Толщина1";
		const string PropNameThickness2 = "Толщина2";		
		const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticCount = "КолВертикАрм";
		const string PropNameBracket1Len = "Скоба1-Длина";
		const string PropNameBracket1Diam = "Скоба1-Диам";
        const string PropNameBracket1Step = "Скоба1-Шаг";
		const string PropNameBracket2Len = "Скоба2-Длина";
		const string PropNameBracket2Diam = "Скоба2-Диам";
		const string PropNameBracket2Step = "Скоба2-Шаг";		   
        const string PropNamePosBracket1 = "ПОЗСКОБЫ1";
		const string PropNamePosBracket2 = "ПОЗСКОБЫ2";		   
        const string PropNameDescBracket1 = "ОПИСАНИЕСКОБЫ1";
		const string PropNameDescBracket2 = "ОПИСАНИЕСКОБЫ2"; 
              
        /// <summary>
        /// Толщина стены 1
        /// </summary>
        public int Thickness1 { get; set; }
		/// <summary>
		/// Толщина стены 2
		/// </summary>
		public int Thickness2 { get; set; }		
        /// <summary>
        /// Кол вертик стержней
        /// </summary>
        public int ArmVerticCount { get; set; }
        /// <summary>
        /// Длина скобы 1
        /// </summary>
        public int Bracket1Length { get; set; }
		/// <summary>
		/// Длина скобы 2
		/// </summary>
		public int Bracket2Length { get; set; }
		/// <summary>
		/// вертикальные арматурные стержни
		/// </summary>
		public Bar ArmVertic { get; set; }        
        /// <summary>
        /// Скоба 1
        /// </summary>
        public Bracket Bracket1 { get; set; }
		/// <summary>
		/// Скоба 2
		/// </summary>
		public Bracket Bracket2 { get; set; }		

        public WallEndCornerBlock (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
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
            AddElement(Bracket1);
            AddElement(Bracket2);
            AddElement(Concrete);
        }

        public override void Numbering()
        {
            // ГорАрм
            FillElemProp(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
            // ВертикАрм
            FillElemProp(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);
            // Скоба 1
            FillElemProp(Bracket1, PropNamePosBracket1, PropNameDescBracket1);
            // Скоба 2
            FillElemProp(Bracket2, PropNamePosBracket2, PropNameDescBracket2);
        }

        private void defineFields()
        {
            Thickness1 = GetPropValue<int>(PropNameThickness1);
			Thickness2 = GetPropValue<int>(PropNameThickness2);
			Height = GetPropValue<int>(PropNameHeight);            
            Outline = GetPropValue<int>(PropNameOutline);            
			ArmVerticCount = GetPropValue<int>(PropNameArmVerticCount);
            var concrete = GetPropValue<string>(PropNameConcrete);
            double volume = getVolume();
            Concrete = new ConcreteH(concrete, volume, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor(Thickness1+Thickness2, PropNameArmHorDiam, PropNamePosHorArm, PropNameArmHorStep);
            // Скоба 1
            Bracket1Length = GetPropValue<int>(PropNameBracket1Len, false);            
            Bracket1 = defineEndBracket(PropNameBracket1Diam, PropNamePosBracket1, PropNameBracket1Step,
                Bracket1Length,Thickness1, ArmVertic.Diameter);
            // Скоба 2
            Bracket2Length = GetPropValue<int>(PropNameBracket2Len, false);
            Bracket2 = defineEndBracket(PropNameBracket2Diam, PropNamePosBracket2, PropNameBracket2Step,
                Bracket2Length, Thickness2, ArmVertic.Diameter);
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

        /// <summary>
        /// Объем бетона - в м3
        /// </summary>        
        private double getVolume ()
        {
            double volume = 0;
            if (ArmVerticCount == 8)
            {
                volume = (Thickness1+100) * (Thickness2+100) * Height * 0.000000001;
                volume -= 0.1 * 0.1 * Height * 0.001;
            }
            else
            {
                volume = Thickness1 * Thickness2 * Height * 0.000000001;
            }            
            return volume;
        }
    }
}