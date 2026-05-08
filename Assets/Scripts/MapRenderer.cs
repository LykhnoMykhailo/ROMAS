using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using map_test;

public class MapRenderer : MonoBehaviour
{
    [Header("Шари Tilemap")]
    public Tilemap backgroundTilemap;
    public Tilemap locationsTilemap;

    private const int ViewWidth = 23;
    private const int ViewHeight = 19;
    private const int OffX = 11;
    private const int OffY = 9;

    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

    public void UpdateView(WorldMap map, int playerX, int playerY)
    {
        if (map == null || map.WorldMapMatrix == null) return;

        // Очищення перед малюванням
        if (backgroundTilemap != null) backgroundTilemap.ClearAllTiles();
        if (locationsTilemap != null) locationsTilemap.ClearAllTiles();

        for (int i = 0; i < ViewWidth; i++)
        {
            for (int b = 0; b < ViewHeight; b++)
            {
                int mx = playerX - OffX + i;
                int my = playerY - OffY + b;

                if (mx >= 0 && mx < map.Width && my >= 0 && my < map.Height)
                {
                    // 1. Ландшафт
                    WordMapTile tileData = map.WorldMapMatrix[mx][my];
                    if (tileData != null) DrawTile(backgroundTilemap, tileData._texture, i, b);

                    // 2. Локації
                    string key = $"{mx},{my}";
                    if (map.Locations != null && map.Locations.ContainsKey(key))
                    {
                        var loc = map.Locations[key];
                        if (loc != null) DrawTile(locationsTilemap, loc.Texture, i, b);
                    }
                }
            }
        }
    }

    private void DrawTile(Tilemap tm, string path, int x, int y)
    {
        if (string.IsNullOrEmpty(path)) return;

        Sprite s = GetSprite(path);
        if (s != null)
        {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = s;
            tm.SetTile(new Vector3Int(x, y, 0), t);
        }
    }

    private Sprite GetSprite(string path)
    {
        // Очищення шляху для Unity Resources
        string cleanPath = path.Replace(".png", "").Replace(".jpg", "");
        if (cleanPath.Contains("Resources/"))
            cleanPath = cleanPath.Substring(cleanPath.IndexOf("Resources/") + 10);

        if (_spriteCache.ContainsKey(cleanPath)) return _spriteCache[cleanPath];

        Sprite s = Resources.Load<Sprite>(cleanPath);
        if (s != null) _spriteCache[cleanPath] = s;
        else Debug.LogWarning($"[MapRenderer] Не знайдено спрайт: {cleanPath}");

        return s;
    }
}