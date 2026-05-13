// --- Класи для серіалізації даних (Data Only) ---

using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int prefabIndex;     // Індекс одного з 5 префабів кімнат
    public Vector2Int gridPos;  // Позиція в матриці (x, y)

    // Стан стін-заглушок
    public bool wallTop = true;
    public bool wallDown = true;
    public bool wallLeft = true;
    public bool wallRight = true;

    public RoomData(int index, Vector2Int pos)
    {
        prefabIndex = index;
        gridPos = pos;
    }
}

[System.Serializable]
public class EntitySnapshot
{
    public string entityID;      // Назва префабу (наприклад, "Grineer_Lancer")
    public string entityClan;    // Клан суб'єкта (визначає ворог він чи друг)
    public Vector3 localPos;     // Координати з enemyPoints
    public float currentHP;
    public bool isDead;

    public EntitySnapshot(string id, string clan, Vector3 pos)
    {
        entityID = id;
        entityClan = clan;
        localPos = pos;
        isDead = false;
    }
}