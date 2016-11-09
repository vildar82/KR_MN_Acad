using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Blocks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using KR_MN_Acad.Model.Pile.Calc;

namespace KR_MN_Acad.Model.Pile
{ 
    public class Pile
    {
        public const string ParamDocLinkName = "ОБОЗНАЧЕНИЕ";
        public const string ParamName = "НАИМЕНОВАНИЕ";
        public const string ParamDescriptionName = "ПРИМЕЧАНИЕ";
        public const string ParamWeightName = "МАССА";
        public const string ParamLengthName = "Длина_сваи_мм";
        public const string ParamSideName = "Размер сваи";        
        public const string ParamViewName = "Вид";
        public const string ParamBottomRostverkName = "Низ_ростверка_м";
        public const string ParamPitHeightName = "Глубина_приямка_мм";

        public static HashSet<string> _ignoreParams = new HashSet<string> { "origin" };

        public string BlName { get; set; }        
        public ObjectId IdBlRef { get; set; }
        public ObjectId IdBtrAnonym { get; set; }
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
        /// <summary>
        /// Высота приямка
        /// </summary>
        public int PitHeight { get; set; }
        public int Side { get; set; }
        /// <summary>
        /// Отметка низа ростверка,м
        /// </summary>
        public double BottomRostverk { get; set; }
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
            IdBtrAnonym = blRef.BlockTableRecord;
            Pt = blRef.Position;                        
            Options = pileOptions;
            // Определение параметров сваи
            defineParameters(blRef);           
        }       

        public Result Check(bool checkNum)
        {            
            if (PosAttrRef == null)
            {
                return Result.Fail($"Не найден атрибут '{Options.PileAttrPos}'");
            }            
            if (checkNum)
            {
                if (Pos == 0)
                {
                    return Result.Fail($"Недопустимый номер сваи '{Pos}'");
                }
            }
            return Result.Ok();
        }

        public static void Check(List<Pile> piles)
        {
            // Проверка последовательности номеров.
            //var sortNums = piles.OrderBy(p => p.Pos);

            // Проверка повторяющихся номеров
            var repeatNums = piles.GroupBy(g => g.Pos).Where(g => g.Skip(1).Any());
            foreach (var repaet in repeatNums)
            {
                foreach (var item in repaet)
                {
                    Inspector.AddError($"Повтор номера {item.Pos}", item.IdBlRef, System.Drawing.SystemIcons.Error);
                }
            }

            // Проверка пропущенных номеров
            var minNum = piles.First().Pos;
            var maxNum = piles.Last().Pos;
            var trueSeq = Enumerable.Range(minNum, maxNum - minNum);
            var missNums = trueSeq.Except(piles.Select(p => p.Pos)).Where(p=>p>0);
            foreach (var item in missNums)
            {
                Inspector.AddError($"Пропущен номер сваи {item}", System.Drawing.SystemIcons.Error);
            }

            // Недопустимые номера - меньше 1
            var negateNums = piles.Where(p => p.Pos < 1);
            foreach (var item in negateNums)
            {
                Inspector.AddError($"Недопустимый номер сваи {item.Pos}", item.IdBlRef, System.Drawing.SystemIcons.Error);
            }
        }

        public static ObjectId GetAttDefPos(ObjectId idBtr)
        {
            using (var btr = idBtr.Open(OpenMode.ForRead) as BlockTableRecord)
            {
                foreach (var idEnt in btr)
                {
                    using (var atrDef = idEnt.Open(OpenMode.ForRead, false, true) as AttributeDefinition)
                    {
                        if (atrDef != null && atrDef.Tag.Equals(PileCalcService.PileOptions.PileAttrPos))
                        {
                            return atrDef.Id;
                        }
                    }
                }
            }
            return ObjectId.Null;
        }

        private bool _extendsHasCalc;        
        private Extents3d _extents;
        public void Show(Editor ed)
        {
            if (!_extendsHasCalc)
            {
                _extendsHasCalc = true;
                using (var blRef = IdBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
                {
                    try
                    {
                        _extents= blRef.GeometricExtents;
                    }
                    catch
                    {
                        var ptMin = new Point3d(Pt.X - Side, Pt.Y - Side, 0);
                        var ptMax = new Point3d(Pt.X + Side, Pt.Y + Side, 0);
                        _extents = new Extents3d(ptMin, ptMax);
                    }
                }
            }
            ed.Zoom(_extents);
            IdBlRef.FlickObjectHighlight(2, 100, 100);
        }

        /// <summary>
        /// Расчет высотных отметок сваи
        /// </summary>
        /// <param name="pileOptions"></param>
        public void CalcHightMarks()
        {
            // отметка верха сваи после забивки
            TopPileAfterBeat = BottomRostverk + (PileCalcService.PileOptions.DimPileBeatToCut+ PileCalcService.PileOptions.DimPileCutToRostwerk - PitHeight) * 0.001;
            // отметка верха сваи после срубки = 'низ ростверка' + 'расст от низа ростверка до верха сваи после срубки'(50). 
            TopPileAfterCut = BottomRostverk + (PileCalcService.PileOptions.DimPileCutToRostwerk-PitHeight) * 0.001;
            // отметка острия сваи
            PilePike = TopPileAfterBeat - (Length * 0.001);
            // Отметка низа ростверка
            BottomRostverk -= PitHeight*0.001;
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
            {
                int num;
                int.TryParse(PosAttrRef.Text, out num);
                Pos = num;
            }
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
            // высота приямка
            PitHeight = GetParamValueInt(ParamPitHeightName, dictParams);
            // сторона сваи
            Side = GetParamValueInt(ParamSideName, dictParams);
            // Низ ростверка
            BottomRostverk = GetParamValueDouble(ParamBottomRostverkName, dictParams);
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
                addError($"Не определен параметр '{paramName}'");
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
            addError($"Не определен параметр '{paramName}'");
            return 0;
        }

        private double GetParamValueDouble(string paramName, Dictionary<string, object> dictParams)
        {
            object value;
            if (dictParams.TryGetValue(paramName, out value))
            {
                try
                {
                    if (value is string)
                    {
                        return ((string)value).ToDouble();
                    }
                    return Convert.ToDouble(value);
                }
                catch { }
            }
            addError($"Не определен параметр '{paramName}'");
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
                addError($"Дублирование параметра '{name}'");                
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
                Error = err;
            }
            else
            {
                Error += "; " + err;
            }
        }
    }
}