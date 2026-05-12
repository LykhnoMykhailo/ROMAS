using GameCore.Entities;
using UnityEngine;
using UnityEngine.InputSystem; // Додаємо цей рядок

public class PlayerCombat : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    [Header("Attack Settings")]
    public float attackSpeed = 2f; // Куль на секунду
    private float nextAttackTime = 0f;
void Update()
{
    // Перевірка затискання лівої кнопки миші (або іншої клавіші)
    if (Mouse.current.leftButton.isPressed && GameManager.Instance.currentState == GameState.Battle)
    {
        if (Time.time >= nextAttackTime)
        {
            Shoot();
            // Розрахунок часу наступного пострілу
            nextAttackTime = Time.time + 1f / attackSpeed;
        }
    }
}

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 1. Отримуємо дані гравця та його екіпірованої зброї
        var player = GameManager.Instance.currentPlayer;
        if (player == null || player.inventory.equippedWeapon == null)
        {
            Debug.LogWarning("Стрільба неможлива: гравець або зброя відсутні!");
            return;
        }

        Weapon currentWeapon = player.inventory.equippedWeapon;

        // 2. Створюємо кулю
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bulletScript = bulletObj.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            // 3. Рахуємо напрямок до миші (твій оригінальний код)
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;

            // 4. ПЕРЕДАЄМО ДАНІ ЗІ ЗБРОЇ
            bulletScript.vector = direction;

            // Використовуємо твій метод GetTotalDamage для врахування статів (st, ag, kn)
            bulletScript.damage = currentWeapon.GetTotalDamage(player);

            bulletScript.speed = currentWeapon.projectile_speed;
            bulletScript.maxRange = currentWeapon.attack_range;

            // Назва спрайту береться з конфігу (має бути в Resources/Bullets/)
            //bulletScript.spriteName = currentWeapon.projectile_type;

            // Розмір залишаємо 0.5f, як ти й вказав
            bulletScript.size = 0.5f;

            // 5. ЗАПУСКАЄМО (Initialize тепер завантажить спрайт і розверне кулю)
            bulletScript.Initialize();
        }
    }
}