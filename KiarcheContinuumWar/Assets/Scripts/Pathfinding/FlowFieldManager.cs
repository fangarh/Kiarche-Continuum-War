using UnityEngine;
using System.Collections.Generic;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Pathfinding
{
    /// <summary>
    /// Менеджер полей потока.
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
        [SerializeField] private bool drawDebugGizmos = false;

        private FlowField _currentField;
        private Vector3 _fieldOrigin;

        // Список препятствий для динамического обновления
        private List<Vector3> _obstaclePositions = new List<Vector3>();
        private HashSet<Vector2Int> _obstacleGridPositions = new HashSet<Vector2Int>();

        // Singleton для доступа из любого места
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

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            InitializeField();
            
            // Автоматическое сканирование препятствий при старте
            if (autoScanObstacles)
            {
                ScanObstacles();
            }
        }

        /// <summary>
        /// Инициализировать поле потока.
        /// </summary>
        private void InitializeField()
        {
            // Вычисляем_origin так, чтобы поле было центрировано на (0, 0, 0)
            _fieldOrigin = new Vector3(
                -fieldWidth * cellSize / 2,
                0,
                -fieldHeight * cellSize / 2
            );

            _currentField = new FlowField(fieldWidth, fieldHeight, cellSize, _fieldOrigin);
        }

        /// <summary>
        /// Автоматическое сканирование препятствий на сцене.
        /// </summary>
        public void ScanObstacles()
        {
            _obstaclePositions.Clear();
            _obstacleGridPositions.Clear();

            // Находим все препятствия через Physics.OverlapSphere
            Collider[] colliders = Physics.OverlapSphere(Vector3.zero, obstacleScanRadius, obstacleLayerMask);

            foreach (Collider collider in colliders)
            {
                Obstacle obstacle = collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    // Добавляем позицию препятствия
                    Vector3 pos = collider.transform.position;
                    if (!_obstaclePositions.Contains(pos))
                    {
                        _obstaclePositions.Add(pos);

                        // Добавляем все ячейки, которые занимает препятствие
                        Vector2Int gridPos = _currentField?.WorldToGrid(pos) ?? Vector2Int.zero;
                        if (!_obstacleGridPositions.Contains(gridPos))
                        {
                            _obstacleGridPositions.Add(gridPos);
                        }
                    }
                }
            }

            Debug.Log($"[FlowFieldManager] Найдено препятствий: {_obstaclePositions.Count}");
        }

        /// <summary>
        /// Добавить динамическое препятствие (например, юнит).
        /// </summary>
        public void AddDynamicObstacle(Vector3 worldPosition, float radius = 0.5f)
        {
            if (_currentField == null) return;

            Vector2Int gridPos = _currentField.WorldToGrid(worldPosition);
            
            // Добавляем центральную ячейку и соседние (в зависимости от радиуса)
            int cellsRadius = Mathf.CeilToInt(radius / cellSize);
            for (int x = -cellsRadius; x <= cellsRadius; x++)
            {
                for (int y = -cellsRadius; y <= cellsRadius; y++)
                {
                    Vector2Int pos = gridPos + new Vector2Int(x, y);
                    if (pos.x >= 0 && pos.x < fieldWidth && pos.y >= 0 && pos.y < fieldHeight)
                    {
                        _currentField.SetObstacle(pos);
                    }
                }
            }
        }

        /// <summary>
        /// Сгенерировать поле потока от целевой точки.
        /// Использует BFS для расчёта стоимости и направлений.
        /// </summary>
        /// <param name="targetPosition">Целевая позиция в мировых координатах</param>
        public void GenerateFlowField(Vector3 targetPosition)
        {
            if (_currentField == null)
            {
                InitializeField();
            }

            _currentField.Clear();

            // Перерегистрировать все статические препятствия
            foreach (Vector2Int gridPos in _obstacleGridPositions)
            {
                _currentField.SetObstacle(gridPos);
            }

            Vector2Int targetGrid = _currentField.WorldToGrid(targetPosition);

            // Проверка границ
            if (targetGrid.x < 0 || targetGrid.x >= fieldWidth ||
                targetGrid.y < 0 || targetGrid.y >= fieldHeight)
            {
                Debug.LogWarning($"Target position {targetPosition} is outside field bounds");
                return;
            }

            // Проверка: цель не в препятствии
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

            // BFS для генерации поля потока
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            // Устанавливаем цель (стоимость = 0)
            _currentField.SetCell(targetGrid, Vector2Int.zero, 0, true);
            queue.Enqueue(targetGrid);

            // Направления для 8 соседей
            Vector2Int[] directions = new Vector2Int[8]
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
                FlowField.Cell currentCell = _currentField.GetCell(current);
                int currentCost = currentCell.Cost;
                processedCells++;

                // Обрабатываем всех соседей
                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    // Проверка границ
                    if (neighbor.x < 0 || neighbor.x >= fieldWidth ||
                        neighbor.y < 0 || neighbor.y >= fieldHeight)
                    {
                        continue;
                    }

                    FlowField.Cell neighborCell = _currentField.GetCell(neighbor);

                    // Пропускаем если уже посещено или непроходимо
                    if (!neighborCell.IsWalkable || neighborCell.Cost <= currentCost + 1)
                    {
                        continue;
                    }

                    // Вычисляем стоимость (диагональ = 1.4, прямая = 1)
                    int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10;
                    int newCost = currentCost + moveCost;

                    if (newCost < neighborCell.Cost)
                    {
                        // Устанавливаем направление ОТ соседа К текущей ячейке (к цели)
                        Vector2Int flowDirection = current - neighbor;
                        _currentField.SetCell(neighbor, flowDirection, newCost, true);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            Debug.Log($"[FlowFieldManager] Flow field generated: {processedCells} cells, target at {targetGrid}");
        }

        /// <summary>
        /// Найти ближайшую проходимую ячейку.
        /// </summary>
        private Vector2Int FindNearestWalkableCell(Vector2Int startGrid)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            
            queue.Enqueue(startGrid);
            visited.Add(startGrid);

            Vector2Int[] directions = new Vector2Int[4]
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                
                FlowField.Cell cell = _currentField.GetCell(current);
                if (cell.IsWalkable)
                {
                    return current;
                }

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;
                    
                    if (neighbor.x >= 0 && neighbor.x < fieldWidth &&
                        neighbor.y >= 0 && neighbor.y < fieldHeight &&
                        !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            // Не найдено проходимой ячейки
            return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Сгенерировать поле потока от нескольких целей.
        /// </summary>
        public void GenerateFlowFieldFromMultipleTargets(List<Vector3> targetPositions)
        {
            if (_currentField == null)
            {
                InitializeField();
            }

            _currentField.Clear();

            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            // Добавляем все цели в очередь
            foreach (Vector3 targetPos in targetPositions)
            {
                Vector2Int targetGrid = _currentField.WorldToGrid(targetPos);
                
                if (targetGrid.x >= 0 && targetGrid.x < fieldWidth &&
                    targetGrid.y >= 0 && targetGrid.y < fieldHeight)
                {
                    _currentField.SetCell(targetGrid, Vector2Int.zero, 0, true);
                    queue.Enqueue(targetGrid);
                }
            }

            // BFS от всех целей одновременно
            Vector2Int[] directions = new Vector2Int[8]
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(1, -1),
                new Vector2Int(-1, 1), new Vector2Int(-1, -1)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                FlowField.Cell currentCell = _currentField.GetCell(current);
                int currentCost = currentCell.Cost;

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    if (neighbor.x < 0 || neighbor.x >= fieldWidth ||
                        neighbor.y < 0 || neighbor.y >= fieldHeight)
                    {
                        continue;
                    }

                    FlowField.Cell neighborCell = _currentField.GetCell(neighbor);

                    if (!neighborCell.IsWalkable || neighborCell.Cost <= currentCost + 1)
                    {
                        continue;
                    }

                    int moveCost = (dir.x != 0 && dir.y != 0) ? 14 : 10;
                    int newCost = currentCost + moveCost;

                    if (newCost < neighborCell.Cost)
                    {
                        Vector2Int flowDirection = current - neighbor;
                        _currentField.SetCell(neighbor, flowDirection, newCost, true);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        /// <summary>
        /// Установить препятствие в поле.
        /// </summary>
        public void SetObstacle(Vector3 worldPosition)
        {
            if (_currentField == null) return;

            Vector2Int gridPos = _currentField.WorldToGrid(worldPosition);
            _currentField.SetObstacle(gridPos);
            
            // Сохраняем позицию препятствия
            if (!_obstaclePositions.Contains(worldPosition))
            {
                _obstaclePositions.Add(worldPosition);
            }
        }

        /// <summary>
        /// Установить круглое препятствие (радиус).
        /// </summary>
        public void SetCircularObstacle(Vector3 center, float radius)
        {
            if (_currentField == null) return;

            int pointsCount = Mathf.CeilToInt(radius * 4); // Точки каждые ~0.25 единицы
            
            for (int i = 0; i < pointsCount; i++)
            {
                float angle = (i / (float)pointsCount) * Mathf.PI * 2;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                
                Vector3 worldPos = center + offset;
                SetObstacle(worldPos);
            }
        }

        /// <summary>
        /// Очистить препятствие (сделать проходимым).
        /// </summary>
        public void ClearObstacle(Vector3 worldPosition)
        {
            if (_currentField == null) return;

            Vector2Int gridPos = _currentField.WorldToGrid(worldPosition);
            FlowField.Cell cell = _currentField.GetCell(gridPos);
            _currentField.SetCell(gridPos, cell.Direction, cell.Cost, true);
        }

        /// <summary>
        /// Получить направление движения для юнита.
        /// </summary>
        public Vector3 GetDirection(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return Vector3.zero;
            }
            return _currentField.GetDirection(worldPosition);
        }

        /// <summary>
        /// Получить стоимость ячейки.
        /// </summary>
        public int GetCost(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return int.MaxValue;
            }
            FlowField.Cell cell = _currentField.GetCell(worldPosition);
            return cell.Cost;
        }

        /// <summary>
        /// Проверить, достижима ли цель.
        /// </summary>
        public bool IsReachable(Vector3 worldPosition)
        {
            if (_currentField == null)
            {
                return false;
            }
            FlowField.Cell cell = _currentField.GetCell(worldPosition);
            return cell.IsWalkable && cell.Cost < int.MaxValue;
        }

        /// <summary>
        /// Отладочная визуализация.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!drawDebugGizmos || _currentField == null)
            {
                return;
            }
            _currentField.DebugDraw();
        }

        // Публичный доступ к полю для отладки
        public FlowField CurrentField => _currentField;
    }
}
