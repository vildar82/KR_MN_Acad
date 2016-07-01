using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Spec.Materials
{
    /// <summary>
    /// Бетон
    /// </summary>
    public abstract class Concrete : IMaterial
    {
        public const string GostNumber = "ГОСТ 26633-2012";
        public const string GostName = "Бетоны тяжелые и мелкозернистые.";
        public const string ClassB25 = "B25";
        public const string ClassB30 = "B30";

        public Gost Gost { get; set; }

        public string Name { get { return $"Бетон {ClassB}"; } }

        /// <summary>
        /// Расход бетона, м3
        /// </summary>
        public double Consumption { get; set; }

        /// <summary>
        /// Класс бетона по прочности на сжатие
        /// </summary>
        public string ClassB { get; set; }
        /// <summary>
        /// Класс бетона по морозостойкости
        /// </summary>
        public string MarkF { get; set; }
        /// <summary>
        /// Класс бетона по водонепроницаемости
        /// </summary>
        public string MarkW { get; set; }
        /// <summary>
        /// Плотность кг/м3
        /// </summary>
        public double D { get; set; }
        /// <summary>
        /// Кол - м3
        /// </summary>
        public double Volume { get; set; }
        /// <summary>
        /// Ед изм - м3
        /// </summary>
        public string Units { get; set; } = "м³";       

        /// <summary>
        /// Парсинг бетона по строке названия
        /// </summary>
        /// <param name="concrete">B25</param>
        public Concrete(string concrete)
        {
            Gost = Gost.GetGost(GostNumber);
            D = 2500;
            // Парсинг строки
            parse(concrete);
        }

        private void parse(string concrete)
        {
            switch (concrete.ToUpper())
            {
                case ClassB25:
                    ClassB = ClassB25;
                    break;
                case ClassB30:
                    ClassB = ClassB30;
                    break;
                default:
                    break;
            }
        }
    }
}
