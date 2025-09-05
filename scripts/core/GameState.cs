using System;
using System.Text.Json;
using System.Collections.Generic;

namespace hoardinggame.Core
{
    [Serializable]
    public class GameState
    {
        public double Time { get; set; } = 0f;

        public float PlayerRotation { get; set; } = 0f;
        public float SanityLevel { get; set; } = 100f;
        public int Money { get; set; } = 0;
        public Dictionary<string, object> Inventory { get; set; } = new();

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        public static GameState FromJson(string json)
        {
            return JsonSerializer.Deserialize<GameState>(json) ?? new GameState();
        }

        public GameState Clone()
        {
            return FromJson(ToJson());
        }
    }
}