using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
	/// <summary>
	/// Торец стены - Т-образный
	/// </summary>
	public class WallEndTBlock : WallBase
	{        
		public const string BlockName = "КР_Арм_Стена_Т-стык";
				
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
		/// Горизонтальные арматурные стержни - погоннаж
		/// </summary>
		public BarRunning ArmHor2 { get; set; }
		/// <summary>
		/// Скоба
		/// </summary>
		public Bracket Bracket { get; set; }        

		public WallEndTBlock (BlockReference blRef, string blName) : base(blRef, blName)
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

		protected override void AddElements()
		{
			base.AddElements();				
			AddElement(ArmHor2);
			AddElement(Bracket);			
		}

		public override void Numbering()
		{
            // Base Numbering - т.к. имена параметров горизонтальной арматуры переименованы.
            // ГорАрм         
            FillElemPropNameDesc(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
            // ВертикАрм         
            FillElemPropNameDesc(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);
            // ВертикГс
            FillElemPropNameDesc(BentBarDirect, PropNamePosVerticBentDirect, PropNameDescVerticBentDirect);


            // ГорАрм2
            FillElemPropNameDesc(ArmHor2, PropNamePosHorArm2, PropNameDescHorArm2);
			// Скобы
			FillElemPropNameDesc(Bracket, PropNamePosBracket, PropNameDescBracket);            
		}

		private void defineFields()
		{
			Length = Block.GetPropValue<int>(PropNameLength);
			Height = Block.GetPropValue<int>(PropNameHeight);
			Thickness = Block.GetPropValue<int>(PropNameThickness);
			Outline = Block.GetPropValue<int>(PropNameOutline);            
			ArmVerticCount = Block.GetPropValue<int>(PropNameArmVerticCount);
			var concrete = Block.GetPropValue<string>(PropNameConcrete);
			Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
			Concrete.Calc();
			// Определние вертикальной арматуры
			ArmVertic = defineVerticArm(ArmVerticCount, PropNameArmVerticDiam, PropNamePosVerticArm);
			// Определние горизонтальной арматуры1
			ArmHor = defineArmHor(Length, PropNameArmHorDiam, PropNamePosHorArm, PropNameArmHorStep);
			// Определние горизонтальной арматуры2
			ArmHor2 = defineArmHor(Thickness, PropNameArmHorDiam2, PropNamePosHorArm2, PropNameArmHorStep2);
			// Скоба
			BracketLength = Block.GetPropValue<int>(PropNameBracketLen, false);
			Bracket = defineEndBracket(PropNameBracketDiam, PropNamePosBracket, PropNameBracketStep,
			   BracketLength, Thickness, ArmVertic.Diameter);
		}		
	}
}
