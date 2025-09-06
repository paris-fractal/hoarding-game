using Godot;
using System;

public partial class SanityMeter : ProgressBar
{
	public override void _Process(double delta)
	{
		this.Value = Orchestrator.GetCurrentState().SanityLevel;
	}
}
