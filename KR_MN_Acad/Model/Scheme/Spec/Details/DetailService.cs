using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using KR_MN_Acad.Scheme.Elements;
using AcadLib;
using AcadLib.Blocks;

namespace KR_MN_Acad.Scheme.Spec.Details
{
    /// <summary>
    /// Сервис построения ведомости деталей
    /// </summary>
    public class DetailService
    {
        const string blockTitleName = "КР_Т_ВД_шапка";        
        Database db;
        List<IDetail> details;        
        double scale;

        /// <summary>
        /// Список деталей должен быть подготовлен и отсортирован
        /// Должна быть запущена транзакция!
        /// </summary>        
        public DetailService(List<IDetail> details, Database db)
        {
            this.details = details;
            this.db = db;
            scale = AcadLib.Scale.ScaleHelper.GetCurrentAnnoScale(db);
        }

        /// <summary>
        /// Создание ведомости деталей в чертеже в указанной точке верхнего левого угла таблицы
        /// Возвращается список объектов таблицы - набор блоков
        /// </summary>
        public List<ObjectId> CreateTable(Point3d ptTopLeftTable, ObjectId layerId)
        {
            List<ObjectId> res = new List<ObjectId> ();
            // Копирование блоков деталей в чертеж    
            var blNames = details.GroupBy(g=>g.BlockNameDetail).Select(s=>s.First().BlockNameDetail).ToList();
            copyBlocks(blNames);

            var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            var cs = db.CurrentSpaceId.GetObject( OpenMode.ForWrite )as BlockTableRecord;
            var t = db.TransactionManager.TopTransaction;

            // Вставка блоков деталей и заполнение параметров            
            var ptInsert = new Point3d (ptTopLeftTable.X, ptTopLeftTable.Y-25*scale, 0);
            var blRefTitle = BlockInsert.InsertBlockRef(blockTitleName, ptInsert,cs, t, scale);
            blRefTitle.LayerId = layerId;
            res.Add(blRefTitle.Id);

            foreach (var detail in details)
            {
                var blRefDetail = BlockInsert.InsertBlockRef(detail.BlockNameDetail, ptInsert, cs,t, scale);
                blRefDetail.LayerId = layerId;
                var atrs = AttributeInfo.GetAttrRefs(blRefDetail);
                // Заполнение параметров                
                detail.SetDetailsParam(atrs);
                // определение точки вставки следующего блока
                ptInsert = defineNextPtInsert(blRefDetail);

                res.Add(blRefDetail.Id);
            }

            return res;
        }

        private Point3d defineNextPtInsert (BlockReference blRefDetail)
        {
            // Нижний левый угол блока
            return blRefDetail.GeometricExtents.MinPoint;
        }        

        /// <summary>
        /// Копирование блоков деталей в чертеж
        /// </summary>
        private void copyBlocks (List<string> blocks)
        {            
            // Блок шапки деталей
            var id = Block.CopyBlockFromExternalDrawing(blockTitleName, KRHelper.FileKRBlocks, db, DuplicateRecordCloning.Replace);            
            foreach (var item in blocks)
            {
                id = Block.CopyBlockFromExternalDrawing(item, KRHelper.FileKRBlocks, db, DuplicateRecordCloning.Replace);
            }
        }
    }
}
