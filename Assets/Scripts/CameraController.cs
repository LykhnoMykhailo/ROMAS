using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Transform gridTransform; // Сюди перетягнемо твій Grid (50, 50)

    private const float ViewWidth = 23f;
    private const int ViewHeight = 19;

    public void SetupCamera()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // 1. Обчислюємо центр вікна рендерингу
        // Ми малюємо 23 на 19 тайлів. Центр — це 11.5 та 9.5 відносно Grid
        float centerX = gridTransform.position.x + (ViewWidth / 2f);
        float centerY = gridTransform.position.y + (ViewHeight / 2f);

        // 2. Переносимо камеру (Z залишаємо -10, щоб бачити 2D)
        mainCamera.transform.position = new Vector3(centerX, centerY, -10f);

        // 3. Налаштовуємо розмір камери (Orthographic Size)
        // В Unity Size — це половина вертикального розміру екрана.
        // Наша висота 19, отже 19 / 2 = 9.5
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = ViewHeight / 2f;

        Debug.Log($"[Camera] Налаштовано на центр: {centerX}, {centerY}. Розмір: {mainCamera.orthographicSize}");
    }
}