using UnityEngine;
using UnityEngine.UI;
using GameCore.Entities;

public class ShopSlot : MonoBehaviour
{
    public Text nameText;
    public Text priceText;
    private Weapon itemData;
    private ShopUI shopUI;

    public void Setup(Weapon weapon, ShopUI ui)
    {
        itemData = weapon;
        shopUI = ui;
        nameText.text = weapon.Pname;
        priceText.text = $"{weapon.price} золота";
    }

    // Викликається при натисканні на рядок (як у Skyrim)
    public void OnClick()
    {
        // Тепер назва збігається з методом у ShopUI
        shopUI.SelectWeapon(itemData);
    }
}