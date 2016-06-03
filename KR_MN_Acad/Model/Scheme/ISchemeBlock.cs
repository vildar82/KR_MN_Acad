using System.Collections.Generic;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.ConstructionServices.Materials;

namespace KR_MN_Acad.Scheme
{
    public interface ISchemeBlock
    {
        string BlName { get; set; }
        Error Error { get; set; }
        ObjectId IdBlref { get; set; }
        Dictionary<string, Property> Properties { get; set; }

        void Calculate();
        List<IMaterial> GetMaterials();
        void Numbering();
    }
}