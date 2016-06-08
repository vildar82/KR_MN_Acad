using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using AcadLib.RTree.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;
using KR_MN_Acad.Scheme.Elements;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Базовый интерфейс блока для схемы армирования
    /// </summary>
    public abstract class SchemeBlock : ISchemeBlock
    {
        private List<IElement> elements = new List<IElement>();

        public SchemeService Service { get; set; }
        public ObjectId IdBlref { get; set; }
        public string BlName { get; set; }
        public Dictionary<string, Property> Properties { get; set; }
        public Error Error { get; set; }        
        public Extents3d Extents { get; set; }
        public Rectangle Rectangle { get; set; }

        public SchemeBlock(BlockReference blRef, string blName, SchemeService service)
        {
            Service = service;
            IdBlref = blRef.Id;
            BlName = blName;
            Properties = GetProperties(blRef);
            Extents = blRef.GeometricExtentsСlean();
            Rectangle = GetRtreeRectangle(Extents);
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
        public List<IElement> GetElements()
        {
            return elements;
        }
        /// <summary>
        /// Заполнение нумерации материалов блока
        /// </summary>
        public abstract void Numbering();

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

        protected void AddError(string msg)
        {
            if (Error == null)
            {
                Error = new Error($"Ошибка в блоке {BlName}: ", IdBlref, System.Drawing.SystemIcons.Error);
                Inspector.AddError(Error);
            }
            Error.AdditionToMessage(msg);
        }

        /// <summary>
        /// преобразование object value свойства Property в указанный тип
        /// Тип T должен точно соответствовать типу object value Property
        /// </summary>        
        protected T GetPropValue<T>(string propName, bool isRequired = true)
        {
            T resVal = default(T);
            Property prop = GetProperty(propName, isRequired);
            if (prop != null)
            {
                resVal = (T)Convert.ChangeType(prop.Value, typeof(T));
            }
            return resVal;
        }

        protected Property GetProperty(string propName, bool isRequired = true)
        {
            Property prop;
            if (!Properties.TryGetValue(propName, out prop))
            {
                if (isRequired)
                {
                    AddError($"Не определен параметр {propName}.");
                }
            }
            return prop;
        }

        protected void AddElement(IElement elem)
        {
            if (elem != null)
            {
                elements.Add(elem);
            }
        }        

        protected void FillProp(Property prop, object value)
        {
            if (prop == null) return;
            if (prop.Type == PropertyType.Attribute && !prop.IdAtrRef.IsNull)
            {
                using (var atr = prop.IdAtrRef.GetObject(OpenMode.ForWrite, false, true) as AttributeReference)
                {                                        
                    atr.TextString = value?.ToString() ?? "";
                }
            }
        }

        protected static Rectangle GetRtreeRectangle(Extents3d extents)
        {
            return new Rectangle(extents.MinPoint.X, extents.MinPoint.Y,
                extents.MaxPoint.X, extents.MaxPoint.Y, 0, 0);
        }
    }
}