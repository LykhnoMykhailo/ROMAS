using System.Collections.Generic;
using System.IO;
using GameCore.Entities;
using map_test;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameState { MainMenu, WorldMap, Battle, Pause, Inventory, Shop }
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
    public HUDManager hudManager;
    public GameObject inGameMenuCanvas;
    public GameObject saveManagerUI;

    [Header("Герой")]
    public GameObject visualPlayer;

    [Header("Поточний стан")]
    public string currentGameName = "";
    public GameState currentState = GameState.MainMenu;
    [Header("Інвентар")]
    public InventoryUI inventoryUI; // Скрипт, який ми створили раніше

    // Цей метод ми викликатимемо через кнопку
    public void ToggleInventory()
    {
        if (currentState == GameState.WorldMap)
            ChangeState(GameState.Inventory);
        else if (currentState == GameState.Inventory)
            ChangeState(GameState.WorldMap);
    }
    [Header("Магазин")]
    public ShopUI shopUI;

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // Зупиняємо час у магазині, як і в інвентарі
        Time.timeScale = (newState == GameState.Pause || newState == GameState.Inventory || newState == GameState.Shop) ? 0f : 1f;

        if (newState == GameState.WorldMap)
        {
            // 1. Вмикаємо все необхідне
            if (mapRenderer != null) mapRenderer.gameObject.SetActive(true);
            if (visualPlayer != null) visualPlayer.SetActive(true);
            if (hudManager != null) hudManager.gameObject.SetActive(true);

            // 2. Скидаємо камеру на сітку мапи
            if (cameraController != null) cameraController.SetupCamera();

            if (visualPlayer != null && cameraController != null)
            {
                visualPlayer.SetActive(true);
                float pX = cameraController.gridTransform.position.x + 11.5f;
                float pY = cameraController.gridTransform.position.y + 9.5f;
                visualPlayer.transform.position = new Vector3(pX, pY, -1f);
            }
        }

        // Логіка вікна магазину
        if (shopUI != null)
        {
            shopUI.gameObject.SetActive(newState == GameState.Shop);
            if (newState == GameState.Shop) shopUI.OpenShop();
        }

        // 1. Керування часом
        Time.timeScale = (newState == GameState.Pause || newState == GameState.Inventory) ? 0f : 1f;

        // 2. HUD (Тексти на карті)
        if (hudManager != null)

            hudManager.gameObject.SetActive(newState == GameState.WorldMap);

        // 3. Меню паузи (Esc)
        if (inGameMenuCanvas != null)
            inGameMenuCanvas.SetActive(newState == GameState.Pause);

        // 4. Інвентар
        if (inventoryUI != null && inventoryUI.inventoryPanel != null)
        {
            // Вмикаємо панель тільки якщо стан Inventory
            inventoryUI.inventoryPanel.SetActive(newState == GameState.Inventory);

            if (newState == GameState.Inventory)
            {
                inventoryUI.RefreshUI(); // Оновлюємо кнопки
            }
        }
    }

    private void Awake()
    {
        string dbPath = Path.Combine(Application.streamingAssetsPath, "data_b/Weapons/Weapons.json");

        if (File.Exists(dbPath))
        {
            string jsonContent = File.ReadAllText(dbPath);
            WeaponDatabase.Initialize(jsonContent);
            Debug.Log("<color=green>[WeaponDatabase]</color> Базу завантажено зі StreamingAssets.");
        }
        else
        {
            Debug.LogError($"[WeaponDatabase] Файл не знайдено за шляхом: {dbPath}");
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else { Destroy(gameObject); return; }

        mapManager = new MapManager();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        // Пауза на Esc
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleInGameMenu();
        }

        // Взаємодія на E
        if (Keyboard.current.eKey.wasPressedThisFrame && currentState == GameState.WorldMap)
        {
            CheckInteraction();
        }
    }
    private void CheckInteraction()
    {
        if (currentPlayer == null || mapManager.CurrentMap == null)
        {
            Debug.LogError("<color=red>[Shop-Debug]</color> Гравець або Карта не ініціалізовані!");
            return;
        }

        int px = currentPlayer.X;
        int py = currentPlayer.Y;

        Debug.Log($"<color=cyan>[Shop-Debug]</color> Намагаємось взаємодіяти. Координати гравця: {px}, {py}");

        // 1. Отримуємо локацію через твій метод
        // 1. Отримуємо локацію на координатах гравця
        Location_tile loc = mapManager.CurrentMap.GetLocation(px, py);

        if (loc != null)
        {
            Debug.Log($"<color=green>[Interaction]</color> Взаємодія з локацією: {loc.Id}");

            // 2. Просто викликаємо Interact(). 
            // Вона сама знає, викликати OnInteract() з Netral_Location чи Agressive_Location
            loc.Interact();
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[Interaction]</color> На координатах {px},{py} порожньо.");
        }
    }
    public void ToggleInGameMenu()
    {
        if (currentState == GameState.MainMenu) return;

        if (currentState == GameState.Pause)
            ChangeState(GameState.WorldMap);
        else
            ChangeState(GameState.Pause);
    }
    public void CloseShop()
    {
        ChangeState(GameState.WorldMap);
    }
    public void OpenLoadMenuFromPause()
    {
        if (saveManagerUI != null)
        {
            saveManagerUI.SetActive(true);
        }
    }

    public void CreateNewGame(string playerName)
    {
        currentGameName = playerName;
        currentPlayer = new Player();
        currentPlayer.InitializeNewPlayer(playerName, currentGameName);
        mapManager.CreateNewWorld(1000, 1000, currentGameName, Random.Range(0, 999999));

        StartGameplay();
        SaveAll();
    }

    public void LoadGame(string worldName)
    {
        currentGameName = worldName;
        mapManager.LoadWorld(worldName);
        currentPlayer = Player.LoadPlayer(worldName);

        if (currentPlayer != null)
        {
            // ВАЖЛИВО: спочатку міняємо стан, щоб вимкнути UI
            ChangeState(GameState.WorldMap);
            StartGameplay();
        }
        else
        {
            Debug.LogError("[GameManager] Помилка завантаження файлів гравця!");
        }
    }

    private void StartGameplay()
    {
        // Примусово вимикаємо всі зайві UI через зміну стану
        ChangeState(GameState.WorldMap);

        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (inGameMenuCanvas != null) inGameMenuCanvas.SetActive(false);

        // Налаштування камери
        if (cameraController != null) cameraController.SetupCamera();

        // Налаштування візуалу гравця
        if (visualPlayer != null && cameraController != null)
        {
            visualPlayer.SetActive(true);
            float pX = cameraController.gridTransform.position.x + 11.5f;
            float pY = cameraController.gridTransform.position.y + 9.5f;
            visualPlayer.transform.position = new Vector3(pX, pY, -1f);
        }

        RefreshMapDisplay();
    }

    private void HandleMapMovement(Vector2Int direction)
    {
        if (currentState != GameState.WorldMap) return;
        if (currentPlayer == null || mapManager.CurrentMap == null) return;

        int nextX = currentPlayer.X + direction.x;
        int nextY = currentPlayer.Y + direction.y;

        if (nextX >= 0 && nextX < mapManager.CurrentMap.Width &&
            nextY >= 0 && nextY < mapManager.CurrentMap.Height)
        {
            currentPlayer.position.x = nextX;
            currentPlayer.position.y = nextY;
            RefreshMapDisplay();

            if (direction.x != 0 && visualPlayer != null)
                visualPlayer.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }
    }

    public void RefreshMapDisplay()
    {
        if (mapManager.CurrentMap != null && mapRenderer != null && currentPlayer != null)
            mapRenderer.UpdateView(mapManager.CurrentMap, currentPlayer.X, currentPlayer.Y);
    }

    public void SaveAll()
    {
        if (string.IsNullOrEmpty(currentGameName)) return;
        string worldFolderPath = Path.Combine(Application.persistentDataPath, "Saves", currentGameName);
        if (!Directory.Exists(worldFolderPath)) Directory.CreateDirectory(worldFolderPath);

        currentPlayer.SavePlayerData(worldFolderPath);
        mapManager.SaveCurrentMap(worldFolderPath);
        Debug.Log("<color=cyan>[GameManager]</color> Збереження виконано успішно!");
    }

    private void OnEnable() { InputHandler.OnMoveInput += HandleMapMovement; }
    private void OnDisable() { InputHandler.OnMoveInput -= HandleMapMovement; }
}