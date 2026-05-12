using UnityEngine;
using UnityEngine.UI;
using TMPro; // 1. Додай це обов'язково!
using System.Collections.Generic;
using GameCore.Entities;

public class ShopUI : MonoBehaviour
{
    public GameObject listButtonPrefab;
    public Transform listContainer;

    [Header("Деталі (Справа)")]
    public TMP_Text detailName;  // 2. Зміни Text на TMP_Text
    public TMP_Text detailStats; // 2. Зміни Text на TMP_Text
    public TMP_Text detailPrice; // 2. Зміни Text на TMP_Text
    public Button buyButton;
    private Weapon selectedWeapon;

    // Виклич цей метод, коли гравець заходить у місто
    public void OpenShop()
    {
        gameObject.SetActive(true);
        RefreshList();
        buyButton.gameObject.SetActive(false); // Ховаємо кнопку купити, поки нічого не обрано
    }

    void RefreshList()
    {
        foreach (Transform child in listContainer) Destroy(child.gameObject);

        // Беремо зброю, де shop = true
        List<Weapon> goods = WeaponDatabase.AllWeapons.FindAll(w => w.shop);

        foreach (Weapon w in goods)
        {
            GameObject go = Instantiate(listButtonPrefab, listContainer);
            go.GetComponent<ShopListItem>().Setup(w, this);
        }
    }

    public void SelectWeapon(Weapon w)
    {
        selectedWeapon = w;
        detailName.text = w.Pname;
        detailStats.text = $"Шкода: {w.base_damage}\nСкейлінг: {w.attack_type}";
        detailPrice.text = $"Ціна: {w.price} золота";

        buyButton.gameObject.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        Player p = GameManager.Instance.currentPlayer;
        if (p.money >= selectedWeapon.price)
        {
            p.money -= selectedWeapon.price;
            p.inventory.AddItem(selectedWeapon);
            GameManager.Instance.hudManager.UpdateHUD(); // Оновлюємо гроші на екрані
            Debug.Log("Предмет придбано!");
        }
        else
        {
            Debug.Log("Недостатньо грошей!");
        }
    }
}