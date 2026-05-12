using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; private set; }

    [Header("Префаби")]
    public Room startRoomPrefab;
    public Room tRoomPrefab;
    public Room bossRoomPrefab;
    public Room[] roomPrefabs;
    public Room[] deadEndPrefabs;

    [Header("Налаштування")]
    public float spacing = 1.0f; // Збільшимо для тесту, щоб побачити розрив

    private List<GameObject> activeRooms = new List<GameObject>();
    private bool bossSpawned = false;

    private void Awake() => Instance = this;

    public void StartDungeonGeneration()
    {
        ClearDungeon();
        bossSpawned = false;

        // 1. Старт
        Room spawn = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity);
        spawn.transform.SetParent(this.transform);
        activeRooms.Add(spawn.gameObject);

        // 2. Ланцюжок
        // ТУНЕЛЬ: вихід 0 спавну -> вхід 0 тунелю
        Room tunnel = SpawnRoomWithMath(spawn, 0, roomPrefabs[0], 0);

        if (tunnel != null)
        {
            // Т-КІМНАТА: вихід 1 тунелю -> вхід 2 (низ) Т-кімнати
            Room tRoom = SpawnRoomWithMath(tunnel, 1, tRoomPrefab, 2);

            if (tRoom != null)
            {
                // ЛІВО: вихід 0 Т-кімнати -> вхід 0 тупика
                SpawnRoomWithMath(tRoom, 0, deadEndPrefabs[0], 0);

                // ПРАВО: вихід 1 Т-кімнати -> вхід 0 БОСА
                if (!bossSpawned && bossRoomPrefab != null)
                {
                    SpawnRoomWithMath(tRoom, 1, bossRoomPrefab, 0);
                    bossSpawned = true;
                }
            }
        }
    }

    public Room SpawnRoomWithMath(Room prevRoom, int prevExitIdx, Room nextPrefab, int nextEntryIdx)
    {
        if (prevRoom == null || prevRoom.exitPoints[prevExitIdx] == null) return null;

        Transform targetExit = prevRoom.exitPoints[prevExitIdx];

        // 1. ПОВОРОТ: Вхід нової кімнати має дивитися на 180 градусів від виходу старої
        Quaternion nextRot = targetExit.rotation * Quaternion.Euler(0, 0, 180f);

        Room nextRoom = Instantiate(nextPrefab);
        nextRoom.transform.rotation = nextRot;

        // 2. МАТЕМАТИКА ЗМІЩЕННЯ (Offset)
        Vector3 entryLocalOffset = nextRoom.exitPoints[nextEntryIdx].localPosition;
        Vector3 rotatedOffset = nextRot * entryLocalOffset;

        // 3. ВІДСТУП (Spacing)
        // Ми використовуємо targetExit.right, бо в 2D 'forward' часто дивиться в бік X
        // Якщо кімнати лізуть одна на одну, заміни .right на .up
        Vector3 directionOut = targetExit.right;
        Vector3 spacingOffset = directionOut * spacing;

        // ФОРМУЛА: Центр = ТочкаВиходу - ВекторДоВходу + Відступ
        nextRoom.transform.position = targetExit.position - rotatedOffset + spacingOffset;

        nextRoom.transform.SetParent(this.transform);

        // ВІЗУАЛІЗАЦІЯ В ЕДИТОРІ (малює лінію від виходу до нового центра)
        Debug.DrawLine(targetExit.position, nextRoom.transform.position, Color.red, 10f);

        // 4. ВИДАЛЕННЯ ПРОХОДУ
        GameObject exitToDestroy = targetExit.gameObject;
        prevRoom.exitPoints[prevExitIdx] = null;
        Destroy(exitToDestroy);

        activeRooms.Add(nextRoom.gameObject);
        return nextRoom;
    }

    public void ClearDungeon()
    {
        foreach (GameObject r in activeRooms) if (r != null) Destroy(r);
        activeRooms.Clear();
    }
}