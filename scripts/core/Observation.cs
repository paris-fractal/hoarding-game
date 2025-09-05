using System;

namespace hoardinggame.Core
{
    public abstract class GameObservation
    {
        public float Timestamp { get; set; }
    }

    public class DoorApertureOccupiedObservation : GameObservation
    {
        public string DoorId { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
    }

    public class BodyCameToRestObservation : GameObservation
    {
        public string BodyId { get; set; } = string.Empty;
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }

    public class PlayerReachedPositionObservation : GameObservation
    {
        public int RoomId { get; set; }
        public int FacingDirection { get; set; } // 0=N, 1=E, 2=S, 3=W
    }

    public class ItemPickupCompletedObservation : GameObservation
    {
        public string ItemId { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class SanityTriggerObservation : GameObservation
    {
        public string TriggerType { get; set; } = string.Empty;
        public float SanityDelta { get; set; }
    }
}