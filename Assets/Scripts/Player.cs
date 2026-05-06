using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCore.Entities
{
    /// <summary>
    /// Клас гравця, що успадковується від Puppet.
    /// Додано властивості X/Y для синхронізації з рендерером карти.
    /// </summary>
    [Serializable]
    public class Player : Puppet
    {
        [Header("Дані Гравця")]
        public float money;

        /// <summary>
        /// Позиція гравця на загальній (глобальній) мапі.
        /// </summary>
        public Vector2 position;

        // Властивості для швидкого доступу до цілочисельних координат (для індексів масиву)
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

        /// <summary>
        /// Ініціалізація нового гравця при створенні світу.
        /// Встановлює початкову позицію в центрі мапи 1000x1000.
        /// </summary>
        public void InitializeNewPlayer(string playerName, string startMapName)
        {
            this.Pname = playerName;
            this.map = startMapName;
            this.money = 100;
            this.lvl = 1;
            this.exp = 0;

            // Початкова позиція в центрі глобальної карти
            this.position = new Vector2(500, 500);

            // Базові характеристики
            this.st = 10;
            this.ag = 10;
            this.kn = 10;
            this.mp = 10;

            calculate_base_stats(10f, 5f);

            Debug.Log($"<color=cyan>[Player]</color> Персонаж <b>{this.Pname}</b> ініціалізований на позиції {this.position}.");
        }

        #region Система Збереження та Завантаження

        public void SavePlayerData(string folderPath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(this, settings);
            string fullPath = Path.Combine(folderPath, "Player.json");

            File.WriteAllText(fullPath, json);
            Debug.Log($"[Player] Дані збережено: {fullPath}");
        }

        public static Player LoadPlayer(string worldName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "Saves", worldName, "Player.json");

            if (!File.Exists(filePath))
            {
                Debug.LogError("Файл гравця не знайдено!");
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                Player loadedPlayer = JsonConvert.DeserializeObject<Player>(json, settings);
                Debug.Log($"[Player] Дані гравця завантажені для світу: {worldName}");
                return loadedPlayer;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Помилка завантаження гравця: {ex.Message}");
                return null;
            }
        }

        #endregion

        public void AddMoney(float amount)
        {
            this.money += amount;
            Debug.Log($"[Player] Отримано {amount} золота. Баланс: {this.money}");
        }

        public bool CanAfford(float cost)
        {
            return this.money >= cost;
        }
    }
}