using System;
using System.Collections.Generic;
using UnityEngine; // Для Debug.Log

// namespace map_test // Видаляємо для Unity

[System.Serializable]
public class Agressive_Location : Task_Location
{
    private GameManager gm;
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
        GameManager gm = GameManager.Instance;

        // Змінюємо стан, щоб вимкнути керування мапою
        gm.ChangeState(GameState.Battle);

        // Ховаємо рендерер мапи світу (вона залишається в пам'яті, але не вантажить відеокарту)
        gm.mapRenderer.gameObject.SetActive(false);

        // Викликаємо старт бою
        BattleManager.Instance.StartBattle(this);
    }
    public void end_battle()
    {
        
    }
}