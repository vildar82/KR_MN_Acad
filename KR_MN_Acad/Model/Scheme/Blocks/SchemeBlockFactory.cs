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
        public static SchemeBlock CreateBlock(BlockReference blRef, string blName, SchemeService service)
        {
            var typesDict = service.Options.TypesBlock;
            Type typeBlock;
            if (typesDict.TryGetValue(blName, out typeBlock))
            {
                return (SchemeBlock)Activator.CreateInstance(typeBlock, blRef, blName, service);
            }
            else
            {
                return null;
            }
        }
    }
}
