using System;
using System.Collections.Generic;

namespace GameCore.Entities
{
    /// <summary>
    /// Клас інвентарю згідно з діаграмою image_76045b.png.
    /// Використовує динамічні списки для зберігання різних типів ігрових об'єктів.
    /// </summary>
    [Serializable]
    public class Inventory
    {
        // Використовуємо List<object> для максимальної гнучкості, 
        // щоб списки самі могли приймати будь-який тип даних.

        /// <summary>
        /// Список зброї (weapons)
        /// </summary>
        public List<object> weapons;

        /// <summary>
        /// Список предметів (iitems)
        /// </summary>
        public List<object> iitems;

        /// <summary>
        /// Список рун (runes)
        /// </summary>
        public List<object> runes;

        public Inventory()
        {
            weapons = new List<object>();
            iitems = new List<object>();
            runes = new List<object>();
        }

        /// <summary>
        /// Додати предмет до відповідного списку.
        /// </summary>
        public void AddItem(string category, object item)
        {
            switch (category.ToLower())
            {
                case "weapon":
                case "weapons":
                    weapons.Add(item);
                    break;
                case "item":
                case "iitems":
                case "items":
                    iitems.Add(item);
                    break;
                case "rune":
                case "runes":
                    runes.Add(item);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"[Inventory] Невідома категорія: {category}");
                    break;
            }
        }
    }
}