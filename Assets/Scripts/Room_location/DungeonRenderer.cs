using UnityEngine;
using System.Collections.Generic;

public class DungeonRenderer : MonoBehaviour
{
    [Header("Префаби")]
    public GameObject startRoomPrefab;
    public GameObject[] randomRoomPrefabs;
    public GameObject wallHorizontal;
    public GameObject wallVertical;

    [Header("Налаштування")]
    public float gridStep = 16f;

    [Header("Корекція дверних отворів (Offset)")]
    // Використовуй ці повзунки в інспекторі, щоб підігнати стіни ідеально
    public float wallOffsetX = 0f;
    public float wallOffsetY = 0f;

    public Room BuildLocation(Agressive_Location location)
    {
        Room startRoomScript = null;

        // Очищення
        foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (RoomData data in location.GeneratedRooms)
        {
            Vector3 worldPos = new Vector3(data.gridPos.x * gridStep, data.gridPos.y * gridStep, 0);

            // Вибір префабу
            GameObject prefabToSpawn = (data.gridPos == Vector2Int.zero) ? startRoomPrefab : randomRoomPrefabs[data.prefabIndex % randomRoomPrefabs.Length];

            GameObject roomObj = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
            Room roomScript = roomObj.GetComponent<Room>();
            PopulateRoomWithEnemies(roomScript, data);
            if (data.gridPos == Vector2Int.zero)
            {
                startRoomScript = roomScript;
                // ГАРАНТІЯ ДЛЯ СТАРТУ: якщо це (0,0), переконуємось, що стіни стоять правильно
                FixStartRoomWalls(data, location);
            }

            // Рендер з твоїм офсетом 0.5
            ApplyWallRendering(data, roomScript);
        }

        return startRoomScript;
    }

    private void FixStartRoomWalls(RoomData startData, Agressive_Location location)
    {
        // Перевіряємо сусідів стартової кімнати. 
        // Якщо сусіда немає — стіна ПОВИННА бути закрита (true).
        startData.wallTop = !HasNeighbor(Vector2Int.up, location);
        startData.wallDown = !HasNeighbor(Vector2Int.down, location);
        startData.wallLeft = !HasNeighbor(Vector2Int.left, location);
        startData.wallRight = !HasNeighbor(Vector2Int.right, location);
    }

    private bool HasNeighbor(Vector2Int direction, Agressive_Location location)
    {
        // Шукаємо, чи є кімната в заданому напрямку від (0,0)
        return location.GeneratedRooms.Exists(r => r.gridPos == direction);
    }

    private void PopulateRoomWithEnemies(Room roomScript, RoomData data)
    {
        // Не спавним ворогів у стартовій кімнаті (0,0)
        if (data.gridPos == Vector2Int.zero) return;

        // Проходимо по всіх точках спавну ворогів, які ви розставили в префабі кімнати
        foreach (Transform p in roomScript.enemyPoints)
        {
            // Шанс спавну (наприклад, 70%)
            if (Random.value < 0.7f)
            {
                // Викликаємо наш спавнер для створення орка
                EnemySpawner.Instance.SpawnEnemyAtPoint("orc", p);
            }
        }
    }
    private void ApplyWallRendering(RoomData data, Room roomScript)
    {
        // Рендеримо стіни з урахуванням твого зміщення 0.5
        // Якщо стіна "занадто низько" — міняй знак + на - у wallOffsetY

        if (data.wallTop && roomScript.exitPoints_top != null)
            Instantiate(wallHorizontal, roomScript.exitPoints_top.position + new Vector3(0, wallOffsetY, 0), Quaternion.identity, roomScript.exitPoints_top);

        if (data.wallDown && roomScript.exitPoints_down != null)
            Instantiate(wallHorizontal, roomScript.exitPoints_down.position + new Vector3(0, -wallOffsetY, 0), Quaternion.identity, roomScript.exitPoints_down);

        if (data.wallLeft && roomScript.exitPoints_left != null)
            Instantiate(wallVertical, roomScript.exitPoints_left.position + new Vector3(-wallOffsetX, 0, 0), Quaternion.identity, roomScript.exitPoints_left);

        if (data.wallRight && roomScript.exitPoints_right != null)
            Instantiate(wallVertical, roomScript.exitPoints_right.position + new Vector3(wallOffsetX, 0, 0), Quaternion.identity, roomScript.exitPoints_right);
    }
}