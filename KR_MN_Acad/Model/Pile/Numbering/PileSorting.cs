using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    static class PileSorting
    {        
        public static List<Pile> Sort(List<KeyValuePair<Point3d, Pile>> piles, Options options)
        {            
            List<Pile> resVal;
            AcadLib.Comparers.DoubleEqualityComparer comparer = new AcadLib.Comparers.DoubleEqualityComparer(options.PileRowWidth);
            if (options.NumberingOrder == EnumNumberingOrder.RightToLeft)
            {                
                resVal = piles.OrderBy(p => p.Key.X).GroupBy(p => p.Key.Y, comparer)
                     .OrderByDescending(g => g.Key).SelectMany(g => g).Select(g => g.Value).ToList();
            }
            else
            {                
                // Сверху-вниз
                resVal = piles.OrderByDescending(p => p.Key.Y).GroupBy(p => p.Key.X, comparer)
                     .OrderBy(g => g.Key).SelectMany(g => g).Select(g => g.Value).ToList();                
            }
            return resVal; 
        }
    }
}
