using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Базовый интерфейс блока для схемы армирования
    /// </summary>
    public abstract class SchemeBlock
    {        
        public ObjectId IdBlref { get; set; }
        public string BlName { get; set; }
        public Dictionary<string, Property> Properties { get; set; }
        public Error Error { get; set; }

        public SchemeBlock(BlockReference blRef, string blName)
        {
            IdBlref = blRef.Id;
            BlName = BlName;
            Properties = GetProperties(blRef);
        }

        /// <summary>
        /// Определение всех полей и расчет элементов.
        /// Все ошибки записать в Error        
        /// </summary>        
        public abstract void Calculate();
        /// <summary>
        /// Получение всех элементов спецификации в блоке
        /// </summary>
        /// <returns></returns>
        public abstract List<RowScheme> GetElements();

        private Dictionary<string, Property> GetProperties(BlockReference blRef)
        {
            Dictionary<string, Property> dictProps = new Dictionary<string, Property>();
            var props = Property.GetAllProperties(blRef);
            foreach (var item in props)
            {
                try
                {
                    dictProps.Add(item.Name, item);
                }
                catch
                {
                    AddError($"Дублирование параметра {item.Name}.");
                }
            }
            return dictProps;
        }

        internal void AddError(string msg)
        {
            if (Error == null)
            {
                Error = new Error($"Ошибка в блоке {BlName}: ", IdBlref, System.Drawing.SystemIcons.Error);
            }
            Error.AdditionToMessage(msg);
        }
    }
}