using UnityEngine;
using GameCore.Entities;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Посилання")]
    public GameObject arenaContainer; // Об'єкт BattleArenaContainer
    public Transform playerSpawnPoint; // Точка (0,0,0) на арені
    public GameObject dummyPrefab;    // Префаб манекена

    private void Awake() => Instance = this;

    public void StartBattle(Agressive_Location data)
    {
        arenaContainer.SetActive(true);

        // 1. Беремо посилання на візуального гравця з GameManager
        GameObject playerObj = GameManager.Instance.visualPlayer;

        // 2. Телепортуємо його в точку спавну арени
        playerObj.transform.position = playerSpawnPoint.position;

        // 3. Додаємо/Активуємо бойову логіку
        // Наприклад, якщо у тебе рух на мапі і в бою різний:
        // playerObj.GetComponent<MapMovement>().enabled = false;
        // playerObj.GetComponent<ArenaMovement>().enabled = true;

        Debug.Log("<color=green>[Battle]</color> Гравця перенесено на арену.");

        SpawnTrainingDummy();
    }

    private void SpawnTrainingDummy()
    {
        if (dummyPrefab != null)
        {
            GameObject dummy = Instantiate(dummyPrefab, playerSpawnPoint.position + new Vector3(3, 0, 0), Quaternion.identity);
            dummy.transform.SetParent(arenaContainer.transform);
            dummy.name = "Test_Dummy";
        }
    }

    public void EndBattle()
    {
        arenaContainer.SetActive(false);
        // Повертаємо гравця в логіку мапи світу (код додамо пізніше)
    }
}