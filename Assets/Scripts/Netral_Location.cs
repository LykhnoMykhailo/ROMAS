using System;
using System.Collections.Generic;
using UnityEngine; // Для Debug.Log

// namespace map_test // Видаляємо для Unity

[System.Serializable]
public class Netral_Location : Task_Location
{
    public List<string> Quests { get; set; }
    public string Shop { get; set; }

    public Netral_Location()
    {
        Type = "Netral";
        // Завжди ініціалізуємо списки, щоб Unity та Newtonsoft не видавали null
        Quests = new List<string>();
        Shop = "Unknown Merchant";
    }

    public override void OnInteract()
    {
        // Використовуємо кольорове маркування для зручності в консолі Unity
        Debug.Log($"<color=green>[Trade]</color> Відкрито торгівлю з <b>{Shop}</b>. Доступно квестів: {Quests.Count}");

        // Тут буде логіка відкриття UI вікна маркету або діалогу
        // UIManager.Instance.OpenShop(Shop, Quests);
    }
}