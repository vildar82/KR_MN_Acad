using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Spec.Slab
{
    /// <summary>
    /// строка таблицы отверстий в плите
    /// </summary>
    public class SlabRow : ISpecElement
    {
        private int side1;
        private int side2;

        public string Mark { get; set; }
        public string Dimension { get; set; }
        public string Role { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }        

        public SlabRow(string mark, int side1, int side2, string role)
        {
            Mark = mark;
            
            Role = role;
        }

        public bool Equals (ISpecElement other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo (ISpecElement other)
        {
            throw new NotImplementedException();
        }
    }
}
