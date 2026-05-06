using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileLoader : MonoBehaviour
{
    // Кеш для зберігання вже завантажених тайлів
    private Dictionary<string, Tile> _tileCache = new Dictionary<string, Tile>();

    public Tile GetTileByPath(string path)
    {
        // 1. Прибираємо розширення файлу (Resources.Load не любить .png)
        string cleanPath = path.Replace(".png", "").Replace(".jpg", "");

        // 2. Перевіряємо, чи ми вже завантажували це
        if (_tileCache.ContainsKey(cleanPath))
        {
            return _tileCache[cleanPath];
        }

        // 3. Завантажуємо спрайт із папки Resources
        Sprite sprite = Resources.Load<Sprite>(cleanPath);

        if (sprite == null)
        {
            Debug.LogError($"Не вдалося знайти зображення за шляхом: Resources/{cleanPath}");
            return null;
        }

        // 4. Створюємо об'єкт Tile для Unity Tilemap
        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = sprite;

        // Додаємо в кеш і повертаємо
        _tileCache[cleanPath] = newTile;
        return newTile;
    }
}