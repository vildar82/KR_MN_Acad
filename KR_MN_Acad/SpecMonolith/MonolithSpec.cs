using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Jigs;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.SpecMonolith
{
   public class MonolithSpec
   {
      public SpecMonolithService Service { get; private set; }
      public List<MonolithGroup> Groups { get; private set; }


      public MonolithSpec(SpecMonolithService service)
      {
         Service = service;
         Groups = new List<MonolithGroup>();
      }

      public void Calc()
      {
         var groups = Service.MonolithItems.GroupBy(i => i.Group).OrderBy(g=>g.Key);
         foreach (var itemGroup in groups)
         {
            MonolithGroup group = new MonolithGroup(itemGroup.Key);
            group.Calc(itemGroup);
            // проверка уникальности марок
            group.Check();
            Groups.Add(group);
         }
      }

      public void CreateTable()
      {
         Table table = getTable();
         insertTable(table);
      }

      private void insertTable(Table table)
      {
         Database db = Service.Doc.Database;
         Editor ed = Service.Doc.Editor;

         TableJig jigTable = new TableJig(table, 100, "Вставка таблицы");
         if (ed.Drag(jigTable).Status == PromptStatus.OK)
         {
            //table.ScaleFactors = new Scale3d(100);
            var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
            cs.AppendEntity(table);
            db.TransactionManager.TopTransaction.AddNewlyCreatedDBObject(table, true);            
         }
      }

      private Table getTable()
      {
         Table table = new Table();
         table.SetDatabaseDefaults(Service.Doc.Database);
         table.TableStyle = Service.Doc.Database.GetTableStylePIK(); // если нет стиля ПИк в этом чертеже, то он скопируетс из шаблона, если он найдется         

         int rows = 2 + Groups.Count + Groups.Sum(g => g.Records.Count);
         table.SetSize(rows, 6);

         // Марка
         table.Columns[0].Alignment = CellAlignment.MiddleCenter;
         table.Columns[0].Width = 15;
         // Обозн
         table.Columns[1].Alignment = CellAlignment.MiddleLeft;
         table.Columns[1].Width = 60;
         // Наимен
         table.Columns[2].Alignment = CellAlignment.MiddleLeft;
         table.Columns[2].Width = 65;
         // Кол
         table.Columns[3].Alignment = CellAlignment.MiddleCenter;
         table.Columns[3].Width = 10;
         // Масаа
         table.Columns[4].Alignment = CellAlignment.MiddleCenter;
         table.Columns[4].Width = 15;
         // Прим
         table.Columns[5].Alignment = CellAlignment.MiddleLeft;
         table.Columns[5].Width = 20;

         table.Rows[1].Height = 15;

         table.Cells[0, 0].TextString = "Спецификация к схеме расположения элементов замаркированных на данном листе";
         table.Cells[1, 0].TextString = "Марка";
         table.Cells[1, 1].TextString = "Обозначение";
         table.Cells[1, 1].Alignment = CellAlignment.MiddleCenter;
         table.Cells[1, 2].TextString = "Наименование";
         table.Cells[1, 2].Alignment = CellAlignment.MiddleCenter;
         table.Cells[1, 3].TextString = "Кол.";
         table.Cells[1, 4].TextString = "Масса, ед. кг";
         table.Cells[1, 5].TextString = "Примечание";

         var rowHeaders = table.Rows[1];
         var lwBold = rowHeaders.Borders.Top.LineWeight;
         rowHeaders.Borders.Bottom.LineWeight = lwBold;

         int row = 2;
         foreach (var group in Groups)
         {
            table.Cells[row, 2].TextString = "{0}{1}{2}".f("{\\L",group.Name, "}");
            table.Cells[row, 2].Alignment = CellAlignment.MiddleCenter;            

            row++;
            foreach (var rec in group.Records)
            {
               table.Cells[row, 0].TextString = rec.Mark;
               table.Cells[row, 1].TextString = rec.Indication;
               table.Cells[row, 2].TextString = rec.Name;
               table.Cells[row, 3].TextString = rec.Count.ToString();
               if (rec.Weight != 0)
               {
                  table.Cells[row, 4].TextString = rec.Weight.ToString("0.0");
               }               
               table.Cells[row, 5].TextString = rec.Description;
               row++;
            }            
         }
         var lastRow = table.Rows.Last();
         lastRow.Borders.Bottom.LineWeight = lwBold;

         table.GenerateLayout();
         return table;
      }
   }
}
