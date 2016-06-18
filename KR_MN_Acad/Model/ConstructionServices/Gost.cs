using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Materials;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// Гост
    /// </summary>
    public class Gost : IComparable<Gost>, IEquatable<Gost>
    {
        private static Dictionary<string, Gost> gosts;
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

        public static Gost GetGost (string number)
        {
            if (gosts == null) gosts = Load();
            if (string.IsNullOrEmpty(number)) return GostEmpty;
            Gost res;
            gosts.TryGetValue(number, out res);
            return res;
        }

        public int CompareTo(Gost other)
        {            
            return Name.CompareTo(other?.Name);
        }

        public bool Equals(Gost other)
        {
            return Name.Equals(other?.Name);
        }

        private static Dictionary<string, Gost> Load()
        {
            return new Dictionary<string, Gost>()
            {                
                {Armature.GostNewNumber,  new Gost(Armature.GostNewNumber,Armature.GostNewName ) },
                {Armature.GostOldNumber,  new Gost(Armature.GostOldNumber, Armature.GostOldName) },
                {Concrete.GostNumber, new Gost(Concrete.GostNumber, Concrete.GostName) },
                {Tube.GostNumber, new Gost(Tube.GostNumber, Tube.GostName) }
            };
        }
    }
}
