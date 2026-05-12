using System;
using UnityEngine; // Потрібно для [Serializable] та інших атрибутів

namespace GameCore.Entities
{
    [System.Serializable]
    public class Weapon : Item
    {
        public float base_damage;

        public float scale_agility;
        public float scale_strength;
        public float scale_knowledge;

        public string attack_type;
        public string projectile_type;

        public float attack_range;
        public float attack_speed;
        public float projectile_speed;

        // --- НОВІ ПОЛЯ ДЛЯ ЕФЕКТІВ ---
        public string status_effect;    // ID ефекту, наприклад "fire", "poison", "freeze"
        public float effect_chance;     // Шанс накладання (від 0.0 до 1.0)
        // -----------------------------

        public bool player_usable;
        public bool shop;

        /// <summary>
        /// Розраховує повну шкоду на основі характеристик того, хто тримає зброю.
        /// </summary>
        public float GetTotalDamage(Puppet owner)
        {
            float bonus = (owner.st * scale_strength) +
                          (owner.ag * scale_agility) +
                          (owner.kn * scale_knowledge);

            return base_damage + bonus;
        }
    }
}