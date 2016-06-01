using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// Гост
    /// </summary>
    public class Gost
    {
        /// <summary>
        /// Номер госта "ГОСТ Р 52544-2006"
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Наименование - "Прокат арматурный свариваемый периодического профиля классов А500С и В500С для армирования железобетонных конструкций. Технические условия"
        /// </summary>
        public string Name { get; set; }

        public Gost (string number, string name)
        {
            Number = number;
            Name = name;
        }

        public static Gost GetGost (string number)
        {
            switch (number.ToUpper())
            {
                case "ГОСТ Р 52544-2006":
                    return new Gost("ГОСТ Р 52544-2006", "Прокат арматурный свариваемый периодического профиля классов А500С и В500С для армирования железобетонных конструкций. Технические условия");
                default:
                    return new Gost("", "");                    
            }
        }
    }
}
