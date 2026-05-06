using System;
using UnityEngine; // Потрібно для Sprite

namespace map_test
{
    [Serializable]
    public class WordMapTile
    {
        public int _id { get; set; }
        public string _texture { get; set; } // Тут зберігається шлях, наприклад "image/tiles/grass"

        [Newtonsoft.Json.JsonIgnore]
        [NonSerialized]
        public Sprite _unitySprite; // Сюди завантажимо саму картинку для Unity

        public WordMapTile() { }

        public void Load_map_tile(int id, string texture)
        {
            _id = id;
            _texture = texture;
        }

        public int Return_id() { return _id; }
        public string Return_texture() { return _texture; }
    }
}