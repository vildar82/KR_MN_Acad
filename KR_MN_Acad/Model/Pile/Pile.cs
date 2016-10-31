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
    /// <summary>
    /// Блок сваи
    /// Сравнение по группе параметров - без ПОЗ!!!
    /// </summary>
    public class Pile : BlockBase, IEquatable<Pile>
    {
        public const string ParamSideName = "Размер сваи";
        public const string ParamViewName = "Вид";
        public const string ParamViewNamePrefix = "Пр";
        public const string ParamTypeName = "Тип";        
        public const string ParamBottomRostverkName = "Низ_ростверка_м";
        public const string ParamLengthName = "Длина_сваи_мм";
        public const string ParamPitHeightName = "Глубина_приямка_мм";
        public const string ParamPosName = "ПОЗ";
        public const string ParamDocLinkName = "ОБОЗНАЧЕНИЕ";
        public const string ParamName = "НАИМЕНОВАНИЕ";
        public const string ParamDescriptionName = "ПРИМЕЧАНИЕ";
        public const string ParamWeightName = "МАССА";
        
        public ObjectId IdBtrAnonym { get; set; }        
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
        public PileTypeEnum PileType { get; set; }
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

        public Pile(BlockReference blRef, string blName) : base(blRef, blName)
        {
            IdBtrAnonym = blRef.BlockTableRecord;            
            // Определение параметров сваи
            defineParameters(blRef);           
        }

        private bool _extendsHasCalc;
        private Extents3d _extents;
        public void Show (Editor ed)
        {
            if (!_extendsHasCalc)
            {
                _extendsHasCalc = true;
                using (var blRef = IdBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
                {
                    try
                    {
                        _extents = blRef.GeometricExtents;
                    }
                    catch
                    {
                        var ptMin = new Point3d(Position.X - Side, Position.Y - Side, 0);
                        var ptMax = new Point3d(Position.X + Side, Position.Y + Side, 0);
                        _extents = new Extents3d(ptMin, ptMax);
                    }
                }
            }
            ed.Zoom(_extents);
            IdBlRef.FlickObjectHighlight(2, 100, 100);
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

        /// <summary>
        /// обновление параметра Поз
        /// </summary>
        public void UpdateDefinePropPos ()
        {
            var blRef = IdBlRef.GetObject(OpenMode.ForRead) as BlockReference;
            var attrs = AcadLib.Blocks.AttributeInfo.GetAttrRefs(blRef);
            var attrPos = attrs.FirstOrDefault(p => p.Tag.Equals(PileCalcService.PileOptions.PileAttrPos, StringComparison.OrdinalIgnoreCase));
            if (attrPos != null)
            {
                var paramPos = GetProperty(ParamPosName);
                if (paramPos != null)
                {
                    paramPos.IdAtrRef = attrPos.IdAtr;
                }
            }
        }

        /// <summary>
        /// Установка Вида свае
        /// </summary>
        /// <param name="index"></param>
        public void SetView (int index)
        {
            var view = ParamViewNamePrefix + index;
            View = view;
            FillPropValue(ParamViewName, view);
        }

        /// <summary>
        /// Расчет высотных отметок сваи
        /// </summary>
        public void CalcHightMarks()
        {
            // отметка верха сваи после забивки
            TopPileAfterBeat = Math.Round (BottomRostverk + (PileCalcService.PileOptions.DimPileBeatToCut+ PileCalcService.PileOptions.DimPileCutToRostwerk - PitHeight) * 0.001, 3);
            // отметка верха сваи после срубки = 'низ ростверка' + 'расст от низа ростверка до верха сваи после срубки'(50). 
            TopPileAfterCut = Math.Round (BottomRostverk + (PileCalcService.PileOptions.DimPileCutToRostwerk-PitHeight) * 0.001,3);
            // отметка острия сваи
            PilePike = Math.Round (TopPileAfterBeat - (Length * 0.001), 3);
            // Отметка низа ростверка ???
            //BottomRostverk = Math.Round( BottomRostverk - PitHeight*0.001,3);
        }

        public static ObjectId GetAttDefPos (ObjectId idBtr)
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

        private void defineParameters(BlockReference blRef)
        {
            // ПОЗ
            Pos = GetPropValue<int>(ParamPosName);
            // Обозначение
            DocLink = GetPropValue<string>(ParamDocLinkName);
            // Наименование
            Name = GetPropValue<string>(ParamName);
            // Примечание
            Description = GetPropValue<string>(ParamDescriptionName, false);
            // Вид
            View = GetPropValue<string>(ParamViewName);
            // Тип
            var pileTypeString = GetPropValue<string>(ParamTypeName);
            PileType = GetPileType(pileTypeString);
            // длина сваи
            Length = GetPropValue<int>(ParamLengthName);
            // высота приямка
            PitHeight = GetPropValue<int>(ParamPitHeightName);
            // сторона сваи
            Side = GetPropValue<int>(ParamSideName);
            // Низ ростверка
            BottomRostverk = Math.Round( GetPropValue<double>(ParamBottomRostverkName), 4);
            // Масса
            Weight = Math.Round ( GetPropValue<double>(ParamWeightName), 4);
        }        

        /// <summary>
        /// Заполнение атрибута номера Позиции сваи в блок
        /// </summary>
        public void FillPos()
        {
            FillPropValue(ParamPosName, Pos);
        }

        private PileTypeEnum GetPileType (string pileTypeString)
        {
            switch (pileTypeString)
            {
                case "Рядовая":
                    return PileTypeEnum.Ordinary;
                case "Статическая":
                    return PileTypeEnum.Static;
                case "Динамическая":
                    return PileTypeEnum.Dynamic;
                case "Анкерная":
                    return PileTypeEnum.Anchor;
                default:
                    break;                        
            }
            AddError("Недопустимый тип сваи - " + pileTypeString);
            return PileTypeEnum.Ordinary;
        }

        public string GetPileType ()
        {
            switch (PileType)
            {
                case PileTypeEnum.Ordinary:
                    return "Рядовая";
                case PileTypeEnum.Static:
                    return "Статическая";
                case PileTypeEnum.Dynamic:
                    return "Динамическая";
                case PileTypeEnum.Anchor:
                    return "Анкерная";                
            }
            return "Не определено";
        }

        public bool Equals (Pile other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            var res = Length == other.Length &&
                Side == other.Side &&
                BottomRostverk == other.BottomRostverk &&
                PitHeight == other.PitHeight &&
                Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) &&
                DocLink.Equals(other.DocLink, StringComparison.OrdinalIgnoreCase) &&
                PilePike == other.PilePike &&                
                TopPileAfterBeat == other.TopPileAfterBeat &&
                TopPileAfterCut == other.TopPileAfterCut;                
            return res;
        }

        public override int GetHashCode ()
        {
            return Length.GetHashCode();
        }
    }
}