using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace GameCore.Entities
{
    /// <summary>
    /// Базовий клас Puppet, реалізований суворо за діаграмою image_7665d5.png.
    /// Містить усі характеристики, системи прогресії та методи завантаження даних.
    /// </summary>
    [Serializable]
    public class Puppet
    {
        [Header("Характеристики (Stats)")]
        public float st; // Strength
        public float ag; // Agility
        public float kn; // Knowledge
        public float mp; // Magical Power
        public string equippedWeaponName;
        [Header("Системи")]
        public Inventory inventory;
        public int lvl;

        [Header("Приріст характеристик за рівень")]
        public float st_lvl;
        public float ag_lvl;
        public float kn_lvl;
        public float mp_lvl;

        [Header("Здоров'я")]
        public float hp; // Базове/Максимальне здоров'я
        public float hp_battle;

        [Header("Мана")]
        public float mana; // Базова/Максимальна мана
        public float mana_battle;

        [Header("Прогрес")]
        public float exp;
        public List<string> skills; // Список ID навичок
        public List<string> book;   // Магічна книга / Рецепти

        [Header("Стан та Ефекти")]
        [JsonProperty("effects")]
        public List<Effect> effects; // ЗАМІНЕНО: Використовуємо клас Effect замість BaseEffect
        public Weapon Weapon_use; // Поточна зброя

        [Header("Бойові параметри")]
        [JsonIgnore]
        public Vector2 position_battle;

        [JsonProperty("posBattleX")]
        public float posBattleX { get => position_battle.x; set => position_battle = new Vector2(value, position_battle.y); }

        [JsonProperty("posBattleY")]
        public float posBattleY { get => position_battle.y; set => position_battle = new Vector2(position_battle.x, value); }

        public float speed;
        public float size;
        public string use_attack; // Поточний тип атаки

        [Header("Захист та Візуал")]
        public float armor;
        public List<string> textures; // Шляхи до спрайтів
        public string cords; // Поле з діаграми

        public string Pname;
        [Header("Налаштування прогресії")]
        public float exp_to_next_level = 100f; // Скільки треба для 2 рівня
        public float exp_multiplier = 1.2f;    // Коефіцієнт подорожчання рівня

        /// <summary>
        /// Підвищує рівень істоти, додає характеристики та знімає досвід.
        /// </summary>
        public void level_up()
        {
            if (this.exp >= this.exp_to_next_level)
            {
                // 1. Списуємо досвід
                this.exp -= this.exp_to_next_level;

                // 2. Збільшуємо рівень
                this.lvl++;

                // 3. Додаємо характеристики (приріст з діаграми)
                this.st += this.st_lvl;
                this.ag += this.ag_lvl;
                this.kn += this.kn_lvl;
                this.mp += this.mp_lvl;

                // 4. Перераховуємо базове HP/Mana (з урахуванням нових статів)
                // Наприклад, з коефіцієнтами 10 та 5
                calculate_base_stats(10f, 5f);

                // 5. Збільшуємо вартість наступного рівня
                this.exp_to_next_level = Mathf.Round(this.exp_to_next_level * this.exp_multiplier);

                Debug.Log($"[Puppet] {Pname} підняв рівень до {lvl}! Наступний рівень коштує {exp_to_next_level}");

                // Рекурсія: якщо досвіду настільки багато, що вистачає на ще один рівень
                if (this.exp >= this.exp_to_next_level)
                {
                    level_up();
                }
                calculate_base_stats(1, 1);
            }
            else
            {
                Debug.Log($"[Puppet] Недостатньо досвіду для рівня. Треба: {exp_to_next_level - exp}");
            }
        }

        /// <summary>
        /// Додає досвід і автоматично перевіряє можливість підвищення рівня.
        /// </summary>
        public void add_exp(float amount)
        {
            this.exp += amount;
            if (this.exp >= this.exp_to_next_level)
            {
                level_up();
            }
        }

        public Puppet()
        {
            this.inventory = new Inventory();
            this.skills = new List<string>();
            this.book = new List<string>();
            this.effects = new List<Effect>();
            this.textures = new List<string>();

            this.lvl = 1;
            this.position_battle = Vector2.zero;
        }

        /// <summary>
        /// СТАТИЧНА ФУНКЦІЯ: Завантажує дані Puppet з JSON-файлу.
        /// </summary>
        public static Puppet LoadFromTemplate(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data/Puppets", fileName + ".json");

            if (!File.Exists(path))
            {
                Debug.LogError($"[Puppet System] Файл шаблону не знайдено: {path}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                Puppet template = JsonConvert.DeserializeObject<Puppet>(json);
                Debug.Log($"[Puppet System] Дані істоти '{fileName}' успішно завантажені.");
                return template;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Puppet System] Помилка завантаження даних: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Застосовує завантажені дані до поточного об'єкта.
        /// </summary>
        public void ApplyTemplate(Puppet template)
        {
            if (template == null) return;

            this.st = template.st;
            this.ag = template.ag;
            this.kn = template.kn;
            this.mp = template.mp;
            this.st_lvl = template.st_lvl;
            this.ag_lvl = template.ag_lvl;
            this.kn_lvl = template.kn_lvl;
            this.mp_lvl = template.mp_lvl;
            this.hp = template.hp;
            this.mana = template.mana;
            this.armor = template.armor;
            this.speed = template.speed;
            this.size = template.size;
            this.textures = new List<string>(template.textures);
            this.skills = new List<string>(template.skills);
            this.Pname = template.Pname;

            // Ініціалізуємо бойові показники
            this.hp_battle = this.hp;
            this.mana_battle = this.mana;
        }

        /// <summary>
        /// Розрахунок характеристик згідно з формулами на діаграмі.
        /// </summary>
        public virtual void calculate_base_stats(float hp_multy, float mana_multy)
        {
            this.hp = 10 * (this.st * hp_multy);
            this.mana = 10 * (this.kn * mana_multy);

            this.hp_battle = this.hp;
            this.mana_battle = this.mana;
        }

        public void take_dmg(float dmg)
        {
            float final_dmg = Mathf.Max(0, dmg - this.armor);
            this.hp_battle -= final_dmg;
            if (this.hp_battle < 0) this.hp_battle = 0;
        }

        public bool is_alive() => this.hp_battle > 0;
    }
}