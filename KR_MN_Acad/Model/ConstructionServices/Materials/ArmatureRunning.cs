using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;
using static AcadLib.Units.UnitsConvertHelper;

namespace KR_MN_Acad.ConstructionServices.Materials
{
    /// <summary>
    /// Арматура измеряемая поганажом
    /// </summary>
    public class ArmatureRunning : Armature
    {        
        /// <summary>
        /// Количество погонных метров
        /// </summary>
        public double Meters { get; set; }
        /// <summary>
        /// Масса общая
        /// </summary>
        public double WeightRunning { get; set; }

        private ArmatureRunning()
        {
        }

        public ArmatureRunning(int diam, double meters) : base(diam)
        {
            Meters = meters;
            CalcWeight();            
        }        

        public override void CalcWeight()
        {
            // Масса ед
            Weight = RoundHelper.RoundSpec(WeightUnit);
            // Масса общая
            WeightRunning = RoundHelper.RoundSpec(Weight * Meters);
        }

        public override void Add(IMaterial elem)
        {
            var add = (ArmatureRunning)elem;
            Meters += add.Meters;
        }

        public override IMaterial Copy()
        {
            ArmatureRunning res = new ArmatureRunning();
            res.CopyFields(res);
            res.Meters = Meters;
            res.WeightRunning = WeightRunning;
            return res;
        }

        public override SchemeRow RowScheme 
        {
            get
            {
                row = base.RowScheme;
                row.CountColumn = Meters.ToString();
                row.NameColumn = $"∅{Diameter} {Class} L=п.м.";
                return row;
            }
        }
        public override string GetLeaderDesc()
        {
            return base.GetLeaderDesc() + Meters;
        }        
    }
}
