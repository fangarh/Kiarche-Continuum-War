using System.Collections.Generic;
using UnityEngine;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Pathfinding
{
    /// <summary>
    /// Генерирует и управляет FlowField для pathfinding юнитов.
    /// </summary>
    public class FlowFieldManager : MonoBehaviour
    {
        [Header("Field Settings")]
        [SerializeField] private int fieldWidth = 100;
        [SerializeField] private int fieldHeight = 100;
        [SerializeField] private float cellSize = 1f;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask obstacleLayerMask = -1;
        [SerializeField] private bool autoScanObstacles = true;
        [SerializeField] private float obstacleScanRadius = 50f;

        [Header("Debug")]
        [SerializeField] private bool drawDebugGizmos;

        private FlowField _currentField;
        private Vector3 _fieldOrigin;
        private readonly List<Vector3> _obstaclePositions = new List<Vector3>();
        private readonly HashSet<Vector2Int> _obstacleGridPositions = new HashSet<Vector2Int>();

        private static FlowFieldManager _instance;
        public static FlowFieldManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<FlowFieldManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("FlowFieldManager");
                        _instance = go.AddComponent<FlowFieldManager>();
                    }
                }

                return _instance;
            }
        }

        public FlowField CurrentField => _currentField;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            InitializeField();

            if (autoScanObstacles)
            {
                ScanObstacles();
            }
        }

        private void InitializeField()
        {
            MapManager mapManager = MapManager.Instance;
            if (mapManager != null)
            {
                fieldWidth = Mathf.Max(1, Mathf.CeilToInt(mapManager.MapWidth / cellSize));
                fieldHeight = Mathf.Max(1, Mathf.CeilToInt(mapManager.MapHeight / cellSize));
                _fieldOrigin = mapManager.MapOrigin;
            }
            else
            {
                _fieldOrigin = new Vector3(
                    -fieldWidth * cellSize / 2f,
                    0f,
                    -fieldHeight * cellSize / 2f);
            }

            _currentField = new FlowField(fieldWidth, fieldHeight, cellSize, _fieldOrigin);
        }

        public void ScanObstacles()
        {
            _obstaclePositions.Clear();
            _obstacleGridPositions.Clear();

            MapManager mapManager = MapManager.Instance;
            Vector3 scanCenter = Vector3.zero;
            Vector3 scanHalfExtents = new Vector3(obstacleScanRadius, 512f, obstacleScanRadius);

            if (mapManager != null)
            {
                scanCenter = new Vector3(
                    mapManager.MapOrigin.x + mapManager.MapWidth * 0.5f,
                    0f,
                    mapManager.MapOrigin.z + mapManager.MapHeight * 0.5f);
                scanHalfExtents = new Vector3(
                    mapManager.MapWidth * 0.5f + cellSize,
                    512f,
                    mapManager.MapHeight * 0.5f + cellSize);
            }

            Collider[] colliders = Physics.OverlapBox(scanCenter, scanHalfExtents, Quaternion.identity, obstacleLayerMask);
            foreach (Collider collider in colliders)
            {
                Obstacle obstacle = collider.GetComponent<Obstacle>() ?? collider.GetComponentInParent<Obstacle>();
                if (obstacle == null)
                {
                    continue;
                }

                Vector3 obstaclePosition = collider.transform.position;
                if (!_obstaclePositions.Contains(obstaclePosition))
                {
                    _obstaclePositions.Add(obstaclePosition);
                }

                RegisterObstacleArea(obstaclePosition, Mathf.Max(obstacle.ObstacleRadius, cellSize * 0.5f));
            }

        }

        public void AddDynamicObstacle(Vector3 worldPosition, float radius = 0.5f)
        {
            RegisterObstacleArea(worldPosition, radius);
        }

        public void GenerateFlowField(Vector3 targetPosition)
        {
            if (_currentField == null)
            {
                InitializeField();
            }

            targetPosition = ClampToMapBounds(targetPosition);
            _currentField.Clear();
            ApplyRegisteredObstacles();

            Vector2Int targetGrid = _currentField.WorldToGrid(targetPosition);
            if (!IsInsideBounds(targetGrid))
            {
                Debug.LogWarning($"Target position {targetPosition} is outside field bounds");
                return;
            }

            if (!_currentField.GetCell(targetGrid).IsWalkable)
            {
                Debug.LogWarning($"Target position {targetPosition} is inside an obstacle. Finding nearest walkable cell...");
                targetGrid = FindNearestWalkableCell(targetGrid);
                if (targetGrid.x == -1)
                {
                    Debug.LogError("No walkable path to target!");
                    return;
                }
            }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            _currentField.SetCell(targetGrid, Vector2Int.zero, 0, true);
            queue.Enqueue(targetGrid);

            Vector2Int[] directions =
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(1, -1),
                new Vector2Int(-1, 1), new Vector2Int(-1, -1)
            };

            int processedCells = 0;
            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                int currentCost = _currentField.GetCell(current).Cost;
                processedCells++;

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;
                    if (!IsInsideBounds(neighbor))
                    {
                        continue;
                    }

                    if (direction.x != 0 && direction.y != 0 && IsDiagonalBlocked(current, direction))
                    {
                        continue;
                    }

                    FlowField.Cell neighborCell = _currentField.GetCell(neighbor);
                    int moveCost = direction.x != 0 && direction.y != 0 ? 14 : 10;
                    int newCost = currentCost + moveCost;

                    if (!neighborCell.IsWalkable || newCost >= neighborCell.Cost)
                    {
                        continue;
                    }

                    _currentField.SetCell(neighbor, current - neighbor, newCost, true);
                    queue.Enqueue(neighbor);
                }
            }

        }

        public void GenerateFlowFieldFromMultipleTargets(List<Vector3> targetPositions)
        {
            if (_currentField == null)
            {
                InitializeField();
            }

            _currentField.Clear();
            ApplyRegisteredObstacles();

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            foreach (Vector3 targetPosition in targetPositions)
            {
                Vector2Int targetGrid = _currentField.WorldToGrid(ClampToMapBounds(targetPosition));
                if (!IsInsideBounds(targetGrid))
                {
                    continue;
                }

                if (!_currentField.GetCell(targetGrid).IsWalkable)
                {
                    targetGrid = FindNearestWalkableCell(targetGrid);
                    if (targetGrid.x == -1)
                    {
                        continue;
                    }
                }

                _currentField.SetCell(targetGrid, Vector2Int.zero, 0, true);
                queue.Enqueue(targetGrid);
            }

            Vector2Int[] directions =
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(1, -1),
                new Vector2Int(-1, 1), new Vector2Int(-1, -1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                int currentCost = _currentField.GetCell(current).Cost;

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;
                    if (!IsInsideBounds(neighbor))
                    {
                        continue;
                    }

                    if (direction.x != 0 && direction.y != 0 && IsDiagonalBlocked(current, direction))
                    {
                        continue;
                    }

                    FlowField.Cell neighborCell = _currentField.GetCell(neighbor);
                    int moveCost = direction.x != 0 && direction.y != 0 ? 14 : 10;
                    int newCost = currentCost + moveCost;

                    if (!neighborCell.IsWalkable || newCost >= neighborCell.Cost)
                    {
                        continue;
                    }

                    _currentField.SetCell(neighbor, current - neighbor, newCost, true);
                    queue.Enqueue(neighbor);
                }
            }
        }

        public void SetObstacle(Vector3 worldPosition)
        {
            RegisterObstacleArea(worldPosition, cellSize * 0.5f);
        }

        public void SetObstacle(Vector3 worldPosition, float radius)
        {
            RegisterObstacleArea(worldPosition, radius);
        }

        public void SetCircularObstacle(Vector3 center, float radius)
        {
            RegisterObstacleArea(center, radius);
        }

        public void ClearObstacle(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return;
            }

            Vector2Int gridPos = _currentField.WorldToGrid(worldPosition);
            _obstacleGridPositions.Remove(gridPos);
            FlowField.Cell cell = _currentField.GetCell(gridPos);
            _currentField.SetCell(gridPos, cell.Direction, cell.Cost, true);
        }

        public Vector3 GetDirection(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return Vector3.zero;
            }

            return _currentField.GetDirection(worldPosition);
        }

        public int GetCost(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return int.MaxValue;
            }

            return _currentField.GetCell(worldPosition).Cost;
        }

        public bool IsReachable(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return false;
            }

            FlowField.Cell cell = _currentField.GetCell(worldPosition);
            return cell.IsWalkable && cell.Cost < int.MaxValue;
        }

        private Vector2Int FindNearestWalkableCell(Vector2Int startGrid)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            queue.Enqueue(startGrid);
            visited.Add(startGrid);

            Vector2Int[] directions =
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                if (_currentField.GetCell(current).IsWalkable)
                {
                    return current;
                }

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;
                    if (!IsInsideBounds(neighbor) || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return new Vector2Int(-1, -1);
        }

        private void ApplyRegisteredObstacles()
        {
            foreach (Vector2Int gridPos in _obstacleGridPositions)
            {
                _currentField.SetObstacle(gridPos);
            }
        }

        private void RegisterObstacleArea(Vector3 center, float radius)
        {
            if (_currentField == null)
            {
                InitializeField();
            }

            Vector2Int centerGrid = _currentField.WorldToGrid(center);
            int gridRadius = Mathf.Max(1, Mathf.CeilToInt(radius / cellSize));
            float radiusSqr = radius * radius;
            float padding = cellSize * cellSize * 0.5f;

            for (int x = -gridRadius; x <= gridRadius; x++)
            {
                for (int y = -gridRadius; y <= gridRadius; y++)
                {
                    Vector2Int gridPos = centerGrid + new Vector2Int(x, y);
                    if (!IsInsideBounds(gridPos))
                    {
                        continue;
                    }

                    Vector3 worldPos = _currentField.GridToWorld(gridPos);
                    Vector2 delta = new Vector2(worldPos.x - center.x, worldPos.z - center.z);
                    if (delta.sqrMagnitude > radiusSqr + padding)
                    {
                        continue;
                    }

                    _currentField.SetObstacle(gridPos);
                    _obstacleGridPositions.Add(gridPos);
                }
            }

            if (!_obstaclePositions.Contains(center))
            {
                _obstaclePositions.Add(center);
            }
        }

        private bool IsInsideBounds(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < fieldWidth && gridPos.y >= 0 && gridPos.y < fieldHeight;
        }

        private Vector3 ClampToMapBounds(Vector3 position)
        {
            MapManager mapManager = MapManager.Instance;
            if (mapManager == null)
            {
                return position;
            }

            Vector3 clamped = mapManager.ClampToBounds(position);
            clamped.y = mapManager.GetTerrainHeight(clamped);
            return clamped;
        }

        private bool IsDiagonalBlocked(Vector2Int current, Vector2Int direction)
        {
            Vector2Int horizontal = new Vector2Int(current.x + direction.x, current.y);
            Vector2Int vertical = new Vector2Int(current.x, current.y + direction.y);

            return !_currentField.GetCell(horizontal).IsWalkable || !_currentField.GetCell(vertical).IsWalkable;
        }

        private void OnDrawGizmos()
        {
            if (!drawDebugGizmos || _currentField == null)
            {
                return;
            }

            _currentField.DebugDraw();
        }
    }
}
