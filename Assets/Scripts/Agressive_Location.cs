using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Agressive_Location : Task_Location
{
    // --- Дані про локацію на ГК ---
    public string Clan { get; set; }        // Клан-власник локації (фракція ворогів)
    public int CountOfRoom { get; set; }   // Бажана кількість кімнат
    public bool Boss { get; set; }         // Чи генерувати кімнату з босом

    // --- Логічна мапа (Структура данжу) ---
    public List<RoomData> GeneratedRooms { get; set; }

    // --- Реєстр суб'єктів (Puppet entities) ---
    // Сюди записуємо і ворогів, і потенційних союзників
    public List<EntitySnapshot> Subjects { get; set; }
    public List<EntitySnapshot> Objects { get; set; }  // Скрині, декор

    public Agressive_Location()
    {
        Type = "Agressive";
        GeneratedRooms = new List<RoomData>();
        Subjects = new List<EntitySnapshot>();
        Objects = new List<EntitySnapshot>();
    }

    /// <summary>
    /// Перехід від глобальної мапи до бойової локації
    /// </summary>
    public override void OnInteract()
    {
        GameManager gm = GameManager.Instance;
        gm.ChangeState(GameState.Battle);

        if (gm.mapRenderer != null)
            gm.mapRenderer.gameObject.SetActive(false);

        // Передаємо дані в BattleManager, який через Renderer 
        // створить фізичні об'єкти на основі цих списків
        BattleManager.Instance.StartBattle(this);
    }
}