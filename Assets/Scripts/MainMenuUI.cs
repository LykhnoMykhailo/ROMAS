using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Елементи")]
    public TMP_InputField nameInputField;
    public Button createButton;
    public Button loadButton;
    public Button exitButton;

    private void Start()
    {
        // Перевірка, чи всі об'єкти підключені в інспекторі (захист від NullReference)
        if (createButton == null || loadButton == null || exitButton == null || nameInputField == null)
        {
            Debug.LogError("[MainMenuUI] Не всі UI елементи підключені в Інспекторі!");
            return;
        }

        // Підписка на події
        createButton.onClick.AddListener(OnCreateClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnCreateClicked()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            // Перевіряємо наявність GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CreateNewGame(playerName);
                Debug.Log($"Починаємо створення світу для: {playerName}");
            }
            else
            {
                Debug.LogError("GameManager не знайдено на сцені!");
            }
        }
        else
        {
            Debug.LogWarning("Введіть ім'я світу!");
        }
    }

    private void OnLoadClicked()
    {
        string worldName = nameInputField.text;
        if (!string.IsNullOrWhiteSpace(worldName) && GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame(worldName);
        }
    }

    private void OnExitClicked()
    {
        Debug.Log("Вихід...");
        Application.Quit(); 
    }
}