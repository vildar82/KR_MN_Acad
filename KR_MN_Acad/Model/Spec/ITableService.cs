﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Spec.Elements;

namespace KR_MN_Acad.Spec
{
    /// <summary>
    /// Сервис формирования таблицы
    /// </summary>
    public interface ITableService
    {   
        /// <summary>
        /// Нумерация элементов.
        /// Группировка элементов, сортировка, простановка позиций.
        /// </summary>        
        void Numbering (List<ISpecElement> elements);
        /// <summary>
        /// Группировка элементов спецификации - формирование строк для таблицы.
        /// Нумерация берется из блоков.
        /// </summary>        
        void CalcRows (List<ISpecElement> elements);
        /// <summary>
        /// Создание спецификации
        /// </summary>        
        Table CreateTable ();
        List<ISpecElement> GetElementsForBill (List<ISpecBlock> blocks);
        List<IDetail> GetDetails ();
        List<ISpecElement> FilterElements (List<ISpecBlock> blocks, bool isNumbering);
    }
}
