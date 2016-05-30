using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace KR_MN_Acad.Scheme.Wall
{
    /// <summary>
    /// Описание блока стены
    /// </summary>
    public class WallBlock
    {
        public string BlName { get; set; }
        public ObjectId IdBlRef { get; set; }
        public bool IsOk { get; private set; }

        /// <summary>
        /// Длина стены
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// толщина стены
        /// </summary>
        public int Thickness { get; set; }

        public WallBlock(BlockReference blRef, string blMame)
        {
            BlName = BlName;
            IdBlRef = blRef.Id;
            defineParameters(blRef);
        }

        private void defineParameters(BlockReference blRef)
        {
            
        }

        public static List<WallBlock> GetWalls(List<ObjectId> ids)
        {
            List<WallBlock> walls = new List<WallBlock>();
            if (ids == null || ids.Count == 0) return walls;            
            var db = WallSchemeService.Db;
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var idEnt in ids)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                    if (blRef == null) continue;

                    string blName = blRef.GetEffectiveName();
                    if (IsBlockWall(blName))
                    {
                        WallBlock wall = new WallBlock(blRef, blName);
                        if (wall.IsOk)
                        {
                            walls.Add(wall);
                        }
                    }
                    else
                    {
                        WallSchemeService.Ed.WriteMessage($"\nПропущен блок '{blName}'");
                    }
                }
                t.Commit();
            }
            return walls; 
        }       

        public static bool IsBlockWall(string blName)
        {
            return Regex.IsMatch(blName, WallSchemeService.Options.WallBlockNameMatch, RegexOptions.IgnoreCase);
        }
    }
}
