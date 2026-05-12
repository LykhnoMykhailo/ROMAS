using UnityEngine;
using TMPro;
using System.Text;

public class HUDManager : MonoBehaviour
{
    public TMP_Text nameLevelText;
    public TMP_Text statsText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text coordinatesText;
    public void UpdateHUD()
    {
        if (GameManager.Instance.currentPlayer != null)
        {
            // Припустимо, у вас є текстове поле для грошей
            // moneyText.text = $"Gold: {GameManager.Instance.currentPlayer.money}";

            Debug.Log($"[HUD] Інтерфейс оновлено. Гроші: {GameManager.Instance.currentPlayer.money}");
        }
    }
    private void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.currentPlayer == null) return;

        var p = gm.currentPlayer;

        // Виводимо дані, які ти прописав у Player.cs та Puppet.cs
        nameLevelText.text = $"{p.Pname} [Lvl {p.lvl}]";

        // Форматуємо статс: Сила (st), Спритність (ag), Знання (kn), Магія (mp)
        statsText.text = $"STR: {p.st} AGI: {p.ag}\nKNW: {p.kn} MAG: {p.mp}";

        // Здоров'я та мана (використовуємо hp_battle для поточного стану)
        hpText.text = $"HP: {Mathf.RoundToInt(p.hp_battle)}/{Mathf.RoundToInt(p.hp)}";
        mpText.text = $"MP: {Mathf.RoundToInt(p.mana_battle)}/{Mathf.RoundToInt(p.mana)}";

        // Координати X/Y (використовуємо твої властивості з Player.cs)
        coordinatesText.text = $"X: {p.X}, Y: {p.Y}";
    }
}