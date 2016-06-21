using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using KR_MN_Acad.Scheme.Materials;
using KR_MN_Acad.Scheme.Spec;
using static AcadLib.Units.UnitsConvertHelper;
using static AcadLib.General;

namespace KR_MN_Acad.Scheme.Elements
{
    /// <summary>
    /// Труба стальная
    /// </summary>
    public class Tube : RollSteel, IElement
    {
        public const string GostNumber = "ГОСТ 10704-91";
        public const string GostName = "Трубы стальные электросварные прямошовные.";
        public static Gost GostElectricWelded = new Gost (GostNumber, GostName);

        public ISchemeBlock Block { get; set; }
        public string FriendlyName { get; set; }
        public string PositionInBlock { get; set; }
        public string Prefix { get; set; } = "";
        public ISpecRow SpecRow { get; set; }
        public GroupType Type { get; set; } = GroupType.Sleeves;

        /// <summary>
        /// Марка  (Г1, Г1.1 и т.п.)
        /// </summary>
        public string Mark { get; set; }
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

        /// <summary>
        /// Труба стальная электросварная прямошовная.
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="t">Толщина стенки</param>
        public Tube (string mark, int diam, int t, int length, double weightUnit, ISchemeBlock block) : base(GostElectricWelded, Symbols.Diam + diam + "х" + t)
        {
            Block = block;
            Mark = mark;
            Diametr = diam;
            Thickness = t;
            Length = length;
            WeightUnit = weightUnit;
            FriendlyName = Name;
            PositionInBlock = Name;
        }       

        public void Calc ()
        {
            // Масса ед. кг.
            Weight = RoundHelper.Round3(WeightUnit * ConvertMmToMLength(Length));
        }        

        public string GetDesc ()
        {
            return "";
        }

        public void Sum (List<IElement> elems)
        {
            // Обозначения, Наименования, Кол, Массы ед, примечания
            SpecRow.DocumentColumn = Gost.Number;
            SpecRow.NameColumn = $"Труба {Name}, L={Length}";

            int countTotal = elems.Count;
            double weightTotal = RoundHelper.Round2(Weight * countTotal);

            SpecRow.CountColumn = countTotal.ToString();
            SpecRow.WeightColumn = Weight.ToString("0.000");
            SpecRow.DescriptionColumn = weightTotal.ToString();
            SpecRow.Amount = weightTotal;
        }

        public int CompareTo (IElement other)
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

        public bool Equals (IElement other)
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

        public string GetPosition (int posIndex, IEnumerable<IElement> items, bool isNumbering)
        {
            //// группировка труб по марке            
            //var marks = items.OfType<Tube>().GroupBy(g=>g.Mark).Select(s=>s.First().Mark).OrderBy(o=>o, AcadLib.Comparers.AlphanumComparator.New);
            //string pos = string.Join(";", marks);
            //return pos;
            return "";
        }

        public void SortRowsSpec (List<ISpecRow> rows)
        {
            // rows - строки спецификации одой группы - закладнве детали
            //rows.Sort((r1, r2) => AcadLib.Comparers.AlphanumComparator.New.Compare(r1.PositionColumn, r2.PositionColumn));
        }
    }
}
