using System;

namespace hoardinggame.Core
{
    public abstract class GameInput
    {
        public float Timestamp { get; set; }
    }

    public class OpenDoorInput : GameInput
    {
        public string DoorId { get; set; } = string.Empty;
    }

    public class PickItemInput : GameInput
    {
        public string ItemId { get; set; } = string.Empty;
        public int BagX { get; set; }
        public int BagY { get; set; }
        public bool Rotated { get; set; } = false;
    }

    public class MovePlayerInput : GameInput
    {
        public enum Direction { Forward, TurnLeft, TurnRight }
        public Direction MoveDirection { get; set; }
    }

    public class SellItemInput : GameInput
    {
        public string ItemId { get; set; } = string.Empty;
    }

    public class BuyUpgradeInput : GameInput
    {
        public string UpgradeId { get; set; } = string.Empty;
    }

    public class RotatePlayerInput : GameInput
    {
        public enum RotationDirection { Left, Right }
        public RotationDirection Direction { get; set; }
    }
}