using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class GameMap
    {
        public int Height;
        public int Width;
        public string Name;

        public string[,] Map;

        public GameMap(int height, int width)
        {
            Height = height;
            Width = width;
            Map = new string[height, width];
        }
    }

    public class MapLoader
    {
        public static List<string> LoadMapPathList(string path)
        {
            var data = CsvLoader.LoadCsv(path);
            return (from DataRow row in data.Rows select row["path"].ToString()).ToList();
        }


        public static GameMap LoadMap(string path)
        {
            var data = CsvLoader.LoadCsv(path);
            var map = new GameMap(data.Rows.Count, data.Columns.Count)
            {
                Name = Path.GetFileNameWithoutExtension(path)
            };
            var rowIndex = 0;
            foreach (DataRow row in data.Rows)
            {
                for (var colIndex = 0; colIndex < data.Columns.Count; ++colIndex)
                {
                    map.Map[rowIndex, colIndex] = row[colIndex].ToString();
                }

                rowIndex++;
            }
            return map;
        }
    }
}