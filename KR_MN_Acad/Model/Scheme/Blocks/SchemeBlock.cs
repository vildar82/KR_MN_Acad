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
using KR_MN_Acad.Scheme.Elements.Bars;
using KR_MN_Acad.Scheme.Materials;
using KR_MN_Acad.Scheme.Spec;

namespace KR_MN_Acad.Scheme
{
    /// <summary>
    /// Базовый интерфейс блока для схемы армирования
    /// </summary>
    public abstract class SchemeBlock : ISchemeBlock
    {
        private HashSet<IElement> elements = new HashSet<IElement>();

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
            return elements.ToList();
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

        protected void FillElemProp (IElement elem, string posPropertyName, string descPropertyName)
        {
            if (elem != null)
            {
                // Поз
                FillProp(GetProperty(posPropertyName), elem.SpecRow.PositionColumn);
                // Опис            
                FillProp(GetProperty(descPropertyName), elem.GetDesc());
            }
        }

        protected static Rectangle GetRtreeRectangle(Extents3d extents)
        {
            return new Rectangle(extents.MinPoint.X, extents.MinPoint.Y,
                extents.MaxPoint.X, extents.MaxPoint.Y, 0, 0);
        }

        /// <summary>
        /// Определение отдельных стержней
        /// </summary>
        /// <param name="countArm">Кол-во стержней</param>
        /// <param name="length">Длина стержня</param>
        /// <param name="propDiam">Параметр диаметра стержня</param>
        /// <param name="propPos">Параметр позиции стержня</param>
        /// <param name="friendName">Человеческое имя стержня для описания ошибок - Горизонтальные стержни, Хомут, и т.п.</param>
        /// <returns></returns>
        protected Bar defineBar (int countArm, int length, string propDiam, string propPos, string friendName)
        {
            if (countArm == 0)
                return null;
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos);            
            var arm = new Bar(diam, length, countArm, pos, this, friendName);            
            arm.Calc();
            return arm;
        }

        /// <summary>
        /// Определение распределенной арматуры
        /// </summary>
        /// <param name="length">Длина стержня</param>
        /// <param name="widthRun">Ширина распределения</param>
        /// <param name="step">Шаг</param>
        /// <param name="propDiam">Парам диаметра</param>        
        /// <param name="propPos">Параметр аозиции</param>
        /// <param name="rows">Рядов стержней</param>
        /// <param name="friendlyName">Пользовательское имя</param>        
        protected Bar defineBarDiv (int length, int widthRun, int step, string propDiam, string propPos,
            int rows, string friendlyName)
        {
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos);            
            var armDiv = new Bar(diam, length, widthRun, step, rows, pos, this, friendlyName);
            armDiv.Calc();
            return armDiv;
        }

        /// <summary>
        /// Определение погонной арм с шириной и шагом распределения
        /// </summary>
        /// <param name="length"></param>
        /// <param name="widthRun"></param>
        /// <param name="propDiam"></param>
        /// <param name="propPos"></param>
        /// <param name="propStep"></param>
        /// <param name="concrete"></param>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        protected BarRunning defineBarRunStep (int length, int widthRun, int rows, string propDiam, string propPos, string propStep,
            Concrete concrete, string friendlyName)
        {
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos);
            int step = GetPropValue<int>(propStep);            
            double len = getLengthRunArm(length, diam, concrete);
            var arm = new BarRunning (diam, len, widthRun, step, rows, pos, this, friendlyName);
            arm.Calc();
            return arm;
        }

        /// <summary>
        /// Определение длины горизонтальных стержней
        /// </summary>
        /// <returns></returns>
        protected double getLengthRunArm (int length, int diam, Concrete concrete)
        {
            double kLap = BarRunning.GetKLap(diam, concrete);
            return length * kLap;
        }

        /// <summary>
        /// Определение скобы
        /// </summary>
        /// <param name="propDiam">Парам диам скобы</param>
        /// <param name="propPos">Парам поз скобы</param>
        /// <param name="propStep">Парам шаг скобы</param>
        /// <param name="bracketLen">Длина вылета скобы</param>
        /// <param name="thicknessWall">Толщина стены</param>
        /// <param name="a">Защитный слой до центра раб арм</param>
        /// <param name="widthRun">Ширина распределения скобы (высота стены)</param>
        /// <param name="diamWorkArm">Диам раб арм</param>
        /// <returns></returns>
        protected Bracket defineBracket (string propDiam, string propPos,
            string propStep, int bracketLen, int thicknessWall, int a, int widthRun, int diamWorkArm, int rows =1)
        {
            int diam = GetPropValue<int>(propDiam, false);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos, false);
            int step = GetPropValue<int>(propStep);            
            // ширина скобы
            int tBracket = thicknessWall - 2 * a + diamWorkArm;
            Bracket b = new Bracket(diam, bracketLen, tBracket , step, widthRun,rows, pos, this);
            b.Calc();
            return b;
        }

        /// <summary>
        /// Определение хомута - по габаритам бетона
        /// </summary>
        /// <param name="width">Ширина бетона</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="range">Ширина распределения хомутов (по бетону, отступ 100 отнимается тут)</param>
        /// <param name="diamWorkArm">Раб диам</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам позиции</param>
        /// <param name="propStep">Парам шага</param>        
        protected Shackle defineShackleByGab (int width, int thickness, int range, int diamWorkArm, int a,
            string propDiam, string propPos, string propStep,int rows=1)
        {
            int diam = GetPropValue<int>(propDiam, false);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos, false);
            int step = GetPropValue<int>(propStep);            
            // длина хомута
            int lShackle =  width-(2*a)+diamWorkArm;
            int hShackle = thickness-(2*a)+diamWorkArm;
            Shackle s = new Shackle(diam, lShackle, hShackle, step, range-100, rows, pos, this);
            s.Calc();
            return s;
        }

        /// <summary>
        /// Хомут по длине хомута
        /// </summary>
        /// <param name="propShLen">Параметр Длины хомута по внутр граням стержней</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="range">Ширина распределения хомутов (по бетону, отступ 100 отнимается тут)</param>
        /// <param name="diamWorkArm">Диам раб арм</param>
        /// <param name="propDiam">Парам диаметра</param>
        /// <param name="propPos">Значение атр позиции</param>
        /// <param name="propStep">Параметр шага</param>                
        protected Shackle defineShackleByLen (string propShLen, int thickness, int a, int range, int diamWorkArm,
            string propDiam, string propPos, string propStep, int rows =1)
        {
            var shackleLen = GetPropValue<int>(propShLen);
            int diam = GetPropValue<int>(propDiam);
            string pos = GetPropValue<string>(propPos);
            int step = GetPropValue<int>(propStep);
            int shackleH = thickness-(2*a)+diamWorkArm;
            var s = new Shackle(diam, shackleLen, shackleH, step, range-100,rows,  pos, this);
            s.Calc();
            return s;
        }

        /// <summary>
        /// определение шпильки
        /// </summary>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="propPos">Парам поз</param>
        /// <param name="propStepHor">Параметр шага по гор</param>
        /// <param name="propStepVert">Параметр шага по вертикали</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="widthHor">Ширина распределения по гор</param>
        /// <param name="widthVert">Ширина распределения по верт</param>
        /// <returns></returns>
        protected Spring defineSpring (string propDiam, string propPos, string propStepHor, string propStepVert,
           int thickness, int a,  int widthHor, int widthVert)
        {
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos);
            int stepHor = GetPropValue<int>(propStepHor);
            int stepVert = GetPropValue<int>(propStepVert);
            // ширина распределения шпилек по горизонтале            
            var lRabSpring =  thickness - 2 * a;
            Spring sp = new Spring(diam, lRabSpring, stepHor, stepVert, widthHor, widthVert, pos, this);
            sp.Calc();
            return sp;
        }

        /// <summary>
        /// Шпилька с шагом по ширине распределения и кол рядов
        /// </summary>
        /// <param name="propDiam">Диаметр</param>
        /// <param name="propPos">Значение атр позиции</param>
        /// <param name="propStep">Параметр шага</param>
        /// <param name="thickness">Толщина бетона</param>
        /// <param name="a">Защ слой до центра раб арм</param>
        /// <param name="width">Ширина распределения (по бетону)</param>
        /// <param name="propCount">Параметр рядов шпилек</param>        
        protected Spring defineSpring (string propDiam, string propPos, string propStep,
           int thickness, int a, int width, string propCount)
        {
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            string pos = GetPropValue<string>(propPos);
            int step = GetPropValue<int>(propStep);            
            var rows = GetPropValue<int>(propCount);
            // ширина распределения шпилек по горизонтале            
            var lRabSpring =  thickness - 2 * a;
            Spring sp = new Spring(diam, lRabSpring, step, width-100, rows, pos, this);
            sp.Calc();
            return sp;
        }


        /// <summary>
        /// определение гнутого стерженя
        /// </summary>
        /// <param name="propDiam">Парам диам</param>
        /// <param name="bentL">Длина Гс</param>
        /// <param name="bentH">высота Гс</param>
        /// <param name="width">Ширина распределения</param>
        /// <param name="propStep">Парам шаг</param>
        /// <param name="propPos">Парам позиции (атр)</param>        
        protected BentBarLshaped defineBent (string propDiam, int bentL, int bentH, int width, string propStep, 
            string propPos, int rows = 1)
        {
            int diam = GetPropValue<int>(propDiam);
            if (diam == 0) return null;
            int step = GetPropValue<int>(propStep);
            string pos = GetPropValue<string>(propPos);            
            BentBarLshaped bent = new BentBarLshaped (diam, bentL, bentH, width, step, rows, pos, this);
            bent.Calc();
            return bent;
        }

        /// <summary>
        /// Вертикальные гнутые стержни на величину диаметра стержня
        /// </summary>
        /// <param name="diam">Диаметр</param>
        /// <param name="count">Кол шт.</param>
        /// <param name="height">Высота конструкции по бетоны</param>
        /// <param name="outline">Выпуск</param>
        /// <param name="propPos">Значение параметра позиции</param>        
        protected BentBarDirect defineBentDirect (int diam, int count, int height, int outline, string propPos)
        {            
            int lstart = height;
            int lDif = 150;
            int lEnd = outline-lDif;
            int hDif = diam;
            var bent = new BentBarDirect(diam, lstart, lEnd, lDif, hDif, count, propPos, this);
            bent.Calc();
            return bent;
        }
    }
}