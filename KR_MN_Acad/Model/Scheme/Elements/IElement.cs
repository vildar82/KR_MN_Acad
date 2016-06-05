﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR_MN_Acad.Scheme.Materials;
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
        /// <summary>
        /// Суммирование элементов и заполнение строк:
        /// Обозначения, Наименования, Кол, Массы ед, примечания
        /// </summary>        
        void Sum(List<IElement> elems);
        /// <summary>
        /// Строка элемента для заполнения в выноску в блоке
        /// </summary>
        /// <returns></returns>
        string GetDesc();        
    }        
}