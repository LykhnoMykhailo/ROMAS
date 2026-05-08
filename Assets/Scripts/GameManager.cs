using System.Collections.Generic;
using System.IO;
using GameCore.Entities; // Ваш клас Player
using map_test;          // Ваш MapManager
using UnityEngine;

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
    public GameObject visualPlayer;

    [Header("Поточний стан")]
    public string currentGameName = "";

    private void Awake()
    {
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

    private void OnEnable()
    {
        // Підписуємося на події вводу при активації об'єкта
        InputHandler.OnMoveInput += HandleMapMovement;
    }

    private void OnDisable()
    {
        // Відписуємося при деактивації (важливо для уникнення помилок)
        InputHandler.OnMoveInput -= HandleMapMovement;
    }

    public void CreateNewGame(string playerName)
    {
        currentGameName = playerName;
        currentPlayer = new Player();
        currentPlayer.InitializeNewPlayer(playerName, currentGameName);

        Debug.Log($"<color=green>[GameManager]</color> Створення світу: {playerName}");
        mapManager.CreateNewWorld(1000, 1000, currentGameName, Random.Range(0, 999999));

        StartGameplay();
        SaveAll();
    }

    public void LoadGame(string worldName)
    {
        currentGameName = worldName;
        mapManager.LoadWorld(worldName);
        currentPlayer = Player.LoadPlayer(worldName);

        if (currentPlayer != null && mapManager.CurrentMap != null)
        {
            StartGameplay();
        }
        else
        {
            Debug.LogError("[GameManager] Помилка завантаження!");
        }
    }

    private void StartGameplay()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);

        if (cameraController != null) cameraController.SetupCamera();

        if (visualPlayer != null && cameraController != null)
        {
            visualPlayer.SetActive(true);
            // Позиціонування відносно Grid (50,50)
            float pX = cameraController.gridTransform.position.x + 11.5f;
            float pY = cameraController.gridTransform.position.y + 9.5f;
            visualPlayer.transform.position = new Vector3(pX, pY, -1f);
        }

        RefreshMapDisplay();
    }

    /// <summary>
    /// Логіка руху ГЛОБАЛЬНОЮ мапою
    /// </summary>
    private void HandleMapMovement(Vector2Int direction)
    {
        if (currentPlayer == null || mapManager.CurrentMap == null) return;

        // Розрахунок нових координат
        int nextX = currentPlayer.X + direction.x;
        int nextY = currentPlayer.Y + direction.y;

        // Перевірка меж карти (запобігає виходу за 0 або 999)
        if (nextX >= 0 && nextX < mapManager.CurrentMap.Width &&
            nextY >= 0 && nextY < mapManager.CurrentMap.Height)
        {
            currentPlayer.position.x = nextX;
            currentPlayer.position.y = nextY;

            RefreshMapDisplay();

            // Поворот спрайту
            if (direction.x != 0 && visualPlayer != null)
            {
                visualPlayer.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            }
        }
    }

    public void RefreshMapDisplay()
    {
        if (mapManager.CurrentMap != null && mapRenderer != null && currentPlayer != null)
        {
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
}