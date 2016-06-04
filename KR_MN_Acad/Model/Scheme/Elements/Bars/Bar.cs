using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.ConstructionServices.Materials;
using KR_MN_Acad.Scheme.Spec;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Одиночный стержень.
    /// </summary>
    public class Bar : Armature, IElement
    {
        protected static Dictionary<Type, int> barTypes = new Dictionary<Type, int>
        {
            { typeof(BarRunning), 1 },
            { typeof(BarRunningStep), 1},
            { typeof(BarDivision), 2},
            { typeof(Bar) ,2}
        };

        public string Prefix { get; set; } = "";

        /// <summary>
        /// Длина стержня
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Кол стержней
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Масса ед. кг.
        /// Округлено до 2 знаков
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Масса всех стержней
        /// </summary>
        public double WeightTotal { get; set; }
        public ISpecRow SpecRow { get; set; }
        public GroupType Type { get; set; }

        public Bar(int diam, int len) : base(diam)
        {
            Type = GroupType.Armatures;
            Length = len;
            Count = 1;
        }

        public virtual void Calc()
        {
            // Масса ед. кг.
            Weight = RoundHelper.RoundSpec(WeightUnit * ConvertMmToMLength(Length));
            // Масса всех стержней
            WeightTotal = Weight * Count;
        }

        public virtual int CompareTo(IElement other)
        {
            var arm = other as Bar;
            if (arm == null)
            {
                return -1;
            }            
            var res = CompareBarClasses(this, arm);
            if (res == 0)
            {
                var result = Diameter.CompareTo(arm.Diameter);
                if (result != 0) return result;

                result = Length.CompareTo(arm.Length);
                if (result != 0) return result*(-1);

                result = Class.CompareTo(arm.Class);
                if (result != 0) return result;

                result = Gost.CompareTo(arm.Gost);
                if (result != 0) return result;

                return result;
            }
            else
            {
                return res;
            }
        }

        public virtual bool Equals(IElement other)
        {
            var arm = other as Bar;
            if (arm == null)
                return false;

            if (EqualBarClasses(this, arm))
            {
                return Diameter.Equals(arm.Diameter) &&
                    Length.Equals(arm.Length) &&
                    Class.Equals(arm.Class) &&
                    Gost.Equals(arm.Gost);
            }
            else
            {
                return false;
            }
        }

        public virtual string GetDesc()
        {
            return $"{SpecRow?.PositionColumn}, {GetName()}";
        }

        /// <summary>
        /// Сравнение типов унаследованных от Bar
        /// </summary>        
        protected int CompareBarClasses(Bar bar1, Bar bar2)
        {
            int index1 = GetIndex(bar1);
            int index2 = GetIndex(bar2);
            return index1.CompareTo(index2);
        }

        protected static int GetIndex(Bar bar)
        {
            int index;
            barTypes.TryGetValue(bar.GetType(), out index);
            return index;
        }

        protected bool EqualBarClasses(Bar bar1, Bar bar2)
        {
            return CompareBarClasses(bar1, bar2) == 0;
        }

        public virtual double GetCount()
        {
            return Count;
        }

        public virtual double GetWeightTotal()
        {
            return WeightTotal;
        }

        public virtual double GetWeight()
        {
            return Weight;
        }

        public virtual string GetName()
        {
            return Name + " L=" + Length;           
        }        

        public override int GetHashCode()
        {            
            return Prefix.GetHashCode() ^ Type.GetHashCode() ^ GetIndex(this).GetHashCode();
        }
    }
}
