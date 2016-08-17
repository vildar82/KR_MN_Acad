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
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
	public abstract class WallBase : SpecBlock
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
		protected const string PropNameArmVerticPos = "ПОЗВЕРТИКАРМ";
		protected const string PropNameArmHorPos = "ПОЗГОРАРМ";
		protected const string PropNameArmHorDesc = "ОПИСАНИЕГОРАРМ";
		protected const string PropNameArmVerticDesc = "ОПИСАНИЕВЕРТИКАРМ";
		protected const string PropNameVerticBentDirectPos = "ПОЗВЕРТИКГС";
		protected const string PropNameVerticBentDirectDesc = "ОПИСАНИЕВЕРТИКГС";
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
		public virtual BarRunning ArmHor { get; set; }
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

		public WallBase (BlockReference blRef, string blName) : base(blRef, blName)
		{
		}

		public override void Numbering ()
		{
			// ГорАрм         
			FillElemPropNameDesc(ArmHor, PropNameArmHorPos, PropNameArmHorDesc);
			// ВертикАрм         
			FillElemPropNameDesc(ArmVertic, PropNameArmVerticPos, PropNameArmVerticDesc);
			// ВертикГс
			FillElemPropNameDesc(BentBarDirect, PropNameVerticBentDirectPos, PropNameVerticBentDirectDesc);
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
            var res = length - (step - a) * 2;
            if (res < 0) res = 0;
            return res;
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
		/// <param name="length">Ширина распределения по бетону (отступ вычисляется тут)</param>
		/// <param name="propDiam">Парам диам</param>
		/// <param name="propStep">Парам шаг</param>
		/// <param name="propPos">Парам поз</param>        
		protected Bar defineVerticArm (int length, string propDiam, string propStep, string propPos)
		{
			int step = Block.GetPropValue<int>(propStep);
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
			if (bracketLen == 0) return null;
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
				//armVertic.Count -= countBent;
				armVertic.AddCount(-countBent);
				//if (armVertic.Count == 0) Elements.Remove(armVertic);
				BentBarDirect = defineBentDirect(armVertic.Diameter, countBent, Height, Outline, PropNameVerticBentDirectPos);
			}
		}
	}
}
