using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme
{
    public static class SchemeBlockFactory
    {
        static Dictionary<string, Type> typesDict = new Dictionary<string, Type>
        {
            { "КР_Арм_Схема_Стена" , typeof(Wall.WallBlock) }
        };

        public static SchemeBlock CreateBlock(BlockReference blRef, string blName)
        {
            Type typeBlock;
            if (typesDict.TryGetValue(blName, out typeBlock))
            {
                return (SchemeBlock)Activator.CreateInstance(typeBlock, blRef, blName);
            }
            else
            {
                return null;
            }
        }
    }
}
