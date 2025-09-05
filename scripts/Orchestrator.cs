using hoardinggame.Core;
using Godot;
using System;
using System.Collections.Generic;

public partial class Orchestrator : Node
{
    private readonly Queue<GameInput> inbox = new();
    private GameState state = new();

    private GameEngine engine = new();

    public void Enqueue(GameInput input)
    {
        inbox.Enqueue(input);
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

        // TODO 3) apply effects
        // fx.Apply(effects, this);    // spawns, anims, impulses, audio, locks, transitions
    }

    private List<GameInput> DrainInbox()
    {
        var list = new List<GameInput>(inbox.Count);
        while (inbox.Count > 0) list.Add(inbox.Dequeue());
        return list;
    }
}