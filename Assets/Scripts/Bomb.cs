using GameCore.Entities;
using UnityEngine;

public class Bomb : Bullet
{
    [Header("Bomb Settings")]
    public float fuseTime = 0f;

    public override void Initialize()
    {
        base.Initialize();
        if (fuseTime > 0)
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            Explode();
        }
    }

    protected override void OnReachMaxRange()
    {
        Explode();
    }

    public virtual void Explode()
    {
        Debug.Log($"<color=orange>[Bomb]</color> ВИБУХ! Радіус: {radiusEffect}");

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radiusEffect);

        foreach (Collider2D obj in objectsInRange)
        {
            if (obj.CompareTag("Enemy"))
            {
                Debug.Log($"[Bomb] Пошкоджено вибухом: {obj.name}");
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusEffect);
    }

    /// <summary>
    /// НОВЕ ПЕРЕВАНТАЖЕННЯ: Ініціалізація бомби (магічної)
    /// </summary>
    public override void InitMagic(Puppet caster, Spell spell, Vector2 dir)
    {
        // 1. Викликаємо базовий метод з Bullet.cs (він задасть шкоду, напрямок, vector, спрайт)
        base.InitMagic(caster, spell, dir);

        // 2. Додаємо суто бомбівські властивості (записуємо у змінну з Element)
        this.radiusEffect = spell.effectRadius;
    }
}