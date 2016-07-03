using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Spec.Materials;
using KR_MN_Acad.Spec.SpecGroup;
using static AcadLib.Units.UnitsConvertHelper;
using static KR_MN_Acad.ConstructionServices.RoundHelper;

namespace KR_MN_Acad.Spec.Elements.Concretes
{
    /// <summary>
    /// Тяжелый бетон
    /// </summary>
    public class ConcreteH : Concrete, IGroupSpecElement
    {
        private const int density = 2500; // Плотность 2500кг/м3
        public string Key { get; set; }
        public string FriendlyName { get; set; }
        public GroupType Group { get; set; } = GroupType.Materials;
        public int Index { get; set; } = 0;
        public string Mark { get; set; } = string.Empty;
        public ISpecBlock SpecBlock { get; set; }

        public double Amount { get; set; }

        public bool IsDefaultGroupings { get; set; } = true;            

        public ConcreteH (string concrete, double volume, ISpecBlock block)
            : base(concrete)
        {
            Key = Name;
            FriendlyName = Name;
            SpecBlock = block;
            Volume = volume;            
        }

        public ConcreteH(string concrete, double len, double width, double height, ISpecBlock block)
            : this(concrete, CalcVolume(len , width, height), block)
        {            
        }
        
        public virtual void Calc()
        {
            Amount = Volume * density;
        }

        public int CompareTo(ISpecElement other)
        {
            var conc = other as Concrete;
            if (conc == null) return -1;

            var res = ClassB.CompareTo(conc.ClassB);
            return res;
        }

        public bool Equals(ISpecElement other)
        {
            var conc = other as Concrete;
            if (conc == null) return false;
            return ClassB.Equals(conc.ClassB);
        }

        public string GetDesc()
        {
            return "";
        } 

        public override int GetHashCode()
        {
            return ClassB.GetHashCode();
        }         

        public void SumAndSetRow (SpecGroupRow row, List<ISpecElement> elems)
        {
            row.Description = Gost.Number;
            row.Name = Name;
            row.Count = Units;
            var volumeTotal = elems.OfType<Concrete>().Sum(c => c.Volume);
            row.Weight = volumeTotal.ToString();
            row.Description = "";            
        }

        public string GetNumber (string index)
        {
            return "";
        }
        public void SetNumber (string num)
        {
            // без позиции            
        }

        public string GetParamInfo ()
        {
            return Name;
        }

        private static double CalcVolume(double len, double width, double height)
        {
            return Round2Digits(0.000000001 * len * width * height);
        }

        public Dictionary<string, List<ISpecElement>> GroupsBySize (IGrouping<int, ISpecElement> indexTypeGroup)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<ISpecElement>> GroupsByArm (List<ISpecElement> value)
        {
            throw new NotImplementedException();
        }
    }
}
