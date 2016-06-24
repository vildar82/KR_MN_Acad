using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Spec
{
    public class SpecBlockFactory
    {
        public static ISpecBlock CreateBlock (BlockReference blRef, string blName, ISpecOptions options)
        {            
            Type typeBlock;
            if (options.TypesBlock.TryGetValue(blName, out typeBlock))
            {
                return (ISpecBlock)Activator.CreateInstance(typeBlock, blRef, blName);
            }
            else
            {
                return null;
            }
        }
    }
}