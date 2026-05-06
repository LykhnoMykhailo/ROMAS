using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// namespace map_test // Видаляємо для Unity

[System.Serializable]
public class TileVariantData
{
    public string Id { get; set; }
    public string BaseDir { get; set; }
    public List<string> Variants { get; set; }

    public TileVariantData()
    {
        Variants = new List<string>();
    }

    /// <summary>
    /// Отримання повного шляху до текстури.
    /// Використовуємо System.Random, щоб уникнути конфліктів з UnityEngine.Random.
    /// </summary>
    public string GetRandomPath(System.Random rnd)
    {
        if (Variants == null || Variants.Count == 0)
        {
            return Path.Combine(BaseDir ?? "", "none_texture.png");
        }

        int index = rnd.Next(Variants.Count);
        // В Unity для текстур зазвичай використовуємо "/" як роздільник
        return Path.Combine(BaseDir ?? "", Variants[index]).Replace("\\", "/");
    }
}

[System.Serializable]
public class TilesetConfig
{
    public List<TileVariantData> Tilesets { get; set; }

    public TilesetConfig()
    {
        Tilesets = new List<TileVariantData>();
    }
}