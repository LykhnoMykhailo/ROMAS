using System;
using System.Collections.Generic;
using System.Linq;

namespace GameCore.Entities
{
    [Serializable]
    public class Inventory
    {
        // Тепер ми використовуємо один список слотів для всього
        public List<InventorySlot> slots = new List<InventorySlot>();

        // Окреме посилання для швидкого доступу до екіпірованої зброї
        public Weapon equippedWeapon;

        public Inventory()
        {
            slots = new List<InventorySlot>();
        }

        // Логіка додавання предметів
        public void AddItem(Item newItem, int amount = 1)
        {
            // 1. Перевіряємо, чи це зброя. У Skyrim зброя зазвичай не стакується.
            if (newItem is Weapon)
            {
                slots.Add(new InventorySlot(newItem, 1));
                return;
            }

            // 2. Якщо це не зброя (наприклад, зілля чи руна), шукаємо існуючий стак
            var existingSlot = slots.Find(s => s.item.Pname == newItem.Pname);

            if (existingSlot != null)
            {
                existingSlot.count += amount;
            }
            else
            {
                // 3. Якщо такого предмета ще немає — створюємо новий слот
                slots.Add(new InventorySlot(newItem, amount));
            }
        }

        // Метод для продажу (зменшення кількості)
        public void RemoveItem(Item item, int amount = 1)
        {
            var slot = slots.Find(s => s.item == item);
            if (slot != null)
            {
                slot.count -= amount;
                if (slot.count <= 0) slots.Remove(slot);
            }
        }
    }

    [Serializable]
    public class InventorySlot
    {
        public Item item;
        public int count;

        public InventorySlot(Item newItem, int newCount)
        {
            item = newItem;
            count = newCount;
        }
    }
}