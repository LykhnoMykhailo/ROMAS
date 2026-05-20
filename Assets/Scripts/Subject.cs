using UnityEngine;
using GameCore.Entities;

public class Subject : MonoBehaviour
{
    [Header("Data")]
    public Puppet stats; // Наслідує характеристики Puppet

    [Header("Subject Fields (from Schema)")]
    public string clan;          // Фракція/клан
    public Vector2 location;    // Поточна локація
    public string weapon_name;   // Назва екіпірованої зброї

    public virtual void Initialize(Puppet data)
    {
        stats = data;
        stats.calculate_base_stats();
        clan = data.Pname; // Або інша логіка згідно з вашим задумом
        weapon_name = data.equippedWeaponName;
    }

    public virtual void TakeDamage(float amount)
    {
        if (stats == null) return;

        stats.take_dmg(amount);
        Debug.Log($"{gameObject.name} (Subject) отримав шкоду. HP: {stats.hp_battle}");

        if (!stats.is_alive()) Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} знищений.");
        Destroy(gameObject);
    }
}