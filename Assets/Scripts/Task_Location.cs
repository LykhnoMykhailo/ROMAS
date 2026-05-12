using System;
using Newtonsoft.Json; // Використовуємо замість System.Text.Json
using UnityEngine;

// namespace map_test // Видаляємо для Unity

[System.Serializable]
// Newtonsoft буде використовувати це поле, щоб розуміти, який це саме клас (Agressive чи Netral)
public abstract class Task_Location
{

    // Ідентифікатор типу для Newtonsoft (альтернатива JsonDerivedType)
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    public Task_Location() { }

    // Абстрактний метод, який реалізують нащадки
    public abstract void OnInteract();
}