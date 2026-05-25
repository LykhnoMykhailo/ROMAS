using System.Linq;
using GameCore.Entities;
using UnityEngine;

public class Bullet : Element
{
    [Header("Bullet Settings")]
    public float damage;
    public float maxRange; // Дальність польоту
    protected Vector3 startPosition;
    protected string targetTag; // Ціль: "Enemy" або "Player"

    public void SetOwner(GameObject sender)
    {
        if (sender.CompareTag("Player")) targetTag = "Enemy";
        else if (sender.CompareTag("Enemy")) targetTag = "Player";
    }

    public override void Initialize()
    {
        base.Initialize();
        startPosition = transform.position;
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        if (vector != Vector2.zero)
        {
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void Update()
    {
        base.Update(); // Рух (vector * speed)
        CheckRange();
    }

    protected virtual void CheckRange()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            OnReachMaxRange();
        }
    }

    protected virtual void OnReachMaxRange()
    {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Перевірка влучання
        if (!string.IsNullOrEmpty(targetTag) && collision.CompareTag(targetTag))
        {
            ApplyDirectDamage(collision.gameObject);
            Destroy(gameObject);
        }
        // Перевірка стін (безпечна)
        else if (collision.CompareTag("Wall") || collision.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }
    }

    protected virtual void ApplyDirectDamage(GameObject target)
    {
        // 1. Перевіряємо чи це Ворог (Subject)
        // Ворог має компонент Enemy, який успадковує Subject
        var enemy = target.GetComponent<Subject>();
        if (enemy != null)
        {
            enemy.TakeDmg(damage);
            Debug.Log($"[Bullet] Нанесено {damage} шкоди ворогу.");
            return;
        }

        // 2. Перевіряємо чи це Гравець
        // Оскільки Player не MonoBehaviour, ми беремо його з GameManager, 
        // якщо об'єкт має тег "Player"
        if (target.CompareTag("Player"))
        {
            var player = GameManager.Instance.currentPlayer;
            if (player != null)
            {
                player.TakeDmg(damage);
                Debug.Log($"[Bullet] Нанесено {damage} шкоди гравцю.");
            }
            return;
        }
    }

    // ПОВНА ІНІЦІАЛІЗАЦІЯ КУЛІ
    public virtual void InitMagic(Puppet caster, Spell spell, Vector2 dir)
    {
        // 1. Параметри
        this.damage = spell.baseDamage + (caster.kn * spell.damagePerInt);
        this.speed = spell.projectile_speed;
        this.vector = dir.normalized;

        // 2. Дальність (Беремо з радіусу, якщо він є, інакше 15)
        this.maxRange = (spell.radius > 0) ? spell.radius : 15f;

        // 3. Розмір (Scale)
        float size = (spell.spellSize > 0) ? spell.spellSize : 0.5f;
        this.transform.localScale = new Vector3(size, size, 1f);

        // 4. Текстура
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && !string.IsNullOrEmpty(spell.texture_path))
        {
            string path = spell.texture_path;
            Sprite loadedSprite = Resources.Load<Sprite>(path);
            if (loadedSprite != null)
            {
                sr.sprite = loadedSprite;
            }
            else
            {
                Debug.LogError($"[Bullet Init] Спрайт не знайдено: Resources/{path}");
            }
        }

        Debug.Log($"[Bullet Init] Куля ініціалізована: Дальність={maxRange}, Розмір={size}");
    }
}