using GameCore.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Attack Settings")]
    public float attackSpeed;
    private float nextAttackTime = 0f;

    void Update()
    {
        // Перевірка стрільби
        if (Mouse.current.leftButton.isPressed && GameManager.Instance.currentState == GameState.Battle)
        {
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + 1f / attackSpeed;
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        var player = GameManager.Instance.currentPlayer;
        if (player == null || player.inventory.equippedWeapon == null) return;

        Weapon currentWeapon = player.inventory.equippedWeapon;
        attackSpeed = currentWeapon.attack_speed;
        // 1. Створюємо об'єкт кулі
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bulletScript = bulletObj.GetComponent<Bullet>();

        // 2. Налаштовуємо візуал (спрайт)
        string spritePath = "image/weapons/iconst/" + currentWeapon.projectile_type;
        Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

        // ДОДАЙ ЦЕЙ РЯДОК ДЛЯ ПЕРЕВІРКИ:
        if (loadedSprite == null)
        {
            Debug.LogError($"!!! КРИТИЧНО: Файл не знайдено за шляхом: Resources/{spritePath}");
        }
        var sr = bulletObj.GetComponent<SpriteRenderer>();
        if (sr != null && loadedSprite != null)
        {
            sr.sprite = loadedSprite;
        }

        if (bulletScript != null)
        {
            // 3. ПЕРЕДАЄМО ВЛАСНИКА (це дозволить кулі вибрати ціль через SetOwner)
            bulletScript.SetOwner(this.gameObject);

            // 4. РАХУЄМО НАПРЯМОК (до миші)
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0; // Для 2D зануляємо Z
            Vector2 direction = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;

            // 5. ЗАПОВНЮЄМО ДАНІ
            bulletScript.vector = direction;
            bulletScript.damage = currentWeapon.GetTotalDamage(player);
            bulletScript.speed = currentWeapon.projectile_speed;
            bulletScript.maxRange = currentWeapon.attack_range;
            bulletScript.size = 0.4f;

            // 6. ЗАПУСКАЄМО
            bulletScript.Initialize();
        }
    }
}