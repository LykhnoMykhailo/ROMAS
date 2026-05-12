using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCore.Entities
{
    [Serializable]
    public class Player : Puppet
    {
        [Header("Дані Гравця")]
        public float money;

        public Vector2 position;

        [JsonIgnore]
        public int X => (int)position.x;
        [JsonIgnore]
        public int Y => (int)position.y;

        [Header("Система завдань та прогресу")]
        public List<string> quest;
        public string playerClass;

        [Header("Навігаційні дані")]
        public string location;
        public string map;
        public string map_loc;

        public Player() : base()
        {
            this.quest = new List<string>();
            this.money = 0;
            this.Pname = "New Hero";
            this.playerClass = "Adventurer";
            this.position = Vector2.zero;
        }

        public void InitializeNewPlayer(string playerName, string startMapName)
        {
            this.Pname = playerName;
            this.map = startMapName;
            this.money = 100;
            this.lvl = 1;
            this.exp = 0;
            this.position = new Vector2(1, 1);

            this.st = 10;
            this.ag = 10;
            this.kn = 10;
            this.mp = 10;

            Weapon starter = WeaponDatabase.GetWeaponByName("Метальний_кинджал");

            if (starter != null)
            {
                inventory.AddItem(starter);
                inventory.Use_weapon(starter);
                this.equippedWeaponName = starter.Pname;

            }

            calculate_base_stats(10f, 5f);
        }

        #region Система Збереження та Завантаження

        public void SavePlayerData(string folderPath)
        {
            // Перед збереженням фіксуємо назву зброї в руках
            this.equippedWeaponName = (Weapon_use != null) ? Weapon_use.Pname : "";

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                // КРИТИЧНО: зберігає інформацію про те, що об'єкт - це Weapon, а не Item
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = JsonConvert.SerializeObject(this, settings);
            string fullPath = Path.Combine(folderPath, "Player.json");

            File.WriteAllText(fullPath, json);
            Debug.Log($"[Save] Збережено. Зброя в руках: {equippedWeaponName}");
        }

        public static Player LoadPlayer(string worldName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "Saves", worldName, "Player.json");

            if (!File.Exists(filePath)) return null;

            try
            {
                string json = File.ReadAllText(filePath);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto // Дозволяє відрізнити Weapon від Item
                };

                Player loadedPlayer = JsonConvert.DeserializeObject<Player>(json, settings);

                // Відновлення посилання на Weapon_use
                if (loadedPlayer != null && !string.IsNullOrEmpty(loadedPlayer.equippedWeaponName))
                {
                    // Шукаємо в інвентарі об'єкт, який є зброєю і має таку саму назву
                    var foundSlot = loadedPlayer.inventory.slots.Find(s =>
                        s.item is Weapon && s.item.Pname == loadedPlayer.equippedWeaponName);

                    if (foundSlot != null)
                    {
                        loadedPlayer.Weapon_use = foundSlot.item as Weapon;
                    }
                }

                return loadedPlayer;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Помилка завантаження: {ex.Message}");
                return null;
            }
        }

        #endregion

        public void AddMoney(float amount) { this.money += amount; }
        public bool CanAfford(float cost) { return this.money >= cost; }
    }
}