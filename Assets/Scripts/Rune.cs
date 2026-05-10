using System;
namespace GameCore.Entities
{
    [Serializable]
    public class Rune : Item
    {
        public int Id;
        public string classes; // "Fire", "Water"
        public int level;      // Π³βενό (L)
        public string texture;
    }
}