using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Materials;
using KR_MN_Acad.Scheme.Spec;
using static AcadLib.Units.UnitsConvertHelper;
using static KR_MN_Acad.ConstructionServices.RoundHelper;

namespace KR_MN_Acad.Scheme.Elements.Concretes
{
    /// <summary>
    /// Тяжелый бетон
    /// </summary>
    public class ConcreteH : Concrete, IElement
    {
        public string FriendlyName { get; set; }
        public string Prefix { get; set; }
        public string PositionInBlock { get; set; }
        public ISpecRow SpecRow { get; set; }
        public GroupType Type { get; set; } = GroupType.Materials;
        public ISchemeBlock Block { get; set; }

        public ConcreteH (string concrete, double volume, ISchemeBlock block)
            : base(concrete)
        {
            FriendlyName = Name;
            Block = block;
            Volume = volume;
        }

        public ConcreteH(string concrete, double len, double width, double height, ISchemeBlock block)
            : this(concrete, Round2(len * width * height * 0.000000001), block)
        {            
        }
        
        public virtual void Calc()
        {            
        }

        public int CompareTo(IElement other)
        {
            var conc = other as Concrete;
            if (conc == null) return -1;

            var res = ClassB.CompareTo(conc.ClassB);
            return res;
        }

        public bool Equals(IElement other)
        {
            var conc = other as Concrete;
            if (conc == null) return false;
            return ClassB.Equals(conc.ClassB);
        }

        public string GetDesc()
        {
            return "";
        }        

        public void Sum(List<IElement> elems)
        {
            SpecRow.DocumentColumn = Gost.Number;
            SpecRow.NameColumn = Name;
            SpecRow.CountColumn = Units;
            var volumeTotal = elems.OfType<Concrete>().Sum(c => c.Volume);
            SpecRow.WeightColumn = volumeTotal.ToString();
            SpecRow.DescriptionColumn = "";
            SpecRow.Amount = volumeTotal;
        }

        public override int GetHashCode()
        {
            return ClassB.GetHashCode();
        }
    }
}
