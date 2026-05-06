using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using map_test;
using Newtonsoft.Json;
using UnityEngine; // Обов'язково для Application та Debug

// Видаляємо namespace map_test, якщо він не використовується в проекті Unity
// namespace map_test

public class MapGeneratorClass
{
    // Посилання на конфігурацію
    private TilesetConfig _config;
    private const string ErrorPath = "image/map/none_texture.png";

    public MapGeneratorClass(string configPath = "image/map/tilesets.json")
    {
        // В Unity шлях до файлів конфігурації має бути всередині StreamingAssets
        string fullPath = Path.Combine(Application.streamingAssetsPath, configPath);

        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                // ВИПРАВЛЕНО: Використовуємо Newtonsoft.Json
                _config = JsonConvert.DeserializeObject<TilesetConfig>(json);
                Debug.Log("Конфігурація тайлсетів завантажена успішно.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Помилка десеріалізації конфігу: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Файл конфігурації не знайдено: {fullPath}");
        }
    }

    public WorldMap Generate_new(int size_x, int size_y, string name, int seed)
    {
        WorldMap generatedMap = new WorldMap(size_x, size_y, name, seed);

        // Вказуємо System.Random явно, щоб не плутати з UnityEngine.Random
        System.Random rand = new System.Random(seed);
        Queue<(int x, int y, int bType)> queue = new Queue<(int, int, int)>();

        if (_config == null || _config.Tilesets == null || _config.Tilesets.Count == 0)
        {
            Debug.LogError("Генерація неможлива: Конфігурація порожня або не завантажена.");
            return generatedMap;
        }

        int avgBiomeSize = 40;
        int numSeeds = (size_x * size_y) / avgBiomeSize;

        var biomeDataList = _config.Tilesets;

        // 1. Розстановка зерен
        for (int i = 0; i < numSeeds; i++)
        {
            int sx = rand.Next(0, size_x);
            int sy = rand.Next(0, size_y);

            if (generatedMap.GetTile(sx, sy) == null)
            {
                int biomeIndex = rand.Next(biomeDataList.Count);
                var selectedBiome = biomeDataList[biomeIndex];

                WordMapTile tile = new WordMapTile();
                string texturePath = selectedBiome.GetRandomPath(rand);

                // Безпечне парсинг ID
                if (int.TryParse(selectedBiome.Id, out int bId))
                {
                    tile.Load_map_tile(bId, texturePath);
                    generatedMap.SetTile(sx, sy, tile);
                    queue.Enqueue((sx, sy, bId));
                }
            }
        }

        // 2. Flood Fill (заповнення)
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (nx >= 0 && nx < size_x && ny >= 0 && ny < size_y)
                {
                    if (generatedMap.GetTile(nx, ny) == null)
                    {
                        var biomeData = _config.Tilesets.FirstOrDefault(t => t.Id == current.bType.ToString());

                        string texturePath = biomeData != null ? biomeData.GetRandomPath(rand) : ErrorPath;

                        WordMapTile tile = new WordMapTile();
                        tile.Load_map_tile(current.bType, texturePath);

                        generatedMap.SetTile(nx, ny, tile);
                        queue.Enqueue((nx, ny, current.bType));
                    }
                }
            }
        }

        Debug.Log($"Генерація мапи '{name}' завершена.");
        return generatedMap;
    }
}