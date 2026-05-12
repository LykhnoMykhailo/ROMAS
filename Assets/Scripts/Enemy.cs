using UnityEngine;
using GameCore.Entities;
public class Enemy : Subject
{
    [Header("Enemy State")]
    public bool isAlerted = false; // „и пом≥тив гравц€
    public float detectionRange = 10f;

    public virtual void Update()
    {
        if (stats != null && stats.is_alive())
        {
            // “ут буде лог≥ка AI: патрулюванн€ або пересл≥дуванн€
        }
    }
}