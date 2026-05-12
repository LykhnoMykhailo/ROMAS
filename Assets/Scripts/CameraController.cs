using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform gridTransform;
    public Transform playerTransform; // Додаємо посилання на трансформ гравця

    private const float ViewWidth = 23f;
    private const int ViewHeight = 19;

    [Header("Зміщення для мапи світу")]
    public float worldMapHorizontalOffset = 5f;
    public void SetupCamera()
    {
        // Викликаємо нову логіку позиціонування мапи
        SetWorldMapPosition();
    }
    void LateUpdate()
    {
        if (GameManager.Instance == null) return;

        // Вибираємо режим камери залежно від стану гри
        if (GameManager.Instance.currentState == GameState.Battle)
        {
            FollowPlayerSmoothly();
        }
        else if (GameManager.Instance.currentState == GameState.WorldMap)
        {
            SetWorldMapPosition();
        }
    }

    // Режим 1: Камера завжди в центрі гравця (для арен)
    private void FollowPlayerSmoothly()
    {
        if (playerTransform == null) return;

        Vector3 targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
        mainCamera.transform.position = targetPos;
        mainCamera.orthographicSize = 7f; // Можна зробити камеру ближче для бою
    }

    // Режим 2: Твоя стара логіка для мапи світу
    public void SetWorldMapPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float centerX = gridTransform.position.x + (ViewWidth / 2f);
        float centerY = gridTransform.position.y + (ViewHeight / 2f);

        Vector3 newPosition = new Vector3(centerX + worldMapHorizontalOffset, centerY, -10f);
        mainCamera.transform.position = newPosition;

        mainCamera.orthographic = true;
        mainCamera.orthographicSize = ViewHeight / 2f;
    }
}