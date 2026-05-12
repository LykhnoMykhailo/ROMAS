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
        // Повідомляємо GameManager, що ми в місті/магазині
        GameManager.Instance.ChangeState(GameState.Shop);
        // Передаємо дані про магазин в UI
        GameManager.Instance.shopUI.OpenShop();
    }
}