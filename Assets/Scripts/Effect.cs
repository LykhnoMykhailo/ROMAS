using System;
using UnityEngine;
using Newtonsoft.Json;

namespace GameCore.Entities
{
    /// <summary>
    /// Клас ефектів згідно з діаграмою image_76005a.png.
    /// </summary>
    [Serializable]
    public class Effect
    {
        [Header("Основні дані")]
        [JsonProperty("name")]
        public string name;

        [JsonProperty("time")]
        public float time;

        [JsonProperty("id")]
        public string id;

        [Header("Гнучкі дані")]
        [JsonProperty("type")]
        public string type;

        public Effect() { }

        public Effect(string name, float time, string id, string type)
        {
            this.name = name;
            this.time = time;
            this.id = id;
            this.type = type;
        }

        public Effect Clone()
        {
            return new Effect(this.name, this.time, this.id, this.type);
        }

        public virtual string apply(Puppet target)
        {
            return $"Ефект {name} діє на сутність. Залишилось: {time}";
        }
    }
}