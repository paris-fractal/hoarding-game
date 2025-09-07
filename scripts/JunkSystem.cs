using Godot;

public partial class JunkSystem : Node3D
{
    public override void _Process(double dt)
    {
        var room = GetNode<Node3D>("Root/room");
        var loaded = GD.Load<PackedScene>("res://objects/junk/junk_can.tscn");
        room.AddChild(loaded.Instantiate<Node3D>());
    }
}