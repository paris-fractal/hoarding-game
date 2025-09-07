using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    [TestSuite]
    public class JunkSystemTests
    {
        [TestCase]
        public void InitialJunkListIsEmpty()
        {
            var state = new GameState();
            AssertThat(state.JunkItems).IsNotNull();
            AssertThat(state.JunkItems).IsEmpty();
        }

        [TestCase]
        public void CanAddJunkToGameState()
        {
            var state = new GameState();
            var junk = new Junk("test-junk-1", "junk_can", 1.0f, 2.0f, 3.0f, 45.0f, 90.0f, 180.0f);

            state.JunkItems.Add(junk);

            AssertThat(state.JunkItems).HasSize(1);
            AssertThat(state.JunkItems[0].Id).IsEqual("test-junk-1");
            AssertThat(state.JunkItems[0].PosX).IsEqual(1.0f);
            AssertThat(state.JunkItems[0].PosY).IsEqual(2.0f);
            AssertThat(state.JunkItems[0].PosZ).IsEqual(3.0f);
            AssertThat(state.JunkItems[0].RotX).IsEqual(45.0f);
            AssertThat(state.JunkItems[0].RotY).IsEqual(90.0f);
            AssertThat(state.JunkItems[0].RotZ).IsEqual(180.0f);
        }

        [TestCase]
        public void CanRemoveJunkFromGameState()
        {
            var state = new GameState();
            var junk1 = new Junk("junk-1", "junk_can", 0f, 0f, 0f, 0f, 0f, 0f);
            var junk2 = new Junk("junk-2", "junk_can", 1f, 1f, 1f, 0f, 0f, 0f);

            state.JunkItems.Add(junk1);
            state.JunkItems.Add(junk2);

            state.JunkItems.RemoveAll(j => j.Id == "junk-1");

            AssertThat(state.JunkItems).HasSize(1);
            AssertThat(state.JunkItems[0].Id).IsEqual("junk-2");
        }

        [TestCase]
        public void JunkRecordHasCorrectProperties()
        {
            var junk = new Junk("test-id", "junk_can", 1.5f, 2.5f, 3.5f, 10.0f, 20.0f, 30.0f);

            AssertThat(junk.Id).IsEqual("test-id");
            AssertThat(junk.PosX).IsEqual(1.5f);
            AssertThat(junk.PosY).IsEqual(2.5f);
            AssertThat(junk.PosZ).IsEqual(3.5f);
            AssertThat(junk.RotX).IsEqual(10.0f);
            AssertThat(junk.RotY).IsEqual(20.0f);
            AssertThat(junk.RotZ).IsEqual(30.0f);
        }

        [TestCase]
        public void StateClonePreservesJunkItems()
        {
            var originalState = new GameState();
            var junk1 = new Junk("junk-1", "junk_can", 1f, 2f, 3f, 4f, 5f, 6f);
            var junk2 = new Junk("junk-2", "junk_can", 7f, 8f, 9f, 10f, 11f, 12f);

            originalState.JunkItems.Add(junk1);
            originalState.JunkItems.Add(junk2);

            var clonedState = originalState.Clone();

            AssertThat(clonedState.JunkItems).HasSize(2);
            AssertThat(clonedState.JunkItems).IsNotSame(originalState.JunkItems);
            AssertThat(clonedState.JunkItems[0].Id).IsEqual("junk-1");
            AssertThat(clonedState.JunkItems[1].Id).IsEqual("junk-2");
        }

        [TestCase]
        public void StateJsonSerializationPreservesJunkItems()
        {
            var originalState = new GameState();
            var junk = new Junk("serialized-junk", "junk_can", 10f, 20f, 30f, 45f, 90f, 135f);

            originalState.JunkItems.Add(junk);

            var json = originalState.ToJson();
            var deserializedState = GameState.FromJson(json);

            AssertThat(deserializedState.JunkItems).HasSize(1);
            var deserializedJunk = deserializedState.JunkItems[0];
            AssertThat(deserializedJunk.Id).IsEqual("serialized-junk");
            AssertThat(deserializedJunk.PosX).IsEqual(10f);
            AssertThat(deserializedJunk.PosY).IsEqual(20f);
            AssertThat(deserializedJunk.PosZ).IsEqual(30f);
            AssertThat(deserializedJunk.RotX).IsEqual(45f);
            AssertThat(deserializedJunk.RotY).IsEqual(90f);
            AssertThat(deserializedJunk.RotZ).IsEqual(135f);
        }

        [TestCase]
        public void CanFindJunkById()
        {
            var state = new GameState();
            var junk1 = new Junk("find-me", "junk_can", 1f, 2f, 3f, 0f, 0f, 0f);
            var junk2 = new Junk("not-me", "junk_can", 4f, 5f, 6f, 0f, 0f, 0f);

            state.JunkItems.Add(junk1);
            state.JunkItems.Add(junk2);

            var found = state.JunkItems.FirstOrDefault(j => j.Id == "find-me");

            AssertThat(found).IsNotNull();
            AssertThat(found.Id).IsEqual("find-me");
            AssertThat(found.PosX).IsEqual(1f);
        }

        [TestCase]
        public void MultipleJunkItemsCanCoexist()
        {
            var state = new GameState();
            var junkItems = new List<Junk>
            {
                new Junk("junk-1", "junk_can",1f, 1f, 1f, 0f, 0f, 0f),
                new Junk("junk-2", "junk_can",2f, 2f, 2f, 10f, 20f, 30f),
                new Junk("junk-3", "junk_can",3f, 3f, 3f, 45f, 90f, 180f)
            };

            foreach (var junk in junkItems)
            {
                state.JunkItems.Add(junk);
            }

            AssertThat(state.JunkItems).HasSize(3);
            foreach (var junk in state.JunkItems)
            {
                AssertThat(junk.Id).IsNotNull();
                AssertThat(junk.Id).StartsWith("junk-");
            }
        }

        [TestCase]
        public void JunkRecordsAreImmutable()
        {
            var junk = new Junk("test", "junk_can", 1f, 2f, 3f, 4f, 5f, 6f);

            // Records are immutable, so we can't modify properties directly
            // This test verifies the record behaves as expected
            AssertThat(junk.Id).IsEqual("test");
            AssertThat(junk.PosX).IsEqual(1f);

            // Create new instance with modified values
            var modifiedJunk = junk with { PosX = 10f };
            AssertThat(modifiedJunk.PosX).IsEqual(10f);
            AssertThat(junk.PosX).IsEqual(1f); // Original unchanged
        }
    }
}