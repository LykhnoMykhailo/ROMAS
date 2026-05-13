using UnityEngine;
using GameCore.Entities;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public GameObject enemyPrefab; // Базовий префаб ворога з компонентом Enemy

    private void Awake() => Instance = this;

    public void SpawnEnemyAtPoint(string enemyType, Transform spawnPoint)
    {
        // 1. Завантажуємо стат-блок з JSON
        Puppet data = Puppet.LoadFromTemplate(enemyType);

        if (data != null)
        {
            // 2. Створюємо об'єкт
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // 3. Ініціалізуємо скрипт Enemy
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(data);
                enemyObj.name = data.Pname;
            }

            // 4. Додаткове налаштування візуалу
            if (data.textures.Count > 0)
            {
                var sr = enemyObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = Resources.Load<Sprite>(data.textures[0]);
            }
        }
    }
}