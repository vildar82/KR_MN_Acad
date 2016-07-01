using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Constructions
{
    /// <summary>
    /// Габариты конструкции по бетону
    /// </summary>
    public class ConstructionSize : IConstructionSize
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Key { get; set; }

        private double volume;

        public ConstructionSize(int len, int width, int height)
        {
            Length = len;
            Width = width;
            Height = height;
            volume = len * width * height;
            Key = Length.ToString() + Width.ToString() + Height.ToString();
        }

        public bool Equals (IConstructionSize other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Length == other.Length && Width == other.Width && Height == other.Height;
        }

        public int CompareTo (IConstructionSize other)
        {
            var res = Length.CompareTo(other.Length);
            if (res != 0) return res;

            res = Width.CompareTo(other.Width);
            if (res != 0) return res;

            res = Height.CompareTo(other.Height);
            return res;
        }        

        public override int GetHashCode ()
        {
            return volume.GetHashCode();
        } 
    }
}
