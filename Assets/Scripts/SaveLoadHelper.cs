using System;
using System.Collections.Generic;
using System.IO;
using map_test;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class SaveLoadHelper
{
    private const string MetaFile = "meta.json";
    private const string GridFile = "grid.json";
    private const string LocFile = "locations.json";

    // Метод збереження
    public static void Save(string worldName, WorldMap map)
    {
        string savePath = Path.Combine(Application.persistentDataPath, "Saves", worldName);
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        // Метадані
        var metaData = new { map.Width, map.Height, map.Seed };
        File.WriteAllText(Path.Combine(savePath, MetaFile), JsonConvert.SerializeObject(metaData));

        // Матриця тайлів
        File.WriteAllText(Path.Combine(savePath, GridFile), JsonConvert.SerializeObject(map.WorldMapMatrix));

        // Словник локацій
        string locJson = JsonConvert.SerializeObject(map.Locations, settings);
        File.WriteAllText(Path.Combine(savePath, LocFile), locJson);

        Debug.Log($"[SaveLoadHelper] Світ '{worldName}' збережено в: {savePath}");
    }

    // Метод завантаження
    public static WorldMap Load(string worldName)
    {
        string savePath = Path.Combine(Application.persistentDataPath, "Saves", worldName);

        if (!Directory.Exists(savePath))
        {
            Debug.LogError($"[SaveLoadHelper] Папка не знайдена: {savePath}");
            return null;
        }

        try
        {
            // Створюємо карту (конструктор вимагає параметри, передаємо 0)
            WorldMap map = new WorldMap(0, 0, worldName, 0);

            // 1. Метадані через JObject (замість dynamic для уникнення помилок Unity)
            string metaJson = File.ReadAllText(Path.Combine(savePath, MetaFile));
            JObject metaData = JObject.Parse(metaJson);
            map.Width = (int)metaData["Width"];
            map.Height = (int)metaData["Height"];
            map.Seed = (int)metaData["Seed"];

            // 2. Матриця тайлів (WordMapTile[][])
            string gridJson = File.ReadAllText(Path.Combine(savePath, GridFile));
            map.WorldMapMatrix = JsonConvert.DeserializeObject<map_test.WordMapTile[][]>(gridJson);

            // 3. Локації (Словник з підтримкою абстрактних типів Task_Location)
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            string locJson = File.ReadAllText(Path.Combine(savePath, LocFile));
            map.Locations = JsonConvert.DeserializeObject<Dictionary<string, map_test.Location_tile>>(locJson, settings);

            return map;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveLoadHelper] Критична помилка: {ex.Message}");
            return null;
        }
    }
}