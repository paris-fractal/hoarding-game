using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

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
        public List<Activity> Activities { get; set; } = new();

        public string ToJson()
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                IncludeFields = true
            };
            options.Converters.Add(new ActivityJsonConverter());
            return JsonSerializer.Serialize(this, options);
        }

        public static GameState FromJson(string json)
        {
            var options = new JsonSerializerOptions 
            { 
                IncludeFields = true
            };
            options.Converters.Add(new ActivityJsonConverter());
            return JsonSerializer.Deserialize<GameState>(json, options) ?? new GameState();
        }

        public GameState Clone()
        {
            var cloned = new GameState
            {
                Time = Time,
                PlayerRotation = PlayerRotation,
                SanityLevel = SanityLevel,
                Money = Money,
                Inventory = new Dictionary<string, object>(Inventory),
                Activities = Activities.ToList()
            };
            return cloned;
        }
    }
}