using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using map_test;

public class MapRenderer : MonoBehaviour
{
    public Tilemap tilemap;
    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    // Твої константи з Pygame
    private const int ViewWidth = 23;
    private const int ViewHeight = 19;
    private const int OffX = 11;
    private const int OffY = 9;

    public void UpdateView(WorldMap map, int playerX, int playerY)
    {
        if (map == null || map.WorldMapMatrix == null) return;

        tilemap.ClearAllTiles(); // Очищаємо старе вікно

        for (int i = 0; i < ViewWidth; i++)
        {
            for (int b = 0; b < ViewHeight; b++)
            {
                // Логіка зміщення як у твоєму Pygame проєкті
                int mx = playerX - OffX + i;
                int my = playerY - OffY + b;

                if (mx >= 0 && mx < map.Width && my >= 0 && my < map.Height)
                {
                    WordMapTile tileData = map.WorldMapMatrix[mx][my];

                    // Завантажуємо спрайт, якщо він ще не в пам'яті
                    if (tileData._unitySprite == null)
                    {
                        tileData._unitySprite = GetSprite(tileData._texture);
                    }

                    if (tileData._unitySprite != null)
                    {
                        Tile unityTile = ScriptableObject.CreateInstance<Tile>();
                        unityTile.sprite = tileData._unitySprite;

                        // Малюємо тайл. Координати (i, b) змусять мапу 
                        // завжди бути центрованою відносно камери
                        tilemap.SetTile(new Vector3Int(i, b, 0), unityTile);
                    }
                }
            }
        }
    }

    private Sprite GetSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        string cleanPath = path.Replace(".png", "").Replace(".jpg", "");
        if (cleanPath.StartsWith("Assets/Resources/"))
            cleanPath = cleanPath.Replace("Assets/Resources/", "");

        if (_spriteCache.ContainsKey(cleanPath)) return _spriteCache[cleanPath];

        Sprite s = Resources.Load<Sprite>(cleanPath);
        if (s != null) _spriteCache[cleanPath] = s;
        return s;
    }
}