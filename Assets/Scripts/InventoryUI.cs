using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameCore.Entities;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Налаштування списку")]
    public GameObject buttonPrefab;
    public Transform container;

    [Header("Панель деталей")]
    public GameObject detailsPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descriptionText;

    [Header("Кнопки дій")]
    public GameObject equipButton; // Об'єкт кнопки Equipt
    public TextMeshProUGUI sellOneBtnText;
    public TextMeshProUGUI sellAllBtnText;

    [Header("Посилання")]
    public GameObject inventoryPanel; // ЦЕ ПОЛЕ МАЄ БУТИ ТУТ

    private InventorySlot selectedSlot;

    public void RefreshUI()
    {
        var player = GameManager.Instance.currentPlayer;
        if (player == null || player.inventory == null) return;

        // Очищення списку
        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (InventorySlot slot in player.inventory.slots)
        {
            if (slot == null || slot.item == null) continue;

            GameObject newBtn = Instantiate(buttonPrefab, container);
            var txt = newBtn.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null)
            {
                string displayName = slot.item.Pname;
                // Позначка спорядженої зброї
                if (slot.item is Weapon w && player.Weapon_use == w)
                {
                    displayName = $"<color=#00FF00>{displayName} (use)</color>";
                }
                txt.text = slot.count > 1 ? $"{displayName} ({slot.count})" : displayName;
            }

            newBtn.GetComponent<Button>().onClick.AddListener(() => ShowDetails(slot));
        }
    }

    public void ShowDetails(InventorySlot slot)
    {
        selectedSlot = slot;
        detailsPanel.SetActive(true);
        var player = GameManager.Instance.currentPlayer;

        nameText.text = slot.item.Pname;
        descriptionText.text = slot.item.description;

        // Якщо це зброя (клас Weapon)
        if (slot.item is Weapon w)
        {
            statsText.text = $"Шкода: {w.base_damage}\nЦіна: {w.price}";

            if (equipButton != null)
            {
                equipButton.SetActive(true); // ТЕПЕР ВОНА МАЄ ВВІМКНУТИСЯ
                var btnText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = (player.Weapon_use == w) ? "Зняти" : "Спорядити";
            }
        }
        else
        {
            // Для звичайних Item (зілля тощо) ховаємо кнопку
            statsText.text = $"Ціна: {slot.item.price}";
            if (equipButton != null) equipButton.SetActive(false);
        }

        if (sellOneBtnText != null) sellOneBtnText.text = $"Продати 1 ({slot.item.price})";
        if (sellAllBtnText != null) sellAllBtnText.text = $"Продати все ({slot.item.price * slot.count})";
    }
    public void OnEquipClick()
    {
        var player = GameManager.Instance.currentPlayer;
        if (selectedSlot == null || player == null || !(selectedSlot.item is Weapon w)) return;

        if (player.Weapon_use == w) player.Weapon_use = null;
        else player.Weapon_use = w;

        RefreshUI();
        ShowDetails(selectedSlot); // Оновлюємо текст на кнопці, щоб вона не зникала
    }

    public void OnSellOne()
    {
        var player = GameManager.Instance.currentPlayer;
        if (selectedSlot != null && player != null)
        {
            player.money += selectedSlot.item.price;
            player.inventory.RemoveItem(selectedSlot.item, 1);
            RefreshUI();
            if (selectedSlot.count <= 0) detailsPanel.SetActive(false);
            else ShowDetails(selectedSlot);
        }
    }

    public void OnSellAll()
    {
        var player = GameManager.Instance.currentPlayer;
        if (selectedSlot != null && player != null)
        {
            player.money += selectedSlot.item.price * selectedSlot.count;
            player.inventory.RemoveItem(selectedSlot.item, selectedSlot.count);
            RefreshUI();
            detailsPanel.SetActive(false);
        }
    }
}