using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Elements.Concretes;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока стены "КР_Арм_Стена"
    /// </summary>
    public class WallBlock : WallBase
    {        
        public const string WallBlockName = "КР_Арм_Стена";                          
        
        const string PropNameLength = "Длина стены";
        const string PropNameThickness = "Толщина стены";        
        const string PropNameArmVerticDiam = "ДиамВертикАрм";
        const string PropNameArmVerticStep = "ШагВертикАрм";        
        const string PropNameSpringDiam = "ДиамШпилек";
        const string PropNameSpringStepHor = "ШагШпилекГор";
        const string PropNameSpringStepVertic = "ШагШпилекВерт";        
        const string PropNamePosSpring = "ПОЗШПИЛЬКИ";        
        const string PropNameDescSpring = "ОПИСАНИЕШПИЛЬКИ";       
           
        /// <summary>
        /// Длина стены
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Толщина стены
        /// </summary>
        public int Thickness { get; set; }        
        /// <summary>
        /// Распределенные вертикальные арматурные стержни
        /// </summary>
        public BarDivision ArmVertic { get; set; }        
        /// <summary>
        /// Шпильки
        /// </summary>
        public Spring Spring { get; set; }        

        public WallBlock(BlockReference blRef, string blName, SchemeService service) : base (blRef, blName, service)
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
            catch(Exception ex)
            {
                AddError(ex.Message);
            }
        }

        private void AddElements()
        {           
            AddElement(ArmHor);
            AddElement(ArmVertic);
            AddElement(Spring);
            AddElement(Concrete);
        }

        private void defineFields()
        {
            Length = Convert.ToInt32(GetPropValue<double>(PropNameLength));
            Height = Convert.ToInt32(GetPropValue<double>(PropNameHeight));            
            Thickness = Convert.ToInt32(GetPropValue<double>(PropNameThickness));
            Outline = Convert.ToInt32(GetPropValue<double>(PropNameOutline));
            var concrete = GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineArmVertic();
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor(Length);
            // Шпильки
            Spring = defineSpring();
        }        

        private BarDivision defineArmVertic ()
        {
            int diam = GetPropValue<int>(PropNameArmVerticDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(PropNamePosVerticArm);            
            int step = GetPropValue<int>(PropNameArmVerticStep);
            int width = getWidthVerticArm(step, Length);
            int len = Height + Outline;            
            var armDiv = new BarDivision(diam, len, width, step, 2, pos, this, "Вертикальная арматура");
            armDiv.Calc();            
            return armDiv;
        }        

        private Spring defineSpring()
        {
            int diam = GetPropValue<int>(PropNameSpringDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(PropNamePosSpring);            
            int stepHor = GetPropValue<int>(PropNameSpringStepHor);
            int stepVert = GetPropValue<int>(PropNameSpringStepVertic);            

            // ширина распределения шпилек по горизонтале
            int widthHor = Length;
            int widthVertic = Height;
            var lRabSpring =  Thickness - 2 * a;
            Spring sp = new Spring(diam, lRabSpring, stepHor, stepVert, widthHor, widthVertic, pos, this);
            sp.Calc();
            return sp;
        } 

        /// <summary>
        /// Заполнение позиций
        /// </summary>
        public override void Numbering()
        {
            // ГорАрм         
            FillElemProp(ArmHor, PropNamePosHorArm, PropNameDescHorArm);
            // ВертикАрм         
            FillElemProp(ArmVertic, PropNamePosVerticArm, PropNameDescVerticArm);
            // Шпилька         
            FillElemProp(Spring, PropNamePosSpring, PropNameDescSpring);           
        }        
    }
}