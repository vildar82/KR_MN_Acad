using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace KR_MN_Acad.Scheme.ArmTypes.UI
{
    public class ViewArmTypes : ObservableObject
    {
        private ArmTypesService armTypesService;

        public ViewArmTypes(ArmTypesService armTypesService)
        {
            this.armTypesService = armTypesService;
        }

        public ViewArmTypeWall ArmTypeWallView { get; set; }
    }
}
