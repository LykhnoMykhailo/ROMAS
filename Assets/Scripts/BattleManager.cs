using UnityEngine;
using GameCore.Entities;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Посилання")]
    public GameObject arenaContainer;
    public Transform playerSpawnPoint;

    private void Awake() => Instance = this;

    public void StartBattle(Agressive_Location data)
    {
        arenaContainer.SetActive(true);

        // 1. Очищення та налаштування ліміту
        LevelGenerator.Instance.ClearDungeon();
        //LevelGenerator.Instance.totalRoomsLimit = data.CountOfRoom;

        // 2. Генерація
        LevelGenerator.Instance.StartDungeonGeneration();

        // 3. Телепортація гравця
        GameObject playerObj = GameManager.Instance.visualPlayer;
        playerObj.transform.position = Vector3.zero;

        Debug.Log($"<color=green>[Battle]</color> Данж згенеровано на {data.CountOfRoom} кімнат.");
    }

    public void EndBattle()
    {
        LevelGenerator.Instance.ClearDungeon();
        arenaContainer.SetActive(false);
        GameManager.Instance.ChangeState(GameState.WorldMap);
        GameManager.Instance.mapRenderer.gameObject.SetActive(true);
    }
}