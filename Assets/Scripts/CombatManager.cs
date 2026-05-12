using UnityEngine;
using System.Collections.Generic;
using GameCore.Entities; // Твій простір імен для Weapon та Item

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Налаштування сцени")]
    [SerializeField] private Transform shootPoint;      // Точка вильоту на персонажі
    [SerializeField] private GameObject battleArenaUI;  // UI бойового режиму

    private float nextAttackTime = 0f;
    private bool isBattleActive = false;

    // Кеш для префабів, щоб не викликати Resources.Load занадто часто
    private Dictionary<string, GameObject> projectileCache = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Якщо менеджер має жити між сценами:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Стріляємо лише в режимі бою
        if (!isBattleActive) return;

        // Обробка стрільби (Bullet Hell механіка)
        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
        {
            HandleShooting();
        }
    }

    /// <summary>
    /// Активує бойовий режим (викликай при заході в агресивну локацію)
    /// </summary>
    public void StartBattle()
    {
        isBattleActive = true;
        if (battleArenaUI != null) battleArenaUI.SetActive(true);
        Debug.Log("<color=red>[Combat]</color> Режим Bullet Hell активовано!");
    }

    /// <summary>
    /// Вимикає бойовий режим
    /// </summary>
    public void StopBattle()
    {
        isBattleActive = false;
        if (battleArenaUI != null) battleArenaUI.SetActive(false);
    }

    private void HandleShooting()
    {
        // Перевірка наявності GameManager та гравця (згідно з твоєю структурою)
        if (GameManager.Instance == null || GameManager.Instance.currentPlayer == null) return;

        Weapon currentWeapon = GameManager.Instance.currentPlayer.Weapon_use;
        if (currentWeapon == null) return;

        // Встановлюємо затримку (attack_speed)
        nextAttackTime = Time.time + currentWeapon.attack_speed;

        // Отримуємо префаб (з кешу або завантажуємо)
        GameObject prefab = GetProjectilePrefab(currentWeapon.projectile_type);

        if (prefab != null)
        {
            SpawnProjectile(prefab, currentWeapon);
        }
    }

    private GameObject GetProjectilePrefab(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return null;

        if (!projectileCache.ContainsKey(typeName))
        {
            GameObject loaded = Resources.Load<GameObject>("Projectiles/" + typeName);
            if (loaded != null)
            {
                projectileCache.Add(typeName, loaded);
            }
            else
            {
                Debug.LogError($"[Combat] Снаряд '{typeName}' не знайдено у Resources/Projectiles/");
                return null;
            }
        }
        return projectileCache[typeName];
    }

    private void SpawnProjectile(GameObject prefab, Weapon weapon)
    {
        // Створюємо об'єкт
        GameObject go = Instantiate(prefab, shootPoint.position, Quaternion.identity);

        // Розраховуємо напрямок до миші
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 shootDirection = (mousePos - shootPoint.position).normalized;

        // Оскільки Bomb : Bullet : Element : Object, спочатку шукаємо найбільш специфічний клас

        // 1. Спроба ініціалізації як Bomb
        if (go.TryGetComponent(out Bomb bomb))
        {
            ApplyCommonSettings(bomb, shootDirection, weapon);
            bomb.Initialize(); // Метод з Object/Element
        }
        // 2. Спроба ініціалізації як Bullet (якщо не Bomb)
        else if (go.TryGetComponent(out Bullet bullet))
        {
            ApplyCommonSettings(bullet, shootDirection, weapon);
            bullet.Initialize(); // Метод з Object/Element
        }

        // Повертаємо спрайт снаряда в бік польоту
        RotateToDirection(go.transform, shootDirection);
    }

    private void ApplyCommonSettings(Element element, Vector2 direction, Weapon weapon)
    {
        element.vector = direction;
        element.speed = weapon.projectile_speed;
        element.size = 1f; // Можна додати в JSON параметр 'projectile_scale'
        element.radiusEffect = 2f; // Для бомб, теж можна винести в JSON

        // Якщо це Bullet (або Bomb, бо він теж Bullet), додаємо специфічні дані
        if (element is Bullet bullet)
        {
            bullet.damage = weapon.base_damage;
            bullet.maxRange = weapon.attack_range;
        }
    }

    private void RotateToDirection(Transform t, Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        t.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}