using System.Linq;
using GameCore.Entities;
using UnityEngine;

public class Bomb : Bullet
{
    [Header("Bomb Settings")]
    public float fuseTime = 0f;

    // Перевизначаємо Initialize для роботи таймера
    public override void Initialize()
    {
        base.Initialize();
        if (fuseTime > 0)
        {
            Invoke(nameof(Explode), fuseTime);
        }
    }

    // Логіка зіткнень
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTarget(collision) || IsObstacle(collision))
        {
            Explode();
        }
    }

    // Допоміжні методи для безпеки
    private bool IsTarget(Collider2D collision)
    {
        return !string.IsNullOrEmpty(targetTag) && collision.CompareTag(targetTag);
    }

    private bool IsObstacle(Collider2D collision)
    {
        return collision.CompareTag("Wall") || (HasTag("Obstacle") && collision.CompareTag("Obstacle"));
    }

    private bool HasTag(string tag)
    {
        try { return UnityEditorInternal.InternalEditorUtility.tags.Contains(tag); }
        catch { return false; }
    }

    // Логіка завершення польоту
    protected override void OnReachMaxRange()
    {
        Explode();
    }

    // Логіка вибуху
    public virtual void Explode()
    {
        Debug.Log($"<color=orange>[Bomb]</color> ВИБУХ! Радіус: {radiusEffect}");

        // Шукаємо всі об'єкти в радіусі вибуху
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, radiusEffect);

        foreach (Collider2D obj in objectsInRange)
        {
            // 1. Перевіряємо ворога (Subject)
            var enemy = obj.GetComponent<Subject>();
            if (enemy != null && enemy.CompareTag(targetTag))
            {
                enemy.TakeDmg(damage);
                Debug.Log($"[Bomb] Ворог {obj.name} отримав {damage} шкоди від вибуху.");
            }

            // 2. Перевіряємо гравця
            if (obj.CompareTag("Player") && targetTag == "Player")
            {
                var player = GameManager.Instance.currentPlayer;
                if (player != null)
                {
                    player.TakeDmg(damage);
                    Debug.Log($"[Bomb] Гравець отримав {damage} шкоди від вибуху.");
                }
            }
        }

        Destroy(gameObject);
    }

    // ПОВНА ІНІЦІАЛІЗАЦІЯ (Текстура, Дальність, Розмір, Радіус)
    public override void InitMagic(Puppet caster, Spell spell, Vector2 dir)
    {
        // 1. Базова ініціалізація з Bullet.cs (damage, speed, vector)
        base.InitMagic(caster, spell, dir);

        // 2. Встановлюємо дальність польоту (maxRange)
        // Якщо в JSON є radius, беремо його, інакше дефолт 15
        this.maxRange = (spell.radius > 0) ? spell.radius : 15f;

        // 3. Встановлюємо розмір снаряда (scale)
        float size = (spell.spellSize > 0) ? spell.spellSize : 0.5f;
        this.transform.localScale = new Vector3(size, size, 1f);

        // 4. Встановлюємо радіус вибуху
        this.radiusEffect = (spell.effectRadius > 0) ? spell.effectRadius : 3f;

        // 5. Встановлюємо текстуру
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && !string.IsNullOrEmpty(spell.texture_path))
        {
            string path =spell.texture_path;
            Sprite loadedSprite = Resources.Load<Sprite>(path);
            if (loadedSprite != null)
            {
                sr.sprite = loadedSprite;
            }
            else
            {
                Debug.LogError($"[Bomb Init] Спрайт не знайдено за шляхом: Resources/{path}");
            }
        }

        Debug.Log($"[Bomb Init] Фаєрбол ініціалізовано: Дальність={maxRange}, Розмір={size}, Текстура={spell.texture_path}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusEffect);
    }
}