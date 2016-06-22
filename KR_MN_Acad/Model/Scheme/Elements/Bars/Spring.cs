using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using KR_MN_Acad.ConstructionServices;

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
        private int tail;                
        private string descEnd;      
        /// <summary>
        /// Длина раб шпильки (без хвостов)
        /// </summary>
        public int LRab { get; set; }
        public string BlockNameDetail { get { return "КР_Деталь_Ш1"; } }

        /// <summary>
        /// Шпилька распределенная по площади стены
        /// </summary>
        /// <param name="diam">Диаметр шпильки</param>
        /// <param name="diamWork">Диам раб арм</param>
        /// <param name="lRab">Раст между раб стержнями (от центров раб стержней)</param>        
        /// <param name="stepHor">Шаг шпилек по горизонтали</param>
        /// <param name="stepVert">Шаг шпилек по вертикали</param>
        /// <param name="widthHor">Ширина распределения по гориз</param>
        /// <param name="widthVertic">Ширина распр по вертик</param>
        /// <param name="pos">Позиция (из атр блока)</param>
        /// <param name="block">Блок</param>
        public Spring (int diam, int lRab, int stepHor, int stepVert, int widthHor,int widthVertic, string pos, ISchemeBlock block) 
            : base(diam, GetLength(lRab, diam), 1, "Ш-", pos, block, "Шпилька")
        {
            descEnd = $", ш.{stepHor}х{stepVert}";            
            tail = getTail(diam);
            LRab = RoundHelper.Round5(lRab);
            Class = ClassA240C;
            Gost = GostOld;            
            Count = CalcCountByArea(widthHor, widthVertic, stepHor, stepVert);
        }

        /// <summary>
        /// Шпилька - с шагом по ширине распределения и кол рядов
        /// </summary>
        /// <param name="diam">Диам</param>
        /// <param name="lRab">Раст между раб стержнями (от центров раб стержней)</param>
        /// <param name="step">Шаг</param>
        /// <param name="width">Ширина распределения</param>
        /// <param name="rows">Рядов шпилек</param>
        /// <param name="pos">значение атр позиции</param>
        /// <param name="block">Блок</param>
        public Spring (int diam, int lRab, int step, int width, int rows, string pos, ISchemeBlock block)
            : base(diam, GetLength(lRab, diam), 1, "Ш-", pos, block, "Шпилька")
        {
            descEnd = $", ш.{step}";
            tail = getTail(diam);
            LRab = RoundHelper.Round5(lRab);
            Class = ClassA240C;
            Gost = GostOld;
            Count = BarDivision.CalcCountByStep(width, step)*rows;
        }

        private static int getTail (int diam)
        {
            return diam >= 10 ? 100 : 75;
        }

        /// <summary>
        /// Определение кол шпилек
        /// </summary>
        /// <returns></returns>
        private int CalcCountByArea(int widthHor, int widthVertic,int stepHor, int stepVertic)
        {
            int countVert = (int)Math.Ceiling((double)widthVertic / stepVertic);
            int countHor = (int)Math.Ceiling((double)widthHor / stepHor);
            return countVert * countHor;
        }

        public override string GetDesc ()
        {
            return base.GetDesc() + descEnd;
        }

        /// <summary>
        /// Определение длины шпильки
        /// </summary>
        /// <param name="lRab">Раст между раб стержнями (от внешних граней стержней)</param>        
        /// <returns></returns>
        private static int GetLength (int lRab, int diam)
		{
			return RoundHelper.Round5(lRab) + 2 * getTail(diam);
		}

        /// <summary>
        /// Заполнение параметров деталей - в блоке детали
        /// </summary>     
        public void SetDetailsParam (List<AttributeInfo> atrs)
        {
            SetDetailParameter("ПОЗИЦИЯ", SpecRow.PositionColumn, atrs);
            SetDetailParameter("ДЛИНА", LRab.ToString(), atrs);            
            SetDetailParameter("ХВОСТ1", "50", atrs);
            SetDetailParameter("ХВОСТ2", "50", atrs);
        }
    }
}
