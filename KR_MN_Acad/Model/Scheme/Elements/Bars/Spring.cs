﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;

namespace KR_MN_Acad.Scheme.Elements.Bars
{
    /// <summary>
    /// Шпилька
    /// </summary>
    public class Spring : BarDetail, IDetail
    {
        /// <summary>
        /// Хвостик
        /// </summary>
        private const int tail = 75;

        /// <summary>
        /// Шаг шпилек по гориз
        /// </summary>
        public int StepHor { get; set; }
        /// <summary>
        /// Шаг шпилек по вертик
        /// </summary>
        public int StepVertic { get; set; }
        /// <summary>
        /// Ширина распределения шпилек по горизонтали
        /// </summary>
        public int WidthHor { get; set; }
        /// <summary>
        /// Ширина распределения шпилек по вертикали
        /// </summary>
        public int WidthVertic { get; set; }
        /// <summary>
        /// Длина раб шпильки (без хвостов)
        /// </summary>
        public int LRab { get; set; }
        public string BlockNameDetail { get { return "КР_Деталь_Ш1"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diam">Диаметр шпильки</param>
        /// <param name="diamWork">Диам раб арм</param>
        /// <param name="lRab">Раст между раб стержнями (от внешних граней стержней)</param>        
        /// <param name="stepHor">Шаг шпилек по горизонтали</param>
        /// <param name="stepVert">Шаг шпилек по вертикали</param>
        /// <param name="widthHor">Ширина распределения по гориз</param>
        /// <param name="widthVertic">Ширина распр по вертик</param>
        /// <param name="pos">Позиция (из атр блока)</param>
        /// <param name="block">Блок</param>
        public Spring (int diam, int lRab, int stepHor, int stepVert, int widthHor,int widthVertic, string pos, ISchemeBlock block) 
            : base(diam, GetLength(lRab), 1, "Ш-", pos, block, "Шпилька")
        {
            LRab = lRab;
            Class = ClassA240C;
            Gost = GostOld;
            StepHor = stepHor;
            StepVertic = stepVert;
            WidthHor = widthHor;
            WidthVertic = widthVertic;
            Count = CalcCount();
        }

        /// <summary>
        /// Определение кол шпилек
        /// </summary>
        /// <returns></returns>
        private int CalcCount()
        {
            int countVert = (int)Math.Ceiling((double)WidthVertic / StepVertic);
            int countHor = (int)Math.Ceiling((double)WidthHor / StepHor);
            return countVert * countHor;
        }

        public override string GetDesc()
        {
            return base.GetDesc() + $", ш.{StepHor}х{StepVertic}";
        }

        /// <summary>
        /// Определение длины шпильки
        /// </summary>
        /// <param name="lRab">Раст между раб стержнями (от внешних граней стержней)</param>        
        /// <returns></returns>
        private static int GetLength (int lRab)
		{
			return lRab + 2 * tail;
		}

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>     
        public void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ДЛИНА", LRab.ToString(), atrs);            
            SetDetailParameter("ХВОСТ1", tail.ToString(), atrs);
            SetDetailParameter("ХВОСТ2", tail.ToString(), atrs);
        }
    }
}
