using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace KR_MN_Acad.Spec.ArmTypes.UI
{
    public class ViewArmTypeWall : ObservableObject
    {
        private ArmTypeWall armTypeWall;
        public ViewArmTypeWall (ArmTypeWall armTypeWall)
        {
            this.armTypeWall = armTypeWall;
        }

        public int Type {
            get { return armTypeWall.Type; }
            set {
                armTypeWall.Type = value;
                RaisePropertyChanged();
            }
        }
        public int ArmVerticDiam {
            get { return armTypeWall.ArmVerticDiam; }
            set {
                armTypeWall.ArmVerticDiam = value;
                RaisePropertyChanged();
            }
        }
        public int ArmVerticStep {
            get { return armTypeWall.ArmVerticStep; }
            set {
                armTypeWall.ArmVerticStep = value;
                RaisePropertyChanged();
            }
        }
        public int ArmHorDiam {
            get { return armTypeWall.ArmHorDiam; }
            set {
                armTypeWall.ArmHorDiam = value;
                RaisePropertyChanged();
            }
        }
        public int ArmHorStep {
            get { return armTypeWall.ArmHorStep; }
            set {
                armTypeWall.ArmHorStep = value;
                RaisePropertyChanged();
            }
        }
        public int SpringDiam {
            get { return armTypeWall.SpringDiam; }
            set {
                armTypeWall.SpringDiam = value;
                RaisePropertyChanged();
            }
        }
        public int SpringStepHor {
            get { return armTypeWall.SpringStepHor; }
            set {
                armTypeWall.SpringStepHor = value;
                RaisePropertyChanged();
            }
        }
        public int SpringStepVertic {
            get { return armTypeWall.SpringStepVertic; }
            set {
                armTypeWall.SpringStepVertic = value;
                RaisePropertyChanged();
            }
        }       
    }
}
