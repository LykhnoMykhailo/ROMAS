using UnityEngine;
using GameCore.Entities;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Налаштування Арени")]
    public GameObject arenaContainer;

    [Header("Модулі")]
    public DungeonRenderer dungeonRenderer;

    private void Awake() => Instance = this;

    public void StartBattle(Agressive_Location data)
    {
        // 1. Активуємо контейнер арени
        arenaContainer.SetActive(true);

        // 2. ГЕНЕРАЦІЯ: Якщо локація нова (немає кімнат), створюємо мапу в даних
        if (data.GeneratedRooms == null || data.GeneratedRooms.Count == 0)
        {
            Debug.Log("<color=yellow>[Battle]</color> Генерація нової мапи...");
            DungeonGenerator generator = new DungeonGenerator();
            generator.Generate(data);
        }

        // 3. РЕНДЕР: Будуємо локацію та отримуємо скрипт стартової кімнати
        Room startRoom = null;
        if (dungeonRenderer != null)
        {
            // Метод тепер повертає Room, де gridPos == (0,0)
            startRoom = dungeonRenderer.BuildLocation(data);
        }
        else
        {
            Debug.LogError("DungeonRenderer не призначений у BattleManager!");
            return;
        }

        // 4. ТЕЛЕПОРТАЦІЯ ТА КАМЕРА
        if (startRoom != null)
        {
            PositionPlayer(startRoom);
        }
        else
        {
            Debug.LogError("Стартову кімнату не знайдено! Перевір DungeonRenderer.");
        }

        Debug.Log($"<color=green>[Battle]</color> Бій розпочато. Клан: {data.Clan}");
    }

    private void PositionPlayer(Room startRoom)
    {
        GameObject playerObj = GameManager.Instance.visualPlayer;

        // Використовуємо точку спавну з префабу стартової кімнати
        if (startRoom.spawnPoint != null)
        {
            playerObj.transform.position = startRoom.spawnPoint.position;

            // Миттєво переміщуємо камеру до гравця, щоб уникнути помилок рендеру
            if (Camera.main != null)
            {
                Vector3 newCamPos = startRoom.spawnPoint.position;
                newCamPos.z = -10f; // Обов'язково для 2D, щоб камера не була в нулі
                Camera.main.transform.position = newCamPos;
            }

            Debug.Log("<color=cyan>[Battle]</color> Гравця та камеру перенесено до SpawnPoint.");
        }
        else
        {
            // Якщо забув поставити точку в префабі - ставимо в центр кімнати (0,0)
            playerObj.transform.position = startRoom.transform.position;
            Debug.LogWarning("SpawnPoint не знайдено у стартовій кімнаті! Ставимо в центр.");
        }
    }

    public void EndBattle()
    {
        // Ховаємо арену
        arenaContainer.SetActive(false);

        // Очищуємо відрендерену локацію (видаляємо всі кімнати)
        foreach (Transform child in arenaContainer.transform)
        {
            // Видаляємо тільки ті об'єкти, які згенеровані (наприклад, за тегом або просто всі дочірні)
            Destroy(child.gameObject);
        }

        // Повертаємо візуал мапи
        if (GameManager.Instance.mapRenderer != null)
            GameManager.Instance.mapRenderer.gameObject.SetActive(true);

        GameManager.Instance.ChangeState(GameState.WorldMap);
    }
}