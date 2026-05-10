using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform gridTransform;

    private const float ViewWidth = 23f;
    private const int ViewHeight = 19;

    [Header("Зміщення")]
    [Tooltip("Чим більше значення, тим лівіше буде мапа на екрані")]
    public float horizontalOffset = 5f;

    public void SetupCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Обчислюємо реальний центр мапи
        float centerX = gridTransform.position.x + (ViewWidth / 2f);
        float centerY = gridTransform.position.y + (ViewHeight / 2f);

        // Додаємо зміщення до X, щоб камера дивилася правіше від центру мапи
        // В результаті мапа опиниться в лівій частині екрана
        Vector3 newPosition = new Vector3(centerX + horizontalOffset, centerY, -10f);

        mainCamera.transform.position = newPosition;

        mainCamera.orthographic = true;
        mainCamera.orthographicSize = ViewHeight / 2f;

        Debug.Log($"[Camera] Мапу зміщено вліво. Позиція камери: {newPosition}");
    }
}