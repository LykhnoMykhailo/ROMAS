using UnityEngine;
using System.IO;
using System.Collections.Generic;
using GameCore.Entities; // Для вашого класу Player
using map_test;          // Для вашого MapManager та WordMapTile
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Системи")]
    public MapManager mapManager;
    public MapRenderer mapRenderer;
    public Player currentPlayer;

    [Header("Поточний стан")]
    public string currentGameName = "";

    private void Awake()
    {
        // Налаштування синглтону
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ініціалізуємо менеджер карти один раз при старті
        mapManager = new MapManager();
    }

    /// <summary>
    /// Створює нову гру, генерує світ та відмальовує початкову зону
    /// </summary>
    public void CreateNewGame(string playerName)
    {
        currentGameName = playerName;

        // 1. Створюємо та ініціалізуємо гравця
        currentPlayer = new Player();
        currentPlayer.InitializeNewPlayer(playerName, currentGameName);

        // Стартові предмети
        currentPlayer.inventory.AddItem("weapon", "base_sword");
        currentPlayer.inventory.AddItem("weapon", "bow");

        // 2. Генеруємо світ (дані записуються в mapManager.CurrentMap)
        // Використовуємо 1000x1000, бо тепер рендеримо лише частину
        mapManager.CreateNewWorld(1000, 1000, currentGameName, Random.Range(0, 999999));

        // 3. Візуалізація: малюємо зону навколо початкових координат гравця
        RefreshMapDisplay();

        // 4. Зберігаємо початковий стан
        SaveAll();

        Debug.Log($"[GameManager] Нову гру '{playerName}' створено та відмальовано.");
    }

    /// <summary>
    /// Завантажує існуючу гру та оновлює екран
    /// </summary>
    public void LoadGame(string worldName)
    {
        currentGameName = worldName;
        string worldFolderPath = Path.Combine(Application.persistentDataPath, "Saves", worldName);

        if (Directory.Exists(worldFolderPath))
        {
            // 1. Завантажуємо дані мапи та гравця
            mapManager.LoadWorld(worldName);
            currentPlayer = Player.LoadPlayer(worldName);

            // 2. Оновлюємо візуалізацію
            RefreshMapDisplay();

            Debug.Log($"[GameManager] Гру '{worldName}' успішно завантажено.");
        }
        else
        {
            Debug.LogError($"[GameManager] Папку збереження '{worldName}' не знайдено за шляхом: {worldFolderPath}");
        }
    }

    /// <summary>
    /// Оновлює Tilemap, базуючись на поточних координатах гравця
    /// </summary>
    public void RefreshMapDisplay()
    {
        if (mapManager.CurrentMap != null && mapRenderer != null && currentPlayer != null)
        {
            // Викликаємо оновлення "вікна" 23x19
            // Використовуємо властивість CurrentMap, яку ми додали в MapManager
            mapRenderer.UpdateView(mapManager.CurrentMap, currentPlayer.X, currentPlayer.Y);
        }
        else
        {
            Debug.LogWarning("[GameManager] Неможливо оновити мапу: відсутні дані або Renderer.");
        }
    }

    /// <summary>
    /// Зберігає поточний прогрес у файли
    /// </summary>
    public void SaveAll()
    {
        if (string.IsNullOrEmpty(currentGameName)) return;

        string worldFolderPath = Path.Combine(Application.persistentDataPath, "Saves", currentGameName);

        if (!Directory.Exists(worldFolderPath))
        {
            Directory.CreateDirectory(worldFolderPath);
        }

        currentPlayer.SavePlayerData(worldFolderPath);
        mapManager.SaveCurrentMap(worldFolderPath);

        Debug.Log($"[GameManager] Всі дані збережено у: {worldFolderPath}");
    }

    // Тимчасовий метод для тестування управління (можна видалити після налаштування UI)
    private void Update()
    {
        // Новий спосіб перевірки натискання клавіші R
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RefreshMapDisplay();
        }
    }
}