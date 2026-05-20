using UnityEngine;
using System.Collections.Generic;
using GameCore.Entities;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Налаштування сцени")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject battleArenaUI;

    private float nextAttackTime = 0f;
    private bool isBattleActive = false;
    private Dictionary<string, GameObject> projectileCache = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isBattleActive) return;
        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime) HandleShooting();
    }

    public void StartBattle() { isBattleActive = true; if (battleArenaUI) battleArenaUI.SetActive(true); }
    public void StopBattle() { isBattleActive = false; if (battleArenaUI) battleArenaUI.SetActive(false); }

    private void HandleShooting()
    {
        if (GameManager.Instance?.currentPlayer?.Weapon_use == null) return;
        Weapon weapon = GameManager.Instance.currentPlayer.Weapon_use;
        nextAttackTime = Time.time + weapon.attack_speed;
        GameObject prefab = GetProjectilePrefab(weapon.projectile_type);
        if (prefab) SpawnProjectile(prefab, weapon);
    }

    private GameObject GetProjectilePrefab(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return null;
        if (!projectileCache.ContainsKey(typeName))
        {
            GameObject loaded = Resources.Load<GameObject>("Projectiles/" + typeName);
            if (loaded) projectileCache.Add(typeName, loaded);
        }
        return projectileCache.ContainsKey(typeName) ? projectileCache[typeName] : null;
    }

    private void SpawnProjectile(GameObject prefab, Weapon weapon)
    {
        GameObject go = Instantiate(prefab, shootPoint.position, Quaternion.identity);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = (mousePos - shootPoint.position).normalized;

        if (go.TryGetComponent(out Element el))
        {
            el.vector = dir;
            el.speed = weapon.projectile_speed;
            if (el is Bullet b) { b.damage = weapon.base_damage; b.maxRange = weapon.attack_range; }
            el.Initialize();
        }
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}