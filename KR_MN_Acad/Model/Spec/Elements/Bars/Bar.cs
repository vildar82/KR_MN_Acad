using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;
using static AcadLib.Units.UnitsConvertHelper;
using static AcadLib.General;
using KR_MN_Acad.Spec.SpecGroup;
using KR_MN_Acad.Spec.Materials;

namespace KR_MN_Acad.Spec.Elements.Bars
{
    /// <summary>
    /// Одиночный стержень.
    /// </summary>
    public class Bar : Armature, IGroupSpecElement
    {
        protected const int indexBarRunning = 0;
        protected const int indexBar = 1;        

        protected string prefix = string.Empty;
        public string Key { get; set; }
        public virtual int Index { get; set; } = indexBar;
        public string Mark { get; set; }
        public virtual GroupType Group { get; set; } = GroupType.Bars;
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Шаг распределения
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Кол. рядов арматуры
        /// </summary>
        public int Rows { get; set; } = 1;                
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
        public ISpecBlock SpecBlock { get; set; }
        public string FriendlyName { get; set; }

        public new double Amount { get; set; }

        public bool IsDefaultGroupings { get; set; } = true;

        /// <summary>
        /// Стержень по количеству штук
        /// </summary>        
        public Bar(int diam, int len, int count, string pos, ISpecBlock block, string friendlyName) : base(diam, len)
        {
            FriendlyName = friendlyName;
            SpecBlock = block;
            Mark = pos;            
            Count = count;
            Key = GetName(); 
        }
        /// <summary>
        /// стержни распределенные по ширина с шагом и рядами
        /// </summary>        
        public Bar (int diam, int len, int width, int step, int rows, string pos, ISpecBlock block, string friendlyName) 
            : this(diam, len, 1, pos, block, friendlyName)
        {
            Rows = rows;
            Width = width;
            Step = step;
        }

        public virtual void Calc()
        {
            // Кол стержней
            if (Width != 0 && Step!=0)
                Count = CalcCountByStep(Width, Step) * Rows;
            // Масса ед. кг.
            Weight = RoundHelper.Round3Digits(WeightUnit * ConvertMmToMLength(Length));
            // Масса всех стержней
            WeightTotal =RoundHelper.Round2Digits(Weight * Count);
            Amount = WeightTotal;            
        }

        /// <summary>
        /// Определение кол-ва стержней по ширине и шагу
        /// Округление вверх + 1
        /// </summary>
        /// <param name="width">Ширина распределения</param>
        /// <param name="step">Шаг</param>
        /// <returns>Кол стержней в распределении</returns>
        public static int CalcCountByStep (double width, double step)
        {
            if (width == 0 || step == 0) return 1;
            return (int)Math.Ceiling(width / step) + 1;
        }

        public virtual int CompareTo (ISpecElement other)
        {
            var arm = other as Bar;
            if (arm == null)
            {
                return -1;
            }
            var res = Index.CompareTo(arm.Index);
            if (res != 0) return res;

            res = prefix.CompareTo(arm.prefix);
            if (res != 0) return res;

            res = Diameter.CompareTo(arm.Diameter);
            if (res != 0) return res;

            res = Length.CompareTo(arm.Length);
            if (res != 0) return res * (-1);

            res = Class.CompareTo(arm.Class);
            if (res != 0) return res;

            res = Gost.CompareTo(arm.Gost);
            if (res != 0) return res;

            return res;
        }

        public virtual bool Equals (ISpecElement other)
        {
            var arm = other as Bar;
            if (arm == null) return false;
            if (ReferenceEquals(this, arm)) return true;

            return Index == arm.Index &&
                prefix == arm.prefix &&
                Diameter == arm.Diameter &&
                Length == arm.Length &&
                Class == arm.Class &&
                Gost == arm.Gost;
        }

        public virtual string GetDesc()
        {
            // 3, ⌀12, L=3050
            string desc = $"{Mark}, {Symbols.Diam}{Diameter}, L={Length}";            
            if (Step != 0)
                desc += ", ш." + Step;
            desc += ", шт." + Count;
            return desc;
        }

        /// <summary>
        /// Столбез наименования
        /// </summary>        
        public virtual string GetName()
        {
            return Name + " L=" + Length;           
        }        

        public override int GetHashCode()
        {            
            return prefix.GetHashCode() ^ Group.GetHashCode() ^ Index.GetHashCode();
        }

        /// <summary>
        /// Суммирование элементов и запись результата в SpecRow
        /// </summary>        
        public virtual void SumAndSetRow (SpecGroupRow row, List<ISpecElement> elems)
        {
            // Обозначения, Наименования, Кол, Массы ед, примечания
            row.Mark = Mark;
            row.Designation = Gost.Number;
            row.Name = GetName();

            int countTotal = 0;
            double weightTotal = 0;
            foreach (var item in elems)
            {
                var bar = item as Bar;
                countTotal += bar.Count;
                //weightTotal += bar.WeightTotal;
            }
            weightTotal = RoundHelper.Round2Digits(Weight * countTotal);

            row.Count = countTotal.ToString();
            row.Weight = Weight.ToString("N3");
            row.Description = weightTotal.ToString();            
        }
        
        public virtual string GetNumber (string index)
        {
            return prefix + index;            
        }

        public string GetParamInfo ()
        {
            return GetName();
        }

        public void SetNumber (string num)
        {
            Mark = num;
        }

        public Dictionary<string, List<ISpecElement>> GroupsBySize (IGrouping<Type, ISpecElement> indexTypeGroup)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<ISpecElement>> GroupsByArm (List<ISpecElement> value)
        {
            throw new NotImplementedException();
        }
    }
}
