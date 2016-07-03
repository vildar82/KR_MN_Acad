using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using static AcadLib.Units.UnitsConvertHelper;
using static AcadLib.General;
using KR_MN_Acad.Spec.Materials;
using KR_MN_Acad.Spec.SpecGroup;

namespace KR_MN_Acad.Spec.Elements
{
    /// <summary>
    /// Труба стальная
    /// </summary>
    public class Tube : RollSteel, IGroupSpecElement
    {
        public const string GostNumber = "ГОСТ 10704-91";
        public const string GostName = "Трубы стальные электросварные прямошовные.";
        public static Gost GostElectricWelded = new Gost (GostNumber, GostName);

        public string Key { get; set; }
        public int Index { get; set; } = 0;
        public GroupType Group { get; set; } = GroupType.Sleeves;
        public ISpecBlock SpecBlock { get; set; }
        public string FriendlyName { get; set; }

        /// <summary>
        /// Марка  (Г1, Г1.1 и т.п.)
        /// </summary>
        public string Mark { get; set; } = "";
        /// <summary>
        /// Длина трубы
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Диаметр
        /// </summary>
        public int Diametr { get; set; }
        /// <summary>
        /// Толщина стенки
        /// </summary>
        public int Thickness { get; set; }
        /// <summary>
        /// Масса 1 п.м.,кг
        /// </summary>
        public double WeightUnit { get; set; }
        /// <summary>
        /// Масса ед. кг.
        /// Округлено до 2 знаков
        /// </summary>
        public double Weight { get; set; }
        //public double Amount { get; set; }

        public bool IsDefaultGroupings { get; set; } = true;

        /// <summary>
        /// Труба стальная электросварная прямошовная.
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="t">Толщина стенки</param>
        public Tube (int diam, int t, int length, double weightUnit, ISpecBlock block) : 
            base(GostElectricWelded, Symbols.Diam + diam + "х" + t)
        {
            SpecBlock = block;            
            Diametr = diam;
            Thickness = t;
            Length = length;
            WeightUnit = weightUnit;
            FriendlyName = "Труба " + Name;
            Key = Name;
        }       

        public void Calc ()
        {
            // Масса ед. кг.
            Weight = RoundHelper.Round3Digits(WeightUnit * ConvertMmToMLength(Length));
            Amount = Weight;
        }        

        public string GetDesc ()
        {
            return "";
        }        

        public int CompareTo (ISpecElement other)
        {
            var tube = other as Tube;
            if (tube == null) return -1;            

            var result = Diametr.CompareTo(tube.Diametr);
            if (result != 0) return result;

            result = Thickness.CompareTo(tube.Thickness);
            if (result != 0) return result;

            result = Length.CompareTo(tube.Length);
            if (result != 0) return result ;

            return 0;
        }

        public bool Equals (ISpecElement other)
        {
            var tube = other as Tube;
            if (tube == null) return false;
            return Diametr == tube.Diametr &&
                   Thickness == tube.Thickness &&
                   Length == tube.Length;
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode();
        }

        public void SumAndSetRow (SpecGroupRow row, List<ISpecElement> elems)
        {
            // Обозначения, Наименования, Кол, Массы ед, примечания
            row.Description = Gost.Number;
            row.Name = $"Труба {Name}, L={Length}";

            int countTotal = elems.Count;
            double weightTotal = RoundHelper.Round2Digits(Weight * countTotal);

            row.Count = countTotal.ToString();
            row.Weight = Weight.ToString("0.000");
            row.Description = weightTotal.ToString();            
        }

        public string GetNumber (string index)
        {
            return "";
        }

        public void SetNumber (string num)
        {            
        }

        public string GetParamInfo ()
        {
            return Name;
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
