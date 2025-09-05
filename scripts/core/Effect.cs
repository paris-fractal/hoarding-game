using System;

namespace hoardinggame.Core
{
    public abstract class GameEffect
    {
        public double Timestamp { get; set; }
    }

    public class PlayAnimEffect : GameEffect
    {
        public string NodeId { get; set; } = string.Empty;
        public string AnimationName { get; set; } = string.Empty;
        public float Duration { get; set; }
    }

    public class SfxEffect : GameEffect
    {
        public string SoundId { get; set; } = string.Empty;
        public float Volume { get; set; } = 1.0f;
        public float Pitch { get; set; } = 1.0f;
    }

    public class SpawnLooseEffect : GameEffect
    {
        public string ItemId { get; set; } = string.Empty;
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public bool IsDynamic { get; set; } = true;
    }

    public class DespawnEffect : GameEffect
    {
        public string ItemId { get; set; } = string.Empty;
    }

    public class TransitionEffect : GameEffect
    {
        public int ToRoomId { get; set; }
        public int FacingDirection { get; set; }
        public float Duration { get; set; } = 0.5f;
    }

    public class UpdateUIEffect : GameEffect
    {
        public string UIElement { get; set; } = string.Empty;
        public object Data { get; set; } = new();
    }

    public class ApplyImpulseEffect : GameEffect
    {
        public string BodyId { get; set; } = string.Empty;
        public float ForceX { get; set; }
        public float ForceY { get; set; }
        public float ForceZ { get; set; }
    }
}