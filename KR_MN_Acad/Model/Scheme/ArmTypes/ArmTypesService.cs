using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace KR_MN_Acad.Scheme.ArmTypes
{
    /// <summary>
    /// Сервис управлением типами армирования
    /// </summary>
    public class ArmTypesService
    {
        public Document Doc { get; private set; }
        public List<ArmTypeWall> ArmTypesWall { get; set; }

        public ArmTypesService(Document doc)
        {
            Doc = doc;
        }

        /// <summary>
        /// Загрузка типов армирования для этого чертежа
        /// </summary>
        public void Loadtypes()
        {

        }

        /// <summary>
        /// Сохранение типов армирования чертежа
        /// </summary>
        public void SaveTypes()
        {

        }

        /// <summary>
        /// Редактор типов армирования чертежа
        /// </summary>
        public void Editor()
        {

        }
    }
}
