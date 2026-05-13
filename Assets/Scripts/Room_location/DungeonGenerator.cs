using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator
{
    private Dictionary<Vector2Int, RoomData> roomMap = new Dictionary<Vector2Int, RoomData>();

    public void Generate(Agressive_Location location)
    {
        roomMap.Clear();
        location.GeneratedRooms.Clear();

        // 1. Початкова кімната (0,0)
        Vector2Int currentPos = Vector2Int.zero;
        RoomData startRoom = CreateRoom(currentPos, 0);

        // 2. Вихід в одну сторону (наприклад, Вгору)
        Vector2Int nextPos = currentPos + Vector2Int.up;
        ConnectRooms(startRoom, CreateRoom(nextPos, Random.Range(0, 5)), Vector2Int.up);

        // 3. Створення 2-3 гілок від цієї другої кімнати
        int branches = Random.Range(2, 4);
        Vector2Int branchBase = nextPos;

        for (int i = 0; i < branches; i++)
        {
            GenerateBranch(branchBase, Random.Range(3, 7));
        }

        // Записуємо результат в локацію
        location.GeneratedRooms.AddRange(roomMap.Values);
    }

    private void GenerateBranch(Vector2Int startPoint, int length)
    {
        Vector2Int currentPos = startPoint;
        for (int i = 0; i < length; i++)
        {
            Vector2Int dir = GetRandomDirection();
            Vector2Int nextPos = currentPos + dir;

            // Перевірка, щоб не накладати кімнати
            if (roomMap.ContainsKey(nextPos)) continue;

            RoomData currentRoom = roomMap[currentPos];
            RoomData nextRoom = CreateRoom(nextPos, Random.Range(0, 5));
            ConnectRooms(currentRoom, nextRoom, dir);

            currentPos = nextPos;
        }
    }

    private RoomData CreateRoom(Vector2Int pos, int index)
    {
        if (roomMap.ContainsKey(pos)) return roomMap[pos];
        RoomData room = new RoomData(index, pos);
        roomMap.Add(pos, room);
        return room;
    }

    private void ConnectRooms(RoomData a, RoomData b, Vector2Int dir)
    {
        if (dir == Vector2Int.up) { a.wallTop = false; b.wallDown = false; }
        else if (dir == Vector2Int.down) { a.wallDown = false; b.wallTop = false; }
        else if (dir == Vector2Int.right) { a.wallRight = false; b.wallLeft = false; }
        else if (dir == Vector2Int.left) { a.wallLeft = false; b.wallRight = false; }
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[Random.Range(0, 4)];
    }
}