using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class SaveManagerUI : MonoBehaviour
{
    [Header("Префаби та Контейнери")]
    public GameObject slotPrefab;
    public Transform gridContainer;

    [Header("Керування Сторінками")]
    public TMP_Text pageText;
    public Button nextButton;
    public Button prevButton;
    public int itemsPerPage = 8;

    private List<string> allWorldNames = new List<string>();
    private int currentPage = 0;

    // Викликається Unity автоматично, коли Canvas вмикається
    private void OnEnable()
    {
        RefreshWorldsList();
    }

    public void RefreshWorldsList()
    {
        string path = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        allWorldNames = Directory.GetDirectories(path)
            .Select(p => Path.GetFileName(p))
            .ToList();

        DisplayPage(0);
    }

    // МЕТОДИ МАЮТЬ БУТИ PUBLIC, щоб Unity їх бачила
    public void DisplayPage(int page)
    {
        currentPage = page;
        foreach (Transform child in gridContainer) Destroy(child.gameObject);

        int startIndex = page * itemsPerPage;
        var worldsToShow = allWorldNames.Skip(startIndex).Take(itemsPerPage);

        foreach (string wName in worldsToShow)
        {
            GameObject slot = Instantiate(slotPrefab, gridContainer);

            // Шукаємо текст саме в об'єкті з назвою "WorldNameText"
            TMP_Text title = slot.transform.Find("WorldNameText")?.GetComponent<TMP_Text>();
            if (title != null) title.text = wName;
            else slot.GetComponentInChildren<TMP_Text>().text = wName; // Запасний варіант

            // Налаштування кнопок всередині слота
            Button[] btns = slot.GetComponentsInChildren<Button>();
            string nameForClosure = wName;

            foreach (var btn in btns)
            {
                if (btn.name == "LoadButton")
                    btn.onClick.AddListener(() => LoadGame(nameForClosure));
                else if (btn.name == "DeleteButton")
                    btn.onClick.AddListener(() => DeleteGame(nameForClosure));
            }
        }

        // Оновлення тексту та кнопок навігації (як на фото)
        if (pageText != null) pageText.text = (currentPage + 1).ToString();
        if (prevButton != null) prevButton.interactable = currentPage > 0;
        if (nextButton != null) nextButton.interactable = startIndex + itemsPerPage < allWorldNames.Count;
    }

    public void NextPage() => DisplayPage(currentPage + 1);
    public void PrevPage() => DisplayPage(currentPage - 1);

    public void LoadGame(string worldName)
    {
        GameManager.Instance.LoadGame(worldName);
        CloseMenu();
    }

    public void DeleteGame(string worldName)
    {
        string path = Path.Combine(Application.persistentDataPath, "Saves", worldName);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            RefreshWorldsList();
        }
    }

    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
    }
}