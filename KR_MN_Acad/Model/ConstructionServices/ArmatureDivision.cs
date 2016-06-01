using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// Распределенные стержни на участке
    /// </summary>
    public class ArmatureDivision
    {
        /// <summary>
        /// Стержень с Count
        /// </summary>
        public Armature Armature { get; set; }
        /// <summary>
        /// Ширина распределения
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Шаг распределения
        /// </summary>
        public int Step { get; set; }        

        public ArmatureDivision (int diam, int length, int width, int step)
        {
            Width = width;
            Step = step;
            Armature = new Armature(diam, length);
            CalcCount();
            Armature.PrepareConstTable();
        }        

        private void CalcCount()
        {
            // Кол стержней
            int count = Width / Step + 1;
            Armature.Count = count;                     
        }        
    }
}