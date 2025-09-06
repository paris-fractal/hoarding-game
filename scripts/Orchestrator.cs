using hoardinggame.Core;
using Godot;
using System;
using System.Collections.Generic;

public partial class Orchestrator : Node
{
	public static Orchestrator Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	private readonly Queue<GameInput> inbox = new();
	private GameState state = new();

	private GameEngine engine = new();

	public static void Enqueue(GameInput input)
	{
		Instance.inbox.Enqueue(input);
	}

	public static GameState GetCurrentState()
	{
		return Instance.state;
	}

	public static void SetInstance(Orchestrator instance)
	{
		Instance = instance;
	}

	public override void _PhysicsProcess(double dt)
	{
		// 1) drain + coalesce deterministically
		var inputs = DrainInbox();
		// TODO fetch observations from the world
		var observations = new List<GameObservation>();

		// 2) step core
		var result = engine.Step(state, inputs, observations, dt);
		state = result.NewState;

		// 3) execute all effects
		foreach (var effect in result.Effects)
		{
			effect.Execute();
		}
	}

	private List<GameInput> DrainInbox()
	{
		var list = new List<GameInput>(inbox.Count);
		while (inbox.Count > 0) list.Add(inbox.Dequeue());
		return list;
	}
}
