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
    public class Gost : IComparable<Gost>, IEquatable<Gost>
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
                case Materials.Armature.GostNewNumber:
                    return new Gost(Materials.Armature.GostNewNumber, "Прокат арматурный свариваемый периодического профиля классов А500С и В500С для армирования железобетонных конструкций. Технические условия");
                case Materials.Armature.GostOldNumber:
                    return new Gost(Materials.Armature.GostOldNumber, "ГОСТ 5781-82.Сталь горячекатаная для армирования железобетонных конструкций.");                    
                default:
                    return new Gost("", "");                    
            }
        }

        public int CompareTo(Gost other)
        {            
            return Name.CompareTo(other?.Name);
        }

        public bool Equals(Gost other)
        {
            return Name.Equals(other?.Name);
        }
    }
}
