using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace map_test
{
    public class WorldHelperClasses
    {
    }
    public class LocationDataRoot
    {
        public List<AgressivePreset> AgressivePresets { get; set; }
        public List<NetralPreset> NetralPresets { get; set; }
    }

    // Шаблон для агресивних локацій (данжів)
    public class AgressivePreset
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Texture { get; set; }
        public string Clan { get; set; }
        public bool Boss { get; set; }
        public List<string> PossibleRooms { get; set; }
        public Dictionary<string, EnemyRange> EnemyPool { get; set; }
    }

    // Шаблон для нейтральних локацій (міста/кузні)
    public class NetralPreset
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Texture { get; set; }
        public List<string> Functions { get; set; }
    }

    // Допоміжний клас для діапазону ворогів
    public class EnemyRange
    {
        public int min_per_room { get; set; }
        public int max_per_room { get; set; }
    }
}
