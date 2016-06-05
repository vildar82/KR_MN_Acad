﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices;

namespace KR_MN_Acad.Scheme.Materials
{
    /// <summary>
    /// Бетон
    /// </summary>
    public class Concrete : IMaterial
    {
        public const string GostNumber = "ГОСТ 26633-2012";
        public const string GostName = "Бетоны тяжелые и мелкозернистые. Технические условия";

        public Gost Gost { get; set; }

        public string Name
        {
            get
            {
                return $"Бетон {ClassB}";
            }
        }

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
                case "B25":
                    ClassB = "B25";
                    break;
                case "B30":
                    ClassB = "B30";
                    break;
                default:
                    break;
            }
        }
    }
}