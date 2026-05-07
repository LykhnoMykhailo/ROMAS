using System.Collections.Generic;
using System.IO;
using GameCore.Entities; // Ваш клас Player
using map_test;          // Ваш MapManager
using UnityEngine;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Системи")]
    public MapManager mapManager;
    public MapRenderer mapRenderer;
    public Player currentPlayer;

    [Header("UI та Сцена")]
    public GameObject mainMenuCanvas;
    public CameraController cameraController;

    [Header("Герой")]
    public GameObject visualPlayer; // Ваш Player_GFX (лицар)

    [Header("Поточний стан")]
    public string currentGameName = "";
    [Header("Налаштування руху")]
    public float moveDelay = 0.2f; // Час між кроками (0.2 - швидше, 0.5 - повільніше)
    private float nextMoveTime = 0f;
    private void Awake()
    {
        // Singleton патерн
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

        mapManager = new MapManager();
    }

    /// <summary>
    /// Створення нової гри
    /// </summary>
    public void CreateNewGame(string playerName)
    {
        currentGameName = playerName;

        // Ініціалізація даних (старт зазвичай на 500, 500)
        currentPlayer = new Player();
        currentPlayer.InitializeNewPlayer(playerName, currentGameName);

        // Генерація світу 1000x1000
        Debug.Log($"<color=green>[GameManager]</color> Починаємо створення світу: {playerName}");
        mapManager.CreateNewWorld(1000, 1000, currentGameName, Random.Range(0, 999999));

        StartGameplay();
        SaveAll();
    }

    /// <summary>
    /// Завантаження існуючої гри
    /// </summary>
    public void LoadGame(string worldName)
    {
        currentGameName = worldName;

        // Завантажуємо дані
        mapManager.LoadWorld(worldName);
        currentPlayer = Player.LoadPlayer(worldName);

        if (currentPlayer != null && mapManager.CurrentMap != null)
        {
            StartGameplay();
            Debug.Log($"<color=green>[GameManager]</color> Гру '{worldName}' завантажено.");
        }
        else
        {
            Debug.LogError("[GameManager] Помилка завантаження даних!");
        }
    }

    /// <summary>
    /// Запуск ігрового процесу та активація об'єктів
    /// </summary>
    private void StartGameplay()
    {
        // 1. Ховаємо меню
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        // 2. Налаштовуємо камеру на Grid (50, 50)
        if (cameraController != null)
        {
            cameraController.SetupCamera();
        }

        // 3. Активуємо та позиціонуємо героя в центрі вікна
        if (visualPlayer != null && cameraController != null)
        {
            visualPlayer.SetActive(true);
            // Центр Grid (50,50) + зміщення вікна (11.5, 9.5)
            float pX = cameraController.gridTransform.position.x + 11.5f;
            float pY = cameraController.gridTransform.position.y + 9.5f;
            visualPlayer.transform.position = new Vector3(pX, pY, -1f);
        }

        // 4. Перший рендер мапи
        RefreshMapDisplay();
    }

    /// <summary>
    /// Оновлення візуального відображення тайлів
    /// </summary>
    public void RefreshMapDisplay()
    {
        if (mapManager.CurrentMap != null && mapRenderer != null && currentPlayer != null)
        {
            // Викликаємо рендерер для малювання вікна навколо координат гравця
            mapRenderer.UpdateView(mapManager.CurrentMap, currentPlayer.X, currentPlayer.Y);
        }
    }

    public void SaveAll()
    {
        if (string.IsNullOrEmpty(currentGameName)) return;

        string worldFolderPath = Path.Combine(Application.persistentDataPath, "Saves", currentGameName);
        if (!Directory.Exists(worldFolderPath)) Directory.CreateDirectory(worldFolderPath);

        currentPlayer.SavePlayerData(worldFolderPath);
        mapManager.SaveCurrentMap(worldFolderPath);
    }

    private void Update()
    {
        if (currentPlayer == null) return;

        // Перевіряємо, чи пройшло достатньо часу з моменту останнього кроку
        if (Time.time >= nextMoveTime)
        {
            HandleMovement();
        }
    }
    private void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2Int direction = Vector2Int.zero;

        // Використовуємо .isPressed для відстеження затиснутої клавіші
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) direction.y += 1;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) direction.y += -1;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) direction.x += -1;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) direction.x += 1;

        // Якщо натиснута будь-яка клавіша руху
        if (direction != Vector2Int.zero)
        {
            currentPlayer.position.x += direction.x;
            currentPlayer.position.y += direction.y;

            // Встановлюємо час для наступного дозволеного кроку
            nextMoveTime = Time.time + moveDelay;

            RefreshMapDisplay();

            // Додатково: розвертаємо спрайт героя (якщо він є)
            if (direction.x != 0 && visualPlayer != null)
            {
                visualPlayer.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            }
        }
    }
}