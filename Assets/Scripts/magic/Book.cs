using System.Collections.Generic;
using GameCore.Entities;
using UnityEngine;
[System.Serializable]
public class Book
{
    // Список усіх вивчених заклинань
    public List<Spell> magic = new List<Spell>();

    // Заглушка для здібностей
    public List<Ability> abilities = new List<Ability>();

    // 9 слотів для екіпірованих заклинань (як на панелі в WoW)
    public Spell[] use_spells = new Spell[9];

    // ============================
    // РОБОТА ІЗ ЗАКЛИНАННЯМИ
    // ============================

    /// <summary>
    /// Додає нове заклинання в книгу
    /// </summary>
    public void AddSpell(Spell spell)
    {
        if (spell == null) return;

        // Перевіряємо, чи немає вже такого заклинання, щоб не було дублікатів
        if (!magic.Contains(spell))
        {
            magic.Add(spell);
            Debug.Log($"[Book] Заклинання '{spell.spellName}' додано до книги.");
        }
    }

    /// <summary>
    /// Видаляє заклинання з книги та автоматично знімає його з панелі швидкого доступу
    /// </summary>
    public void RemoveSpell(Spell spell)
    {
        if (spell == null) return;

        if (magic.Contains(spell))
        {
            magic.Remove(spell);
            Debug.Log($"[Book] Заклинання '{spell.spellName}' видалено з книги.");

            // Шукаємо, чи було воно екіпіроване, і видаляємо зі слота
            for (int i = 0; i < use_spells.Length; i++)
            {
                if (use_spells[i] == spell)
                {
                    use_spells[i] = null;
                    Debug.Log($"[Book] Заклинання '{spell.spellName}' автоматично прибрано зі слота {i + 1}.");
                }
            }
        }
    }

    /// <summary>
    /// Додає заклинання у швидкий слот (від 0 до 8)
    /// </summary>
    public void EquipSpell(Spell spell, int slotIndex)
    {
        // Перевіряємо, чи взагалі знаємо ми це заклинання
        if (spell != null && !magic.Contains(spell))
        {
            Debug.LogWarning($"[Book] Неможливо екіпірувати '{spell.spellName}'. Його немає у книзі!");
            return;
        }

        if (slotIndex >= 0 && slotIndex < 9)
        {
            use_spells[slotIndex] = spell;
            Debug.Log($"[Book] Заклинання '{spell?.spellName}' екіпіровано у слот {slotIndex + 1}");
        }
        else
        {
            Debug.LogWarning("[Book] Невірний індекс слота! Має бути від 0 до 8.");
        }
    }

    /// <summary>
    /// Очищає конкретний слот на панелі
    /// </summary>
    public void UnequipSpell(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 9)
        {
            use_spells[slotIndex] = null;
            Debug.Log($"[Book] Слот {slotIndex + 1} очищено.");
        }
    }

    // ============================
    // РОБОТА ЗІ ЗДІБНОСТЯМИ (Abilities)
    // ============================

    public void AddAbility(Ability ability)
    {
        if (ability != null && !abilities.Contains(ability))
        {
            abilities.Add(ability);
            Debug.Log($"[Book] Здібність '{ability.abilityName}' вивчено.");
        }
    }

    public void RemoveAbility(Ability ability)
    {
        if (ability != null && abilities.Contains(ability))
        {
            abilities.Remove(ability);
            Debug.Log($"[Book] Здібність '{ability.abilityName}' забуто.");
        }
    }
}