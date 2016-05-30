using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.ConstructionServices
{
    /// <summary>
    /// арматурный стержень
    /// </summary>
    public class Armature
    {
        public static int DefaultDiameter { get; set; } = 10;
        public static string DefaultClass { get; set; } = "A500C";

        /// <summary>
        /// Диаметр
        /// </summary>
        public int Diameter { get; private set; }
        /// <summary>
        /// Масса 1 п.м.,кг
        /// </summary>
        public double Weight { get; private set; }
        /// <summary>
        /// Площадь поперечного сечения, см2
        /// </summary>
        public double Area { get; private set; }
        /// <summary>
        /// Номер госта ГОСТ 5781-82
        /// </summary>
        public string Gost { get; set; } = "ГОСТ 5781-82";
        /// <summary>
        /// Полное назване госта
        /// </summary>
        public string GostFull { get; set; } = "ГОСТ 5781-82. Сталь горячекатаная для армирования железобетонных конструкций.";
        /// <summary>
        /// Класс арматуры
        /// </summary>
        public string Class { get; set; } = DefaultClass;

        /// <summary>
        /// Дефолтный конструктор по диаметру, остальные дефолтные значения
        /// </summary>
        /// <param name="diameter"></param>
        public Armature(int diameter)
        {
            Diameter = diameter;
            defineParams();            
        }

        /// <summary>
        /// Диаметры арматуры
        /// </summary>
        public static List<Armature> Diameters { get; } = new List<Armature>
        { new Armature(6), new Armature(8),new Armature(10),new Armature(12),new Armature(14),
            new Armature(16),new Armature(18),new Armature(20),new Armature(22),new Armature(25),
            new Armature(28),new Armature(32),new Armature(36),new Armature(40),new Armature(45),
            new Armature(50),new Armature(55),new Armature(60),new Armature(70),new Armature(80)
        };

        private void defineParams()
        {
            switch (Diameter)
            {
                case 6:
                    Weight = 0.222;
                    Area = 0.283;
                    break;
                case 8:
                    Weight = 0.395;
                    Area = 0.503;
                    break;
                case 10:
                    Weight = 0.617;
                    Area = 0.785;
                    break;
                case 12:
                    Weight = 0.888;
                    Area = 1.131;
                    break;
                case 14:
                    Weight = 1.210;
                    Area = 1.540;
                    break;
                case 16:
                    Weight = 1.580;
                    Area = 2.010;
                    break;
                case 18:
                    Weight = 2.000;
                    Area = 2.540;
                    break;
                case 20:
                    Weight = 2.470;
                    Area = 3.140;
                    break;
                case 22:
                    Weight = 2.980;
                    Area = 3.800;
                    break;
                case 25:
                    Weight = 3.850;
                    Area = 4.910;
                    break;
                case 28:
                    Weight = 4.830;
                    Area = 6.160;
                    break;
                case 32:
                    Weight = 6.310;
                    Area = 8.040;
                    break;
                case 36:
                    Weight = 7.990;
                    Area = 10.180;
                    break;
                case 40:
                    Weight = 9.870;
                    Area = 12.570;
                    break;
                case 45:
                    Weight = 12.480;
                    Area = 15.000;
                    break;
                case 50:
                    Weight = 15.410;
                    Area = 19.630;
                    break;
                case 55:
                    Weight = 18.650;
                    Area = 23.760;
                    break;
                case 60:
                    Weight = 22.190;
                    Area = 28.270;
                    break;
                case 70:
                    Weight = 30.210;
                    Area = 38.480;
                    break;
                case 80:
                    Weight = 39.460;
                    Area = 50.270;
                    break;
                default:
                    break;
            }
        }        
    }
}