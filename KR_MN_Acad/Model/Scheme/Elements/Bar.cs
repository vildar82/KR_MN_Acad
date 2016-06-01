using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Elements
{
    /// <summary>
    /// Арматурный стержень - элемент спецификации материалов схемы армироваания
    /// </summary>
    public class Bar
    {
        public string CountColumn { get; set; }      
        public string DescriptionColumn { get; set; }
        public string DocumentColumn { get; set; }
        public string NameColumn { get; set; }
        public string PositionColumn { get; set; }
        public string WeightColumn { get; set; }
        public GroupType Type { get; set; }

        public void PrepareConstTable()
        {
            throw new NotImplementedException();
        }

        public void PrepareFinalTable()
        {
            throw new NotImplementedException();
        }
    }
}
