using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.ConstructionServices.Materials;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme.Elements
{
    /// <summary>
    /// Элемент спецификации материалов (Поз, Обозн, Наимен, Кол, Масса ед, Примечание)
    /// </summary>
    public interface IElement : IMaterial, IComparable<IElement>, IEquatable<IElement>
    {
        string Prefix { get; set; }
        ISpecRow SpecRow { get; set; }
        GroupType Type { get; set; }         

        /// <summary>
        /// Расчет матариала (массы)
        /// </summary>
        void Calc();
        string GetDesc();
        double GetCount();
        double GetWeight();
        double GetWeightTotal();
        string GetName();
    }        
}
