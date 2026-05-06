using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace map_test
{
    public class MapManager
    {
        // Поточна активна мапа
        public WorldMap CurrentMap { get;  set; }

        private MapGeneratorClass _generator;
        private LocationGenerator _locGenerator = new LocationGenerator(); // Рядок 1: Ініціалізація

        public MapManager()
        {
            _generator = new MapGeneratorClass();
        }

        // Створюємо нову мапу з нуля
        public WorldMap Return_map()
        {
            return CurrentMap;
        }
        public void CreateNewWorld(int x, int y, string name, int seed)
        {
            Console.WriteLine($"Генерація нового світу: {name}...");

            // ГЕНЕРАТОР тепер робить всю брудну роботу: і тайли, і локації
            CurrentMap = _generator.Generate_new(x, y, name, seed);

            if (CurrentMap != null)
            {
                _locGenerator.GenerateWorldLocations(CurrentMap); 

                Console.WriteLine("Генерація завершена.");
                Console.WriteLine($"Кількість локацій: {CurrentMap.Locations.Count}");
            }
        }

        // Збереження поточної мапи
        public void SaveCurrentMap(string folderPath)
        {
            // Тепер ми використовуємо отриманий шлях для збереження
            // Якщо ваш SaveLoadHelper.Save підтримує шлях, передайте його туди
            // Наприклад:
            SaveLoadHelper.Save(folderPath, CurrentMap);
}

        // Завантаження мапи з файлу
        public void LoadWorld(string name)
        {
            CurrentMap = SaveLoadHelper.Load(name); 
    
    if (CurrentMap != null)
            {
                Debug.Log($"[MapManager] Карта {name} завантажена в систему.");
            }
        }
    }
}