using Godot;
using hoardinggame.Core;
using hoardinggame;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public partial class JunkSystem : Node3D
{
	private Dictionary<string, Node3D> realizedJunkObjects = new();
	private Node3D junkPlane;

	public override void _Process(double dt)
	{
		var currentGameState = Orchestrator.GetCurrentState();
		if (currentGameState == null)
		{
			GD.Print("No current game state");
			return;
		}

		ProcessJunkRealization(currentGameState);
	}

	private void ProcessJunkRealization(GameState gameState)
	{
		// Remove junk objects that no longer exist in GameState
		RemoveStaleJunkObjects(gameState);

		// Create junk objects for new items in GameState
		CreateNewJunkObjects(gameState);
	}

	private void RemoveStaleJunkObjects(GameState gameState)
	{
		var currentJunkIds = gameState.JunkItems.Select(j => j.Id).ToHashSet();
		var toRemove = realizedJunkObjects.Keys.Where(id => !currentJunkIds.Contains(id)).ToList();

		foreach (var junkId in toRemove)
		{
			var junkObject = realizedJunkObjects[junkId];
			junkObject.QueueFree();
			realizedJunkObjects.Remove(junkId);
		}
	}

	private void CreateNewJunkObjects(GameState gameState)
	{
		junkPlane ??= GetNodeOrNull<Node3D>("/root/Root/room/JunkPlane");
		foreach (var junk in gameState.JunkItems)
		{
			if (!realizedJunkObjects.ContainsKey(junk.Id))
			{
				var junkObject = CreateJunkObject(junk);
				if (junkObject != null)
				{
					var room = GetNode<Node3D>("/root/Root/room");
					var parent = junkPlane ?? room;
					parent.AddChild(junkObject);
					realizedJunkObjects[junk.Id] = junkObject;
					junkObject.GlobalPosition = new Vector3(junk.PosX, junk.PosY, junk.PosZ);
					GD.Print($"Added {junk.Id} to plane, now holding: {junkPlane.GetChildCount()}");
				}
				else
				{
					GD.Print($"Failed to create junk object for {junk.Id}");
				}
			}
		}
	}

	private Node3D CreateJunkObject(hoardinggame.Core.JunkItem junk)
	{
		var scenePath = GetJunkScenePath(junk.type);
		if (scenePath == null)
		{
			GD.PrintErr($"Unknown junk type: {junk.type}");
			return null;
		}

		var scene = GD.Load<PackedScene>(scenePath);
		if (scene == null)
		{
			GD.PrintErr($"Failed to load scene: {scenePath}");
			return null;
		}

		var junkObject = scene.Instantiate() as Node3D;

		// Set position and rotation
		junkObject.Name = junk.type + "_" + junk.Id;
		junkObject.GlobalPosition = new Vector3(junk.PosX, junk.PosY, junk.PosZ);
		junkObject.GlobalRotationDegrees = new Vector3(junk.RotX, junk.RotY, junk.RotZ);

		// Set the GameState ID in the Junk component if it exists
		var junkComponent = junkObject as Junk;
		if (junkComponent != null)
		{
			junkComponent.GameStateId = junk.Id;
		}

		return junkObject;
	}

	private string GetJunkScenePath(string junkType)
	{
		return junkType switch
		{
			"junk_can" => "res://objects/junk/junk_can.tscn",
			_ => null
		};
	}
}
