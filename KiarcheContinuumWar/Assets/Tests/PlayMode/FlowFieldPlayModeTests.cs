using System.Collections;
using KiarcheContinuumWar.Pathfinding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace KiarcheContinuumWar.Tests.PlayMode
{
    public class FlowFieldPlayModeTests
    {
        [UnityTest]
        public IEnumerator FlowField_PreservesObstacleStateAcrossFrame()
        {
            var field = new FlowField(4, 4, 1f, Vector3.zero);
            Vector2Int obstacle = new Vector2Int(2, 1);

            field.SetObstacle(obstacle);

            yield return null;

            FlowField.Cell cell = field.GetCell(obstacle);
            Assert.That(cell.IsWalkable, Is.False);
            Assert.That(field.GetNeighbors(new Vector2Int(1, 1)), Has.No.Member(obstacle));
        }

        [UnityTest]
        public IEnumerator FlowField_WorldGridConversion_RemainsStableInPlayMode()
        {
            var field = new FlowField(6, 6, 2f, new Vector3(-6f, 0f, -6f));
            Vector2Int gridPosition = new Vector2Int(4, 3);
            Vector3 worldPosition = field.GridToWorld(gridPosition);

            yield return null;

            Assert.That(field.WorldToGrid(worldPosition), Is.EqualTo(gridPosition));
        }
    }
}
