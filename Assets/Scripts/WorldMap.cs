using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace map_test
{
    public class WorldMap
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Seed { get; set; }

        // Основна сітка тайлів (ландшафт)
        public WordMapTile[][] WorldMapMatrix { get; set; }

        // Словник локацій (міста, данжі). Ключ у форматі "x,y"
        public Dictionary<string, Location_tile> Locations { get; set; } = new Dictionary<string, Location_tile>();

        public WorldMap(int width, int height, string name, int seed)
        {
            Width = width;
            Height = height;
            Name = name;
            Seed = seed;

            // Ініціалізація матриці
            WorldMapMatrix = new WordMapTile[width][];
            for (int i = 0; i < width; i++)
                WorldMapMatrix[i] = new WordMapTile[height];
        }

        /// <summary>
        /// Зручний метод для додавання локації через координати
        /// </summary>
        public void AddLocation(int x, int y, Location_tile loc)
        {
            string key = $"{x},{y}";
            loc.cords_x = x; // Про всяк випадок синхронізуємо
            loc.cords_y = y;
            Locations[key] = loc;
        }

        /// <summary>
        /// Метод для отримання локації за координатами
        /// </summary>
        public Location_tile GetLocation(int x, int y)
        {
            string key = $"{x},{y}";
            if (Locations.TryGetValue(key, out Location_tile loc))
            {
                return loc;
            }
            return null;
        }
        public void SetTile(int x, int y, WordMapTile tile)
        {
            WorldMapMatrix[x][y] = tile;
        }

        public WordMapTile GetTile(int x, int y)
        {
            return WorldMapMatrix[x][y];
        }
        public void SetLocation(int x, int y, Location_tile location)
        {
            string key = $"{x},{y}";
            location.cords_x = x;
            location.cords_y = y;
            Locations[key] = location;
        }
        public Location_tile GetLocationTile(int x, int y)
        {

            return Locations[$"{x},{y}"];
        }
    }
}
