using GameCore.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopListItem : MonoBehaviour
{
    public TMP_Text nameText;
    private Weapon weaponData;
    private ShopUI shopUI;

    public void Setup(Weapon w, ShopUI ui)
    {
        weaponData = w;
        shopUI = ui;
        nameText.text = w.Pname;
    }

    public void OnClick()
    {
        // Коли тиснемо на кнопку в списку, передаємо дані в праву панель
        shopUI.SelectWeapon(weaponData);
    }
}