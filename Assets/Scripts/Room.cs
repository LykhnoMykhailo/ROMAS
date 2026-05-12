using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform[] exitPoints;
    public Transform spawnPoint; // Тільки для стартової
    public Transform[] enemyPoints; // Для звичайних ворогів

    [Header("Special Objects")]
    public Transform chestPoint; // Точка для скрині
    public Transform bossPoint;  // Точка для боса (якщо є)
}