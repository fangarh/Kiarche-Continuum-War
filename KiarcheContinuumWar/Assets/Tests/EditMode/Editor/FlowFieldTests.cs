using System.Linq;
using KiarcheContinuumWar.Pathfinding;
using NUnit.Framework;
using UnityEngine;

namespace KiarcheContinuumWar.Tests.EditMode
{
    public class FlowFieldTests
    {
        [Test]
        public void Constructor_InitializesCellsAsWalkableWithMaxCost()
        {
            var field = new FlowField(4, 3, 2f, new Vector3(-4f, 0f, -3f));

            Assert.That(field.Width, Is.EqualTo(4));
            Assert.That(field.Height, Is.EqualTo(3));
            Assert.That(field.CellSize, Is.EqualTo(2f));
            Assert.That(field.GetCell(new Vector2Int(2, 1)).IsWalkable, Is.True);
            Assert.That(field.GetCell(new Vector2Int(2, 1)).Cost, Is.EqualTo(int.MaxValue));
            Assert.That(field.GetCell(new Vector2Int(2, 1)).Direction, Is.EqualTo(Vector2Int.zero));
        }

        [Test]
        public void GridToWorldAndWorldToGrid_RoundTripCellCenter()
        {
            var field = new FlowField(10, 10, 1f, new Vector3(-5f, 0f, -5f));
            var gridPosition = new Vector2Int(3, 7);

            Vector3 worldPosition = field.GridToWorld(gridPosition);
            Vector2Int converted = field.WorldToGrid(worldPosition);

            Assert.That(converted, Is.EqualTo(gridPosition));
        }

        [Test]
        public void GetDirection_ReturnsNormalizedWorldDirectionFromStoredCellDirection()
        {
            var field = new FlowField(5, 5, 1f, Vector3.zero);
            field.SetCell(new Vector2Int(1, 1), new Vector2Int(1, 1), 14);

            Vector3 direction = field.GetDirection(new Vector3(1.5f, 0f, 1.5f));
            Vector3 expected = new Vector3(1f, 0f, 1f).normalized;

            Assert.That(direction.x, Is.EqualTo(expected.x).Within(0.0001f));
            Assert.That(direction.y, Is.EqualTo(expected.y).Within(0.0001f));
            Assert.That(direction.z, Is.EqualTo(expected.z).Within(0.0001f));
        }

        [Test]
        public void GetNeighbors_ExcludesBlockedAndOutOfBoundsCells()
        {
            var field = new FlowField(3, 3, 1f, Vector3.zero);
            field.SetObstacle(new Vector2Int(0, 0));
            field.SetObstacle(new Vector2Int(2, 2));

            var neighbors = field.GetNeighbors(new Vector2Int(1, 1));

            Assert.That(neighbors.Count, Is.EqualTo(6));
            Assert.That(neighbors.Contains(new Vector2Int(0, 0)), Is.False);
            Assert.That(neighbors.Contains(new Vector2Int(2, 2)), Is.False);
            Assert.That(neighbors.All(pos => pos.x >= 0 && pos.x < 3 && pos.y >= 0 && pos.y < 3), Is.True);
        }

        [Test]
        public void Clear_ResetsCellsBackToDefaultValues()
        {
            var field = new FlowField(2, 2, 1f, Vector3.zero);
            field.SetCell(new Vector2Int(1, 1), Vector2Int.left, 15, false);

            field.Clear();

            FlowField.Cell cell = field.GetCell(new Vector2Int(1, 1));
            Assert.That(cell.Direction, Is.EqualTo(Vector2Int.zero));
            Assert.That(cell.Cost, Is.EqualTo(int.MaxValue));
            Assert.That(cell.IsWalkable, Is.True);
        }
    }
}
