using UnityEngine;

public class Bomb : Bullet
{
    [Header("Bomb Settings")]
    public float fuseTime = 0f; // Якщо 0 — вибухає при влучанні, якщо > 0 — через час

    public override void Initialize()
    {
        base.Initialize();
        if (fuseTime > 0)
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    // Перевизначаємо поведінку при влучанні
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            Explode();
        }
    }

    // Перевизначаємо поведінку при досягненні ліміту дистанції
    protected override void OnReachMaxRange()
    {
        Explode();
    }

    public virtual void Explode()
    {
        Debug.Log($"<color=orange>[Bomb]</color> ВИБУХ! Радіус: {radiusEffect}");

        // Пошук усіх цілей у радіусі вибуху (radiusEffect беремо з Element)
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radiusEffect);

        foreach (Collider2D obj in objectsInRange)
        {
            if (obj.CompareTag("Enemy"))
            {
                // Наносимо шкоду всім, хто потрапив у радіус
                // obj.GetComponent<Health>().TakeDamage(damage);
                Debug.Log($"[Bomb] Пошкоджено вибухом: {obj.name}");
            }
        }

        // Тут можна заспавнити префаб візуального ефекту вибуху
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // Для візуалізації радіуса вибуху в редакторі Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusEffect);
    }
}