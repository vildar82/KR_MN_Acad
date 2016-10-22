using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR_MN_Acad.Model.Pile
{
    public enum SchemaEnum
    {
        [Description("Обычный")]
        Normal,
        [Description("С промежуточной плитой")]
        Interval
    }
}
