using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using hoardinggame;
using static GdUnit4.Assertions;
using Godot;

namespace hoardinggame.Core.Tests
{
    [RequireGodotRuntime]
    [TestSuite]
    public class JunkRealizationSystemTests
    {
        private ISceneRunner sceneRunner;
        private JunkSystem junkSystem;

        [Before]
        public void SetUp()
        {
            sceneRunner = ISceneRunner.Load("res://scene/main.tscn");

            // Get the JunkSystem from the scene
            junkSystem = sceneRunner.Scene().GetNode<JunkSystem>("Global/JunkSystem");
        }

        [After]
        public void TearDown()
        {
            sceneRunner?.Dispose();
        }

        [TestCase]
        public void JunkSystemExists_InScene()
        {
            AssertThat(junkSystem).IsNotNull();
        }

        [TestCase]
        public void JunkSystem_CanAccessOrchestrator()
        {
            // Process one frame to ensure JunkSystem runs
            sceneRunner.SimulateFrames(1);

            // If we get here without crashing, the JunkSystem can access the Orchestrator
            AssertThat(true).IsTrue();
        }

        [TestCase]
        public void JunkSystem_DoesNotCrashWithEmptyGameState()
        {
            // The default GameState should be empty
            sceneRunner.SimulateFrames(5);

            // If we get here without crashing, the system handles empty state
            AssertThat(true).IsTrue();
        }

        [TestCase]
        public void RoomNode_Exists()
        {
            var room = sceneRunner.Scene().GetNode<Node3D>("room");
            AssertThat(room).IsNotNull();

            // Check that room has some children (static junk_can)
            var children = room.GetChildren();
            AssertThat(children.Count).IsGreater(0);
        }

        [TestCase]
        public void JunkSystem_CreatesJunkObjectWhenAddedToGameState()
        {
            // Arrange - Get the orchestrator and room
            var orchestrator = sceneRunner.Scene().GetNode<Orchestrator>("Global/Orchestrator");
            var room = sceneRunner.Scene().GetNode<Node3D>("room");
            var initialChildCount = room.GetChildren().Count;

            // Create a junk item in the GameState
            var gameState = Orchestrator.GetCurrentState();
            gameState.JunkItems.Add(new hoardinggame.Core.JunkItem(
                Id: "test-junk-1",
                type: "junk_can",
                PosX: 1.0f,
                PosY: 0.0f,
                PosZ: 2.0f,
                RotX: 0.0f,
                RotY: 45.0f,
                RotZ: 0.0f
            ));

            // Act - Process frames to let JunkSystem realize the junk
            sceneRunner.SimulateFrames(2);

            // Assert - Check that a new object was added to the room
            var newChildCount = room.GetChildren().Count;
            AssertThat(newChildCount).IsEqual(initialChildCount + 1);

            // Find the newly created junk object by searching through all children
            Junk newJunkComponent = null;
            Node3D newJunkObject = null;
            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var junkComponent = node3D.GetNodeOrNull<Junk>(".");
                    if (junkComponent != null && junkComponent.GameStateId == "test-junk-1")
                    {
                        newJunkComponent = junkComponent;
                        newJunkObject = node3D;
                        break;
                    }
                }
            }

            AssertThat(newJunkComponent).IsNotNull();
            AssertThat(newJunkObject).IsNotNull();

            // Verify position and rotation
            AssertThat(newJunkObject.Position.X).IsEqualApprox(1.0f, 0.01f);
            AssertThat(newJunkObject.Position.Y).IsEqualApprox(0.0f, 0.01f);
            AssertThat(newJunkObject.Position.Z).IsEqualApprox(2.0f, 0.01f);
            AssertThat(newJunkObject.RotationDegrees.Y).IsEqualApprox(45.0f, 0.01f);
        }

        [TestCase]
        public void JunkSystem_RemovesJunkObjectWhenRemovedFromGameState()
        {
            // Arrange - Create initial junk object
            var gameState = Orchestrator.GetCurrentState();
            gameState.JunkItems.Add(new hoardinggame.Core.JunkItem(
                Id: "test-junk-remove",
                type: "junk_can",
                PosX: 3.0f,
                PosY: 0.0f,
                PosZ: 1.0f,
                RotX: 0.0f,
                RotY: 0.0f,
                RotZ: 0.0f
            ));

            // Process frames to realize the junk
            sceneRunner.SimulateFrames(2);

            var room = sceneRunner.Scene().GetNode<Node3D>("room");
            var initialChildCount = room.GetChildren().Count;

            // Verify the object was created
            Junk testJunkComponent = null;
            Node3D testJunkObject = null;
            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var junkComponent = node3D.GetNodeOrNull<Junk>(".");
                    if (junkComponent != null && junkComponent.GameStateId == "test-junk-remove")
                    {
                        testJunkComponent = junkComponent;
                        testJunkObject = node3D;
                        break;
                    }
                }
            }
            AssertThat(testJunkComponent).IsNotNull();
            AssertThat(testJunkObject).IsNotNull();

            // Act - Remove the junk from GameState
            gameState.JunkItems.RemoveAll(j => j.Id == "test-junk-remove");

            // Process frames to let JunkSystem remove the object
            sceneRunner.SimulateFrames(2);

            // Assert - Check that the object was removed
            var newChildCount = room.GetChildren().Count;
            AssertThat(newChildCount).IsEqual(initialChildCount - 1);

            // Verify the specific object is gone
            Junk removedJunkComponent = null;
            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var junkComponent = node3D.GetNodeOrNull<Junk>(".");
                    if (junkComponent != null && junkComponent.GameStateId == "test-junk-remove")
                    {
                        removedJunkComponent = junkComponent;
                        break;
                    }
                }
            }
            AssertThat(removedJunkComponent).IsNull();
        }

        [TestCase]
        public void JunkSystem_ManagesMultipleJunkObjects()
        {
            // Arrange - Create multiple junk objects
            var gameState = Orchestrator.GetCurrentState();
            gameState.JunkItems.Clear(); // Start fresh

            gameState.JunkItems.Add(new JunkItem(
                Id: "multi-junk-1",
                type: "junk_can",
                PosX: 0.0f,
                PosY: 0.0f,
                PosZ: 0.0f,
                RotX: 0.0f,
                RotY: 0.0f,
                RotZ: 0.0f
            ));

            gameState.JunkItems.Add(new JunkItem(
                Id: "multi-junk-2",
                type: "junk_can",
                PosX: 5.0f,
                PosY: 1.0f,
                PosZ: -2.0f,
                RotX: 0.0f,
                RotY: 90.0f,
                RotZ: 0.0f
            ));

            // Act - Process frames
            sceneRunner.SimulateFrames(2);

            // Assert - Both objects should exist
            var room = sceneRunner.Scene().GetNode<Node3D>("room");

            Junk junk1Component = null;
            Junk junk2Component = null;
            Node3D junk1 = null;
            Node3D junk2 = null;

            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var junkComponent = node3D.GetNodeOrNull<Junk>(".");
                    if (junkComponent != null)
                    {
                        if (junkComponent.GameStateId == "multi-junk-1")
                        {
                            junk1Component = junkComponent;
                            junk1 = node3D;
                        }
                        else if (junkComponent.GameStateId == "multi-junk-2")
                        {
                            junk2Component = junkComponent;
                            junk2 = node3D;
                        }
                    }
                }
            }

            AssertThat(junk1Component).IsNotNull();
            AssertThat(junk2Component).IsNotNull();
            AssertThat(junk1).IsNotNull();
            AssertThat(junk2).IsNotNull();

            // Verify positions
            AssertThat(junk1.Position.X).IsEqualApprox(0.0f, 0.01f);
            AssertThat(junk2.Position.X).IsEqualApprox(5.0f, 0.01f);
            AssertThat(junk2.Position.Y).IsEqualApprox(1.0f, 0.01f);
            AssertThat(junk2.RotationDegrees.Y).IsEqualApprox(90.0f, 0.01f);

            // Now remove one and verify selective removal
            gameState.JunkItems.RemoveAll(j => j.Id == "multi-junk-1");
            sceneRunner.SimulateFrames(2);

            Junk remainingJunk1Component = null;
            Junk remainingJunk2Component = null;

            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var junkComponent = node3D.GetNodeOrNull<Junk>(".");
                    if (junkComponent != null)
                    {
                        if (junkComponent.GameStateId == "multi-junk-1")
                        {
                            remainingJunk1Component = junkComponent;
                        }
                        else if (junkComponent.GameStateId == "multi-junk-2")
                        {
                            remainingJunk2Component = junkComponent;
                        }
                    }
                }
            }

            AssertThat(remainingJunk1Component).IsNull();
            AssertThat(remainingJunk2Component).IsNotNull();
        }

        [TestCase]
        public void JunkSystem_HandlesUnknownJunkType()
        {
            // Arrange
            var gameState = Orchestrator.GetCurrentState();
            var room = sceneRunner.Scene().GetNode<Node3D>("room");
            var initialChildCount = room.GetChildren().Count;

            gameState.JunkItems.Add(new hoardinggame.Core.JunkItem(
                Id: "unknown-junk",
                type: "unknown_type",
                PosX: 0.0f,
                PosY: 0.0f,
                PosZ: 0.0f,
                RotX: 0.0f,
                RotY: 0.0f,
                RotZ: 0.0f
            ));

            // Act - Process frames
            sceneRunner.SimulateFrames(2);

            // Assert - No new object should be created for unknown type
            var newChildCount = room.GetChildren().Count;
            AssertThat(newChildCount).IsEqual(initialChildCount);
        }

        [TestCase]
        public void JunkSystem_UpdatesPositionWhenJunkModified()
        {
            // Arrange - Create initial junk
            var gameState = Orchestrator.GetCurrentState();
            gameState.JunkItems.Clear();

            gameState.JunkItems.Add(new hoardinggame.Core.JunkItem(
                Id: "position-test-junk",
                type: "junk_can",
                PosX: 0.0f,
                PosY: 0.0f,
                PosZ: 0.0f,
                RotX: 0.0f,
                RotY: 0.0f,
                RotZ: 0.0f
            ));

            sceneRunner.SimulateFrames(2);

            // Verify initial position
            var room = sceneRunner.Scene().GetNode<Node3D>("room");

            Junk junkComponent = null;
            Node3D junkObject = null;
            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var component = node3D.GetNodeOrNull<Junk>(".");
                    if (component != null && component.GameStateId == "position-test-junk")
                    {
                        junkComponent = component;
                        junkObject = node3D;
                        break;
                    }
                }
            }

            AssertThat(junkObject.Position.X).IsEqualApprox(0.0f, 0.01f);

            // Act - Modify the junk position in GameState
            gameState.JunkItems.Clear();
            gameState.JunkItems.Add(new hoardinggame.Core.JunkItem(
                Id: "position-test-junk",
                type: "junk_can",
                PosX: 10.0f,
                PosY: 5.0f,
                PosZ: -3.0f,
                RotX: 0.0f,
                RotY: 180.0f,
                RotZ: 0.0f
            ));

            sceneRunner.SimulateFrames(2);

            // Assert - The object should be recreated at the new position
            Junk updatedJunkComponent = null;
            Node3D updatedJunkObject = null;
            foreach (Node child in room.GetChildren())
            {
                if (child is Node3D node3D)
                {
                    var component = node3D.GetNodeOrNull<Junk>(".");
                    if (component != null && component.GameStateId == "position-test-junk")
                    {
                        updatedJunkComponent = component;
                        updatedJunkObject = node3D;
                        break;
                    }
                }
            }

            AssertThat(updatedJunkObject.Position.X).IsEqualApprox(10.0f, 0.01f);
            AssertThat(updatedJunkObject.Position.Y).IsEqualApprox(5.0f, 0.01f);
            AssertThat(updatedJunkObject.Position.Z).IsEqualApprox(-3.0f, 0.01f);
            AssertThat(updatedJunkObject.RotationDegrees.Y).IsEqualApprox(180.0f, 0.01f);
        }
    }
}