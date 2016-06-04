using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Spec;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Арматура измеряемая поганажом
    /// </summary>
    public class BarRunning : Bar
    {        
        /// <summary>
        /// Количество погонных метров
        /// </summary>
        public double Meters { get; set; }       

        public BarRunning(int diam) : base(diam, 1)
        {            
        }

        public BarRunning(int diam, double meters) : base(diam, 0)
        {
            Meters = RoundHelper.RoundSpec(meters);            
        }        

        public override void Calc()
        {
            // Масса ед
            Weight = RoundHelper.RoundSpec(WeightUnit);
            // Масса общая
            WeightTotal = RoundHelper.RoundSpec(Weight * Meters);
        }

        public override int CompareTo(IElement other)
        {
            var arm = other as BarRunning;
            if (arm == null)
            {
                if (other is Bar)
                {
                    return CompareBarClasses(this, (Bar)other);
                }
                return -1;
            }
            else
            {
                var result = Diameter.CompareTo(arm.Diameter);
                if (result != 0) return result;                

                result = Class.CompareTo(arm.Class);
                if (result != 0) return result;

                result = Gost.CompareTo(arm.Gost);
                if (result != 0) return result;

                return result;
            }
        }

        public override bool Equals(IElement other)
        {
            var arm = other as BarRunning;
            if (arm == null)
            {
                if (other is Bar)
                {
                    return EqualBarClasses(this, (Bar)other);
                }
                return false;
            }
            return Diameter.Equals(arm.Diameter) &&                
                Class.Equals(arm.Class) &&
                Gost.Equals(arm.Gost);
        }

        public override string GetDesc()
        {
            return $"{SpecRow?.PositionColumn}, {GetName()}{Meters}";
        }

        public override double GetCount()
        {
            return Meters;
        }

        public override string GetName()
        {
            return Name + " L=п.м.";
        }
    }
}
