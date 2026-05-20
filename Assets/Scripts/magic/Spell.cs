using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GameCore.Entities // Використовую твій простір імен
{
    public enum SpellType
    {
        Target,
        Projectile,
        Bomb,
        Zone,
        Summon
    }
    [System.Serializable]
    public class Spell
    {
        [Header("Основні характеристики")]
        public string spellName;
        public SpellType type;
        public float baseDamage;
        public float manaCost;
        public float cooldown;
        public float spellSize;
        [Header("Скейли (Масштабування)")]
        public float damagePerInt;
        public float damagePerSchoolLvl;

        [Header("Ефекти та Зони")]
        public string effect;
        public float effectSpeed;
        public float duration;
        public float radius;
        public float effectRadius;
        public int projectileCount;

        [Header("Візуал та Швидкість снаряда")]
        public string texture_path;       // НОВЕ ПОЛЕ
        public float projectile_speed;    // НОВЕ ПОЛЕ

        [Header("Крафт")]
        public CraftRecipe craftData;

        public Spell(string name, SpellType spellType)
        {
            spellName = name;
            type = spellType;
            craftData = new CraftRecipe();
        }

        /// <summary>
        /// СТАТИЧНА ФУНКЦІЯ: Завантажує заклинання з StreamingAssets/Data/Spells/
        /// </summary>
        public static Spell LoadFromConfig(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Data/Spells", fileName + ".json");

            if (!File.Exists(path))
            {
                Debug.LogError($"[Spell System] Конфіг заклинання не знайдено: {path}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                Spell loadedSpell = JsonConvert.DeserializeObject<Spell>(json, settings);
                return loadedSpell;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Spell System] Помилка завантаження заклинання '{fileName}': {ex.Message}");
                return null;
            }
        }
    }
}
