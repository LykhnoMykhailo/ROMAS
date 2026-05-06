using System;
using System.Collections.Generic;
using UnityEngine; // Для Debug.Log

// namespace map_test // Видаляємо для Unity

[System.Serializable]
public class Agressive_Location : Task_Location
{
    // Дані про фракцію/власника локації
    public string Clan { get; set; }

    // Кількість кімнат, які мають бути згенеровані на полотні
    public int CountOfRoom { get; set; }

    // Чи є в кінці бос
    public bool Boss { get; set; }

    // Списки об'єктів, що існують на "полотні" локації
    // Використовуємо List<string> для прототипу, пізніше можна змінити на класи об'єктів
    public List<string> Rooms { get; set; }   // Координати та розміри кімнат
    public List<string> Walls { get; set; }   // Сегменти стін
    public List<string> Enemys { get; set; }  // Список ворогів
    public List<string> Objects { get; set; } // Декор, скрині, пастки

    public Agressive_Location()
    {
        Type = "Agressive";
        // Ініціалізуємо списки, щоб уникнути NullReferenceException при зверненні
        Rooms = new List<string>();
        Walls = new List<string>();
        Enemys = new List<string>();
        Objects = new List<string>();
    }

    /// <summary>
    /// Реалізація взаємодії: завантаження бойової сцени в Unity
    /// </summary>
    public override void OnInteract()
    {
        Debug.Log($"<color=red>[Battle]</color> Вхід у лігво клану <b>{Clan}</b>.");
        Debug.Log($"Генерація {CountOfRoom} кімнат на полотні локації...");

        if (Boss)
        {
            Debug.LogWarning("УВАГА: У цій локації присутній БОС!");
        }

        // В Unity тут зазвичай викликається SceneManager або запуск процедурної генерації об'єктів
        // GenerateInterior(); 
    }
}