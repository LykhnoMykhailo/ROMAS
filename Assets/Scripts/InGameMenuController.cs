using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuController : MonoBehaviour
{
    [Header("Посилання на вікна")]
    public GameObject saveManagerUI; // Сюди перетягнеш свій SaveManagerUI

    private void OnEnable()
    {
        // Коли меню відкривається — ставимо гру на паузу
        Time.timeScale = 0f;
        Debug.Log("[Menu] Пауза активована");
    }

    private void OnDisable()
    {
        // Коли меню закривається — повертаємо час у норму
        Time.timeScale = 1f;
        Debug.Log("[Menu] Гра триває");
    }

    // 1. Кнопка "Продовжити"
    public void Resume()
    {
        gameObject.SetActive(false);
    }

    // 2. Кнопка "Зберегти"
    public void Save()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveAll();
            // Можна додати текст "Збережено!" на екрані
            Debug.Log("Гру збережено успішно!");
        }
    }

    // 3. Кнопка "Завантажити" (Відкриває твоє вікно слотів)
    public void OpenLoadMenu()
    {
        if (saveManagerUI != null)
        {
            saveManagerUI.SetActive(true);
            // Саме меню паузи можна або сховати, або залишити під низом
            // gameObject.SetActive(false); 
        }
    }

    // 4. Кнопка "Вийти в головне меню"
    public void QuitToMain()
    {
        Time.timeScale = 1f; // Важливо! Повертаємо час перед виходом
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Оскільки у тебе все в одній сцені, це просто скине стан до MainMenu
    }
}