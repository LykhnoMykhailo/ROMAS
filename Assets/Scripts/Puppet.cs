using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace GameCore.Entities
{
    /// <summary>
    /// Базовий клас Puppet, реалізований за діаграмою з урахуванням системи магії (Book).
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
        public Book book;   // Оновлено: Магічна книга (клас Book замість List<string>)

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

        [Header("Стан та Ефекти")]
        [JsonProperty("effects")]
        public List<Effect> effects;
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
        public string cords;

        public string Pname;
        [Header("Налаштування прогресії")]
        public float exp_to_next_level = 100f;
        public float exp_multiplier = 1.2f;

        /// <summary>
        /// Підвищує рівень істоти, додає характеристики та знімає досвід.
        /// </summary>
        public void level_up()
        {
            if (this.exp >= this.exp_to_next_level)
            {
                this.exp -= this.exp_to_next_level;
                this.lvl++;

                this.st += this.st_lvl;
                this.ag += this.ag_lvl;
                this.kn += this.kn_lvl;
                this.mp += this.mp_lvl;

                // Розрахунок з коефіцієнтами
                calculate_base_stats(10f, 5f);

                this.exp_to_next_level = Mathf.Round(this.exp_to_next_level * this.exp_multiplier);

                Debug.Log($"[Puppet] {Pname} підняв рівень до {lvl}! Наступний рівень коштує {exp_to_next_level}");

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
            this.book = new Book(); // ВИПРАВЛЕНО: Додано дужки () для коректного створення екземпляра
            this.effects = new List<Effect>();
            this.textures = new List<string>();

            this.lvl = 1;
            this.position_battle = Vector2.zero;
        }

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

                // КРИТИЧНО ДЛЯ КНИГИ: додаємо налаштування десеріалізації типів
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                Puppet template = JsonConvert.DeserializeObject<Puppet>(json, settings);
                Debug.Log($"[Puppet System] Дані істоти '{fileName}' разом із магічною книгою успішно завантажені.");
                return template;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Puppet System] Помилка завантаження даних Puppet: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Застосовує завантажені дані до поточного об'єкта.
        /// </summary>
        public void UpdateToLvl(int lvl_user)
        {
            for (int i = 0; i < lvl_user; i++)
            {
                this.st = this.st + this.st_lvl;
                this.ag=this.ag + this.ag_lvl;
                this.kn=this.kn + this.kn_lvl;
                this.mp=this.mp + this.mp_lvl;

            }
            this.lvl = lvl_user;
            calculate_base_stats(1, 1);
        }
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

            // ДОДАНО: Передаємо магічну книгу з шаблону JSON (якщо вона там прописана)
            if (template.book != null)
            {
                this.book = template.book;
            }

            this.hp_battle = this.hp;
            this.mana_battle = this.mana;
        }

        public virtual void calculate_base_stats(float hp_multy = 1, float mana_multy = 1)
        {
            // Формула розрахунку HP від Сили (st) та Мани від Знань (kn)
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