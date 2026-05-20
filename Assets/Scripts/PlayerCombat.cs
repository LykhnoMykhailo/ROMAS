using GameCore.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Attack Settings")]
    public float attackSpeed;
    private float nextAttackTime = 0f;

    [Header("Magic Settings")]
    public GameObject baseProjectilePrefab;
    public GameObject baseBombPrefab;
    private float[] lastCastTimes = new float[9];

    void Update()
    {
        // 1. —Ú≥Î¸·‡ Á≥ Á·Óø (“¬≤… Œ–»√≤Õ¿À‹Õ»…  Œƒ)
        if (Mouse.current.leftButton.isPressed && GameManager.Instance.currentState == GameState.Battle)
        {
            if (Time.time >= nextAttackTime)
            {
                Shoot();
                nextAttackTime = Time.time + 1f / attackSpeed;
            }
        }

        // 2. Ã‡„≥ˇ
        if (GameManager.Instance.currentState == GameState.Battle)
        {
            HandleMagicInput();
        }
    }

    // “¬≤… Œ–»√≤Õ¿À‹Õ»… Ã≈“Œƒ SHOOT (¡≈« «Ã≤Õ)
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        var player = GameManager.Instance.currentPlayer;
        if (player == null || player.inventory.equippedWeapon == null) return;

        Weapon currentWeapon = player.inventory.equippedWeapon;
        attackSpeed = currentWeapon.attack_speed;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        var bulletScript = bulletObj.GetComponent<Bullet>();

        string spritePath = "image/weapons/iconst/" + currentWeapon.projectile_type;
        Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

        if (loadedSprite == null)
            Debug.LogError($"!!!  –»“»◊ÕŒ: ‘‡ÈÎ ÌÂ ÁÌ‡È‰ÂÌÓ: Resources/{spritePath}");

        var sr = bulletObj.GetComponent<SpriteRenderer>();
        if (sr != null && loadedSprite != null) sr.sprite = loadedSprite;

        if (bulletScript != null)
        {
            bulletScript.SetOwner(this.gameObject);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            Vector2 direction = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;

            bulletScript.vector = direction;
            bulletScript.damage = currentWeapon.GetTotalDamage(player);
            bulletScript.speed = currentWeapon.projectile_speed;
            bulletScript.maxRange = currentWeapon.attack_range;
            bulletScript.size = 0.4f;

            bulletScript.Initialize();
        }
    }

    // ÀŒ√≤ ¿ Ã¿√≤Ø
    private void HandleMagicInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Key[] magicKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9 };

        for (int i = 0; i < magicKeys.Length; i++)
        {
            if (keyboard[magicKeys[i]].wasPressedThisFrame)
            {
                AttemptCastSpell(i);
                break;
            }
        }
    }

    private void AttemptCastSpell(int slotIndex)
    {
        var player = GameManager.Instance.currentPlayer;
        if (player == null || player.book == null || player.book.use_spells == null || slotIndex >= player.book.use_spells.Length) return;

        Spell spell = player.book.use_spells[slotIndex];
        if (spell == null || player.mana_battle < spell.manaCost) return;

        // œ≈–≈¬≤– ¿  ”Àƒ¿”Õ”
        if (Time.time < lastCastTimes[slotIndex] + spell.cooldown) return;

        lastCastTimes[slotIndex] = Time.time;
        player.mana_battle -= spell.manaCost;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        Vector2 direction = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;

        GameObject prefab = (spell.type == SpellType.Projectile) ? baseProjectilePrefab : baseBombPrefab;
        if (prefab == null) return;

        GameObject go = Instantiate(prefab, firePoint.position, Quaternion.identity);

        if (go.TryGetComponent(out Bomb bomb))
        {
            bomb.SetOwner(this.gameObject);
            bomb.InitMagic(player, spell, direction);
            bomb.Initialize();
        }
        else if (go.TryGetComponent(out Bullet bullet))
        {
            bullet.SetOwner(this.gameObject);
            bullet.InitMagic(player, spell, direction);
            bullet.Initialize();
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}