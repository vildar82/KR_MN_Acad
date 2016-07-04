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
        private static Gost GostEmpty = new Gost("", "");
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

        public int CompareTo(Gost other)
        {            
            return AcadLib.Comparers.AlphanumComparator.New.Compare(Name,other?.Name);
        }

        public bool Equals(Gost other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name.Equals(other?.Name);
        }        
    }
}
