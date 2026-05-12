using UnityEngine;

public class Bullet : Element
{
    [Header("Bullet Settings")]
    public float damage;
    public float maxRange;
    protected Vector3 startPosition;

    public override void Initialize()
    {
        base.Initialize(); // ¬становлюЇ розм≥р (size)
        startPosition = transform.position;

        // ѕовертаЇмо кулю в б≥к польоту
        RotateTowardsMovement();
    }

    private void RotateTowardsMovement()
    {
        if (vector != Vector2.zero)
        {
            // –ахуЇмо кут в градусах. Mathf.Atan2 повертаЇ рад≥ани, тому переводимо в градуси.
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            // якщо тв≥й спрайт спочатку дивитьс€ вправо, то додавати н≥чого не треба.
            // якщо в≥н дивитьс€ вгору, додай -90f. якщо вл≥во Ч додай 180f.
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    protected override void Update()
    {
        base.Update(); // –ух по вектору (vector * speed) з Element
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
        // ѕерев≥рка, щоб не влучити в самого себе або ≥нш≥ снар€ди
        if (collision.CompareTag("Enemy"))
        {
            ApplyDirectDamage(collision.gameObject);
            Destroy(gameObject);
        }
    }

    protected void ApplyDirectDamage(GameObject target)
    {
        // “ут буде тв≥й скрипт здоров'€ ворога
        // target.GetComponent<Health>().TakeDamage(damage);
        Debug.Log($"[Bullet] ¬лучанн€ в {target.name}! Ўкода: {damage}");
    }
}