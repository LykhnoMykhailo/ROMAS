using System;
using UnityEngine; // Потрібно для [Serializable] та інших атрибутів

namespace GameCore.Entities
{
    [Serializable]
    public class Weapon : Item
    {
        public float base_damage;
        public float damage_per_st;
        public float damage_per_int;
        public float attack_kd;
        public int range;
        // Переконайся, що ці класи (Enchant_Weapon, Project_tille) 
        // також доступні або знаходяться в цьому namespace
        public Enchant_Weapon upgrade;
        public string type;
        public bool Player_use;
        public bool Magazine;
        public Project_tille attack;
    }
}