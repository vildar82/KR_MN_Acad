using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;

namespace KR_MN_Acad.Scheme.Wall
{
	public abstract class WallBase : SchemeBlock
	{        
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
		protected const string PropNamePosVerticBentDirect = "ПОЗВЕРТИКГС";
		protected const string PropNameDescVerticBentDirect = "ОПИСАНИЕВЕРТИКГС";
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
		public BarRunning ArmHor { get; set; }
		/// <summary>
		/// Бетон
		/// </summary>
		public ConcreteH Concrete { get; set; }
		/// <summary>
		/// Гнутые вертикальные стержни
		/// </summary>
		public BentBarDirect BentBarDirect { get; set; }
		/// <summary>
		/// вертикальные арматурные стержни
		/// </summary>
		public Bar ArmVertic { get; set; }

		public WallBase (BlockReference blRef, string blName, SchemeService service) : base(blRef, blName, service)
		{
		}

		public override void Numbering ()
		{
			// ГорАрм         
			FillElemProp(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
			// ВертикАрм         
			FillElemProp(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);
			// ВертикГс
			FillElemProp(BentBarDirect, PropNamePosVerticBentDirect, PropNameDescVerticBentDirect);
		}

		protected virtual void AddElements ()
		{
			AddElement(ArmHor);
			AddElement(ArmVertic);
			AddElement(BentBarDirect);
			AddElement(Concrete);
		}

		/// <summary>
		/// Определение ширины распределения вертикальных стержней в стене
		/// </summary>        
		protected int getWidthVerticArm (int step, int length)
		{
			// Вычесть отступ у торцов стены = шаг вертик стержней - а.            
			return length - (step - a) * 2;
		}

		/// <summary>
		/// Вертикальные отдельные стержени
		/// </summary>
		/// <param name="count">Кол</param>
		/// <param name="propDiam">Парам диам</param>
		/// <param name="propPos">Парам поз</param>        
		protected Bar defineVerticArm (int count, string propDiam, string propPos)
		{
			return defineBar(count, Height + Outline, propDiam, propPos, "Вертикальная арматура");
		}

		/// <summary>
		/// Вертикальная распределенная арматура
		/// </summary>
		/// <param name="length">Длина по бетону (отступ вычисляется тут)</param>
		/// <param name="propDiam">Парам диам</param>
		/// <param name="propStep">Парам шаг</param>
		/// <param name="propPos">Парам поз</param>        
		protected Bar defineVerticArm (int length, string propDiam, string propStep, string propPos)
		{
			int step = GetPropValue<int>(propStep);
			return defineBarDiv(Height + Outline, getWidthVerticArm(step, length), step, propDiam, propPos, 2, "Вертикальная арматура");
		}

		protected BarRunning defineArmHor (int length, string propDiam, string propPos, string propStep)
		{
			int widthRun = Height - 100;
			return defineBarRunStep(length, widthRun, 2, propDiam, propPos, propStep, Concrete, "Горизонтальная арматура");
		}

		protected Bracket defineEndBracket (string propDiam, string propPos,
			string propStep, int bracketLen, int thickness, int diamVerticArm)
		{            
			int wBracket = Height;
			return defineBracket(propDiam, propPos, propStep, bracketLen, thickness, a, wBracket, diamVerticArm);
		}

		/// <summary>
		/// Определение вертикальных гнутых стержней.        
		/// </summary>
		protected void checkBentBarDirect (Bar armVertic, int countBent)
		{
			if (Requirements.IsNeedToBentVerticArm(armVertic.Diameter))
			{
				armVertic.Count -= countBent;
				armVertic.Calc();
				BentBarDirect = defineBentDirect(armVertic.Diameter, countBent, Height, Outline, PropNamePosVerticBentDirect);
			}
		}
	}
}
