using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine; // Необхідно для Application та Debug
using Newtonsoft.Json;
using map_test;

public class LocationGenerator
{
    private LocationDataRoot presets;
    private System.Random random = new System.Random();

    // Використовуємо Path.Combine та StreamingAssets для надійності в Unity
    private string GetPresetPath()
    {
        return Path.Combine(Application.streamingAssetsPath, "data_b/WorldMap/LocationPresets.json");
    }

    public void LoadPresets()
    {
        string path = GetPresetPath();
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                presets = JsonConvert.DeserializeObject<LocationDataRoot>(json);

                // Додаткова ініціалізація, якщо JSON завантажився некоректно або порожнім
                if (presets == null) presets = new LocationDataRoot();
                if (presets.AgressivePresets == null) presets.AgressivePresets = new List<AgressivePreset>();
                if (presets.NetralPresets == null) presets.NetralPresets = new List<NetralPreset>();

                Debug.Log("<color=green>Пресети локацій успішно завантажені.</color>");
            }
            else
            {
                Debug.LogError($"Помилка: Файл не знайдено за шляхом {path}. Перевір папку StreamingAssets!");
                InitializeEmptyPresets();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Критична помилка при читанні пресетів: {ex.Message}");
            InitializeEmptyPresets();
        }
    }

    private void InitializeEmptyPresets()
    {
        presets = new LocationDataRoot
        {
            AgressivePresets = new List<AgressivePreset>(),
            NetralPresets = new List<NetralPreset>()
        };
    }

    public void GenerateWorldLocations(WorldMap map, int count = 12000)
    {
        // Якщо presets ще не завантажені — завантажуємо
        if (presets == null) LoadPresets();

        // Перевірка на null самого об'єкта map або його словника
        if (map == null || map.Locations == null)
        {
            Debug.LogError("Генерація скасована: Об'єкт WorldMap або словник Locations не ініціалізовані!");
            return;
        }

        // Рядок 42: Безпечна перевірка списків
        if (presets.AgressivePresets.Count == 0 && presets.NetralPresets.Count == 0)
        {
            Debug.LogWarning("Генерація скасована: списки пресетів у JSON порожні.");
            return;
        }

        int locationsAdded = 0;
        for (int i = 0; i < count; i++)
        {
            bool spawn = true;
            int attempts = 0;

            while (spawn && attempts < 100)
            {
                attempts++;
                int x = random.Next(1, 1001);
                int y = random.Next(1, 1001);
                string key = $"{x},{y}";

                if (!map.Locations.ContainsKey(key))
                {
                    Location_tile newLoc = new Location_tile();

                    // Шанс 1 до 6 на нейтральну
                    if (random.Next(0, 6) > 0 && presets.AgressivePresets.Count > 0)
                    {
                        var preset = presets.AgressivePresets[random.Next(presets.AgressivePresets.Count)];
                        newLoc.Setup_Location(preset.Id, preset.Texture, x, y, CreateAgressiveTask(preset));
                    }
                    else if (presets.NetralPresets.Count > 0)
                    {
                        var preset = presets.NetralPresets[random.Next(presets.NetralPresets.Count)];
                        newLoc.Setup_Location(preset.Id, preset.Texture, x, y, CreateNetralTask(preset));
                    }
                    else continue;

                    map.Locations.Add(key, newLoc);
                    locationsAdded++;
                    spawn = false;
                }
            }
        }
        Debug.Log($"<color=cyan>[System]</color> Генерація завершена: додано {locationsAdded} локацій.");
    }

    private Task_Location CreateAgressiveTask(AgressivePreset preset)
    {
        return new Agressive_Location
        {
            Id = preset.Id,
            Clan = preset.Clan,
            Boss = preset.Boss,
            CountOfRoom = random.Next(3, 8),
            Type = "Agressive"
        };
    }

    private Task_Location CreateNetralTask(NetralPreset preset)
    {
        return new Netral_Location
        {
            Id = preset.Id,
            Type = "Netral"
        };
    }
}