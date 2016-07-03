using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Spec;
using KR_MN_Acad.Spec.Elements.Bars;
using KR_MN_Acad.Spec.Elements.Concretes;

namespace KR_MN_Acad.Spec.ArmWall.Blocks
{
    /// <summary>
    /// Описание блока стены "КР_Арм_Стена"
    /// </summary>
    public class WallBlock : WallBase
    {        
        public const string BlockName = "КР_Арм_Стена";                          
        
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
        /// Шпильки
        /// </summary>
        public Spring Spring { get; set; }        

        public WallBlock(BlockReference blRef, string blName) : base (blRef, blName)
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

        protected override void AddElements()
        {
            base.AddElements();
            AddElement(Spring);
        }

        private void defineFields()
        {
            Length = Block.GetPropValue<int>(PropNameLength);
            Height = Block.GetPropValue<int>(PropNameHeight);
            Thickness = Block.GetPropValue<int>(PropNameThickness);
            Outline = Block.GetPropValue<int>(PropNameOutline);
            var concrete =Block.GetPropValue<string>(PropNameConcrete);
            Concrete = new ConcreteH(concrete, Length, Thickness, Height, this);
            Concrete.Calc();
            // Определние вертикальной арматуры
            ArmVertic = defineVerticArm(Length, PropNameArmVerticDiam, PropNameArmVerticStep, PropNamePosVerticArm);
            // Определние горизонтальной арматуры
            ArmHor = defineArmHor(Length, PropNameArmHorDiam, PropNamePosHorArm, PropNameArmHorStep);
            // Шпильки
            Spring = defineSpring(PropNameSpringDiam, PropNamePosSpring, PropNameSpringStepHor, PropNameSpringStepVertic,
                Thickness, a, Length, Height);
        }                

        /// <summary>
        /// Заполнение позиций
        /// </summary>
        public override void Numbering()
        {
            base.Numbering();
            // Шпилька         
            FillElemPropNameDesc(Spring, PropNamePosSpring, PropNameDescSpring);           
        }        
    }
}