using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace KR_MN_Acad.Model.Pile
{ 
    public class Pile
    {
        public const string ParamDocLinkName = "ОБОЗНАЧЕНИЕ";
        public const string ParamName = "НАИМЕНОВАНИЕ";
        public const string ParamDescriptionName = "ПРИМЕЧАНИЕ";
        public const string ParamWeightName = "МАССА";
        public const string ParamLengthName = "Длина сваи";
        public const string ParamSideName = "Размер сваи";
        public const string ParamViewName = "Вид";
        public const string ParamBottomGrillageName = "Низ_ростверка";

        public static HashSet<string> _ignoreParams = new HashSet<string> { "origin" };

        public string BlName { get; set; }        
        public ObjectId IdBlRef { get; set; }
        public Point3d Pt { get; set; }                
        public AttributeInfo PosAttrRef { get; set; }
        public PileOptions Options { get; set; }
        /// <summary>
        /// Номер сваи
        /// </summary>
        public int Pos { get; set; }
        /// <summary>
        /// Обозначение - документ, альбом, или пусто
        /// </summary>
        public string DocLink { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public int Length { get; set; }
        public int Side { get; set; }
        /// <summary>
        /// Отметка низа ростверка,м
        /// </summary>
        public double BottomGrillage { get; set; }
        /// <summary>
        /// Вид сваи
        /// </summary>
        public string View { get; set; }

        public string Error { get; set; }

        /// <summary>
        /// Верх сваи после забивки
        /// </summary>
        public double TopPileAfterBeat { get; set; }
        /// <summary>
        /// Верх сваи после срубки
        /// </summary>
        public double TopPileAfterCut { get; set; }        
        /// <summary>
        /// Отметка острия сваи
        /// </summary>
        public double PilePike { get; set; }

        public Pile(BlockReference blRef, string blName, PileOptions pileOptions)
        {
            BlName = blName;
            IdBlRef = blRef.Id;
            Pt = blRef.Position;                        
            Options = pileOptions;
            // Определение параметров сваи
            defineParameters(blRef);           
        }       

        public AcadLib.Result Check()
        {            
            if (PosAttrRef == null)
            {
                return AcadLib.Result.Fail($"Не найден атрибут '{Options.PileAttrPos}'");
            }            
            return AcadLib.Result.Ok();
        }

        /// <summary>
        /// Расчет высотных отметок сваи
        /// </summary>
        /// <param name="pileOptions"></param>
        public void CalcHightMarks(PileOptions pileOptions)
        {
            // отметка верха сваи после забивки
            TopPileAfterBeat = BottomGrillage + (pileOptions.DimPileBeatToRostwerk * 0.001);
            // отметка верха сваи после срубки = 'низ ростверка' + 'расст от низа ростверка до верха сваи после срубки'(50). 
            TopPileAfterCut = BottomGrillage + (pileOptions.DimPileCutToRostwerk * 0.001);
            // отметка острия сваи
            PilePike = TopPileAfterBeat - (Length * 0.001);
        }

        private void defineParameters(BlockReference blRef)
        {
            Dictionary<string, object> dictParams = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            // список атрибутов
            List<AttributeInfo> attrRefs =AttributeInfo.GetAttrRefs(blRef);
            attrRefs.ForEach(a => dictParams.Add(a.Tag, a.Text));            
            // Добавление всех дин параметров в словарь параметров
            defineDynParams(blRef, ref dictParams);

            // ПОЗ
            PosAttrRef = attrRefs.FirstOrDefault(a => a.Tag.Equals(Options.PileAttrPos, StringComparison.OrdinalIgnoreCase));
            if (PosAttrRef != null)            
                Pos = int.Parse(PosAttrRef.Text);
            // Обозначение
            DocLink = GetParamValueString(ParamDocLinkName, dictParams);
            // Наименование
            Name = GetParamValueString(ParamName, dictParams);
            // Примечание
            Description = GetParamValueString(ParamDescriptionName, dictParams);
            // Примечание
            View = GetParamValueString(ParamViewName, dictParams);
            // длина сваи
            Length = GetParamValueInt(ParamLengthName, dictParams);
            // сторона сваи
            Side = GetParamValueInt(ParamSideName, dictParams);
            // Низ ростверка
            BottomGrillage = GetParamValueDouble(ParamBottomGrillageName, dictParams);
            // Масса
            Weight = GetParamValueDouble(ParamWeightName, dictParams);
        }

        private string GetParamValueString(string paramName, Dictionary<string, object> dictParams)
        {
            object value;
            if (dictParams.TryGetValue(paramName, out value))
            {
                return value.ToString();
            }
            else
            {
                addError($"Не определен параметр {paramName}");
                return string.Empty;                
            }
        }

        private int GetParamValueInt(string paramName, Dictionary<string, object> dictParams)
        {
            object value = null;
            if (dictParams.TryGetValue(paramName, out value))
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch { }
            }            
            addError($"Не определен параметр {paramName}");
            return 0;
        }

        private double GetParamValueDouble(string paramName, Dictionary<string, object> dictParams)
        {
            object value;
            if (dictParams.TryGetValue(paramName, out value))
            {
                try
                {
                    return Convert.ToDouble(value);
                }
                catch { }
            }
            addError($"Не определен параметр {paramName}");
            return 0;
        }

        private void defineDynParams (BlockReference blRef,ref Dictionary<string, object> dictParams)
        {
            if (blRef.IsDynamicBlock)
            {
                foreach (DynamicBlockReferenceProperty prop in blRef.DynamicBlockReferencePropertyCollection)
                {
                    if (!_ignoreParams.Contains(prop.PropertyName, StringComparer.OrdinalIgnoreCase))
                    {                        
                        addParam(dictParams, prop.PropertyName, prop.Value);
                    }
                }
            }
        }

        private void addParam(Dictionary<string, object> parameters, string name, object value)
        {
            if (parameters.ContainsKey(name))
            {
                addError($"Дублирование параметра {name}");                
            }
            else
            {                
                parameters.Add(name, value);
            }
        }

        private void addError (string err)
        {
            if (Error == null)
            {
                Error = $"Ошибки в блоке {BlName}: {err}";
            }
            else
            {
                Error += "; " + err;
            }
        }
    }
}