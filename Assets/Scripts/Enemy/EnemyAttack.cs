using GameCore.Entities;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Accuracy Settings")]
    public float accuracyVariation = 30f; // розкид +- 30 градусів
    public GameObject defaultProjectilePrefab; // запасний префаб

    private float _attackCooldown = 1.5f;
    private float _lastAttackTime;
    private Enemy _enemy;
    private Transform _player;

    void Start()
    {
        _enemy = GetComponent<Enemy>();
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) _player = pObj.transform;

        if (_enemy != null && _enemy.stats != null && _enemy.stats.Weapon_use != null)
        {
            _attackCooldown = _enemy.stats.Weapon_use.attack_speed;
            if (_attackCooldown <= 0) _attackCooldown = 1.5f;
        }
    }

    public void TryAttack()
    {
        if (Time.time - _lastAttackTime < _attackCooldown) return;

        Shoot();
        _lastAttackTime = Time.time;
    }

    private void Shoot()
    {
        if (_player == null || _enemy?.stats?.Weapon_use == null) return;

        Weapon weapon = _enemy.stats.Weapon_use;

        // Розраховуємо напрямок до гравця з урахуванням розкиду
        Vector3 targetDirection = (_player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float randomOffset = Random.Range(-accuracyVariation, accuracyVariation);
        float finalAngle = baseAngle + randomOffset;

        // Перетворюємо кут назад у Vector2 для поля element.vector
        float finalAngleRad = finalAngle * Mathf.Deg2Rad;
        Vector2 shootDirection = new Vector2(Mathf.Cos(finalAngleRad), Mathf.Sin(finalAngleRad));

        // Завантажуємо префаб снаряда
        string projectilePath = "Projectiles/" + weapon.projectile_type;
        GameObject prefab = Resources.Load<GameObject>(projectilePath) ?? defaultProjectilePrefab;

        if (prefab != null)
        {
            // Створюємо об'єкт у позиції ворога
            GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);

            // Перевіряємо компоненти та налаштовуємо базові параметри Element
            if (go.TryGetComponent(out Bomb bomb))
            {
                ApplyEnemyProjectileSettings(bomb, shootDirection, weapon);
                bomb.Initialize();
            }
            else if (go.TryGetComponent(out Bullet bullet))
            {
                ApplyEnemyProjectileSettings(bullet, shootDirection, weapon);
                bullet.Initialize();
            }

            // ТОЧНИЙ ШЛЯХ ДО СПРАЙТУ З ТВОГО СКРІНШОТУ: image/weapons/iconst/
            SpriteRenderer bulletSprite = go.GetComponent<SpriteRenderer>();
            if (bulletSprite != null)
            {
                string spritePath = "image/weapons/iconst/" + weapon.projectile_type;
                Sprite newSprite = Resources.Load<Sprite>(spritePath);

                if (newSprite != null)
                {
                    bulletSprite.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning($"[Enemy Attack] Спрайт не знайдено за шляхом: Resources/{spritePath}");
                }
            }

            // Повертаємо снаряд у бік польоту
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Debug.Log($"[Enemy Attack] {_enemy.stats.Pname} вистрілив снарядом {weapon.projectile_type}.");
        }
        else
        {
            Debug.LogError($"[Enemy Attack] Префаб '{weapon.projectile_type}' не знайдено!");
        }
    }

    private void ApplyEnemyProjectileSettings(Element element, Vector2 direction, Weapon weapon)
    {
        element.vector = direction;
        element.speed = weapon.projectile_speed;

        // Встановлюємо розмір 0.5f, як ти просив. 
        // Твій Element.Initialize() сам застосує це значення до localScale
        element.size = 0.5f;
        element.radiusEffect = 2f;

        if (element is Bullet bullet)
        {
            bullet.SetOwner(gameObject);
            bullet.damage = weapon.base_damage;
            bullet.maxRange = weapon.attack_range;
        }
    }
}