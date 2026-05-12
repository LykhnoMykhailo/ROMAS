using UnityEngine;

public abstract class Element : Object
{
    [Header("Element Physics")]
    public Vector2 vector;      // Напрямок
    public float speed;         // Швидкість
    public float size;          // Масштаб
    public float radiusEffect;  // Радіус вибуху/аури
    public int count;           // Кількість (для мульти-снарядів)

    public override void Initialize()
    {
        base.Initialize();
        transform.localScale = Vector3.one * size;
    }

    protected virtual void Update()
    {
        // Логіка руху, спільна для Bullet, Bomb та Summon
        transform.position += (Vector3)vector * speed * Time.deltaTime;
    }
}