using System.Collections.Generic;
using UnityEngine;
using GameCore.Entities;

public class ShopManager : MonoBehaviour
{
    public List<Weapon> availableWeapons;

    void Start()
    {
        // Завантажуємо асортимент магазину при старті локації
        availableWeapons = WeaponDatabase.GetShopWeapons();
    }

    public void BuyWeapon(Weapon weaponToBuy)
    {
        Player player = GameManager.Instance.currentPlayer;

        if (player.CanAfford(weaponToBuy.price))
        {
            player.money -= weaponToBuy.price;
            player.inventory.AddItem(weaponToBuy);

            Debug.Log($"<color=yellow>[Shop]</color> Ви купили {weaponToBuy.Pname} за {weaponToBuy.price}");

            // Оновлюємо інтерфейс магазину та інвентарю
            GameManager.Instance.hudManager.UpdateHUD(); // Оновити текст грошей
        }
        else
        {
            Debug.Log("<color=red>[Shop]</color> Недостатньо грошей!");
        }
    }
}