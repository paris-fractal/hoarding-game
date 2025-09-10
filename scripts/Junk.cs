using Godot;

namespace hoardinggame
{
    public partial class Junk : Node3D
    {
        public string GameStateId { get; set; } = "";

        public override void _Process(double dt)
        {
            // must always face camera logic, etc
        }
    }
}