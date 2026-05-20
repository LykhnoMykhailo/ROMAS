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
        public void TakeDmg(float dmg)
        {
            this.take_dmg(dmg);
            if (this.is_alive())
            {
                GameManager.Instance.HandlePlayerDeath();
            }
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

            // --- ПРАВИЛЬНИЙ БЛОК: ЗАВАНТАЖЕННЯ МАГІЇ З JSON ---

            // 1. Створюємо книгу магії, якщо вона null
            if (this.book == null)
            {
                this.book = new Book();
            }

            // 2. Завантажуємо заклинання "fireball" через вбудований метод конфігу
            // Функція автоматично шукає файл у StreamingAssets/Data/Spells/fireball.json
            Spell fireball = Spell.LoadFromConfig("fireball");

            if (fireball != null)
            {
                // Додаємо заклинання до списку вивчених (у загальний список книги)
                this.book.AddSpell(fireball);

                // Екіпіруємо у слот 1 (індекс 0) за допомогою твого методу з Book.cs
                this.book.EquipSpell(fireball, 0);
            }
            else
            {
                Debug.LogError("[Player Init] КРИТИЧНО: Не вдалося завантажити стартове заклинання! Перевірте файл StreamingAssets/Data/Spells/fireball.json");
            }

            // --------------------------------------------------

            calculate_base_stats(10f, 5f);

            // Даємо ману на старті
            this.mana_battle = this.mana;
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
        // Метод для повного відновлення стану гравця після смерті
        public void Respawn()
        {
            // Відновлюємо HP до максимального (припускаю, що у тебе є змінна max_hp)
            this.hp_battle = this.hp;
            this.mana_battle = this.mana;

            // Скидаємо позицію на стартову, якщо треба (або залишаємо поточну)
            // this.position = new Vector2(1, 1); 

            Debug.Log("[Player] Гравець відродився.");
        }
        public void AddMoney(float amount) { this.money += amount; }
        public bool CanAfford(float cost) { return this.money >= cost; }
    }
}