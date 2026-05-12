using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

namespace GameCore.Entities
{
    public static class WeaponDatabase
    {
        // Головний список всієї зброї в грі
        public static List<Weapon> AllWeapons = new List<Weapon>();

        /// <summary>
        /// Завантажує зброю з JSON файлу
        /// </summary>
        public static void Initialize(string jsonContent)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            AllWeapons = JsonConvert.DeserializeObject<List<Weapon>>(jsonContent, settings);
            Debug.Log($"[Database] База зброї ініціалізована. Завантажено {AllWeapons.Count} одиниць.");
        }

        // 1. Вся зброя, яку можна купити в магазині
        public static List<Weapon> GetShopWeapons() =>
            AllWeapons.Where(w => w.shop && w.player_usable).ToList();

        // 2. Рідкісна зброя (може носити гравець, але не можна купити)
        public static List<Weapon> GetLootOnlyWeapons() =>
            AllWeapons.Where(w => !w.shop && w.player_usable).ToList();

        // 3. Зброя тільки для ворогів (суб'єктів)
        public static List<Weapon> GetEnemyOnlyWeapons() =>
            AllWeapons.Where(w => !w.player_usable).ToList();

        // 4. Пошук конкретної зброї за назвою
        public static Weapon GetWeaponByName(string name) =>
            AllWeapons.FirstOrDefault(w => w.Pname == name);
    }
}