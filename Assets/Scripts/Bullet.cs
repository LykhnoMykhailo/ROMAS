using GameCore.Entities;
using UnityEngine;

public class Bullet : Element
{
    [Header("Bullet Settings")]
    public float damage;
    public float maxRange;
    protected Vector3 startPosition;

    // ЗМІНЕНО З private НА protected:
    protected float speed;
    protected Vector2 direction;
    protected string effectToApply;
    protected string targetTag; // Кого ми б'ємо

    public void SetOwner(GameObject sender)
    {
        Debug.Log($"SetOwner Log Sender = {sender.CompareTag("Player")}");
        if (sender.CompareTag("Player"))
        {
            targetTag = "Enemy";
        }
        else if (sender.CompareTag("Enemy"))
        {
            targetTag = "Player";
        }
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
        base.Update(); // Рух по вектору (vector * speed) з Element
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
        Debug.Log($"Trigger use on {targetTag}");
        if (collision.CompareTag(targetTag))
        {
            ApplyDirectDamage(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    protected void ApplyDirectDamage(GameObject target)
    {
        Debug.Log($"[Bullet] Влучання в {target.name}! Шкода: {damage}");
    }

    /// <summary>
    /// НОВЕ ПЕРЕВАНТАЖЕННЯ: Ініціалізація кулі як магічного снаряда
    /// Додано ключове слово virtual, щоб Bomb міг його перевизначити
    /// </summary>
    public virtual void InitMagic(Puppet caster, Spell spell, Vector2 dir)
    {
        this.damage = spell.baseDamage + (caster.kn * spell.damagePerInt);
        this.speed = spell.projectile_speed;

        // ВАЖЛИВО: Задаємо vector для Element, щоб воно рухалось
        this.direction = dir.normalized;
        this.vector = this.direction;

        this.maxRange = spell.radius; // Прив'язуємо дистанцію польоту до радіусу з JSON
        this.effectToApply = spell.effect;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && !string.IsNullOrEmpty(spell.texture_path))
        {
            Sprite spellSprite = Resources.Load<Sprite>(spell.texture_path);
            if (spellSprite != null) sr.sprite = spellSprite;
        }

        Destroy(gameObject, spell.radius / Mathf.Max(this.speed, 1f));
    }
}