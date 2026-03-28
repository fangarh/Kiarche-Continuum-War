using UnityEngine;
using System.Collections.Generic;

namespace KiarcheContinuumWar.Pathfinding
{
    /// <summary>
    /// Поле потока для pathfinding.
    /// Хранит направление и стоимость для каждой ячейки сетки.
    /// </summary>
    public class FlowField
    {
        // Ячейка поля потока
        public struct Cell
        {
            public Vector2Int Direction;  // Направление движения (-1,0 до 1,1)
            public int Cost;              // Стоимость прохождения (0 = цель, >0 = расстояние)
            public bool IsWalkable;       // Проходимость ячейки
            
            public Cell(Vector2Int direction, int cost, bool isWalkable)
            {
                Direction = direction;
                Cost = cost;
                IsWalkable = isWalkable;
            }
        }

        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly Cell[,] _cells;
        private readonly Vector3 _origin;

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public Vector3 Origin => _origin;

        public FlowField(int width, int height, float cellSize, Vector3 origin)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _origin = origin;
            _cells = new Cell[width, height];

            // Инициализация ячеек
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = new Cell(Vector2Int.zero, int.MaxValue, true);
                }
            }
        }

        /// <summary>
        /// Получить ячейку по мировым координатам.
        /// </summary>
        public Cell GetCell(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            return GetCell(gridPos);
        }

        /// <summary>
        /// Получить ячейку по координатам сетки.
        /// </summary>
        public Cell GetCell(Vector2Int gridPos)
        {
            if (gridPos.x < 0 || gridPos.x >= _width || 
                gridPos.y < 0 || gridPos.y >= _height)
            {
                return new Cell(Vector2Int.zero, int.MaxValue, false);
            }
            return _cells[gridPos.x, gridPos.y];
        }

        /// <summary>
        /// Установить направление и стоимость ячейки.
        /// </summary>
        public void SetCell(Vector2Int gridPos, Vector2Int direction, int cost, bool isWalkable = true)
        {
            if (gridPos.x < 0 || gridPos.x >= _width || 
                gridPos.y < 0 || gridPos.y >= _height)
            {
                return;
            }
            _cells[gridPos.x, gridPos.y] = new Cell(direction, cost, isWalkable);
        }

        /// <summary>
        /// Получить направление для мировых координат.
        /// </summary>
        public Vector3 GetDirection(Vector3 worldPosition)
        {
            Vector2Int gridPos = WorldToGrid(worldPosition);
            Cell cell = GetCell(gridPos);
            return new Vector3(cell.Direction.x, 0, cell.Direction.y).normalized;
        }

        /// <summary>
        /// Конвертировать мировые координаты в координаты сетки.
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - _origin.x) / _cellSize);
            int y = Mathf.FloorToInt((worldPosition.z - _origin.z) / _cellSize);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Конвертировать координаты сетки в мировые.
        /// </summary>
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(
                gridPos.x * _cellSize + _origin.x + _cellSize / 2,
                0,
                gridPos.y * _cellSize + _origin.z + _cellSize / 2
            );
        }

        /// <summary>
        /// Получить соседние проходимые ячейки.
        /// </summary>
        public List<Vector2Int> GetNeighbors(Vector2Int gridPos)
        {
            var neighbors = new List<Vector2Int>(8);
            
            // 8 направлений (включая диагонали)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int newX = gridPos.x + x;
                    int newY = gridPos.y + y;

                    if (newX >= 0 && newX < _width && newY >= 0 && newY < _height)
                    {
                        Cell cell = _cells[newX, newY];
                        if (cell.IsWalkable)
                        {
                            neighbors.Add(new Vector2Int(newX, newY));
                        }
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Очистить поле (сбросить к значениям по умолчанию).
        /// </summary>
        public void Clear()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _cells[x, y] = new Cell(Vector2Int.zero, int.MaxValue, true);
                }
            }
        }

        /// <summary>
        /// Установить ячейку как непроходимую (препятствие).
        /// </summary>
        public void SetObstacle(Vector2Int gridPos)
        {
            if (gridPos.x < 0 || gridPos.x >= _width || 
                gridPos.y < 0 || gridPos.y >= _height)
            {
                return;
            }
            
            Cell existing = _cells[gridPos.x, gridPos.y];
            _cells[gridPos.x, gridPos.y] = new Cell(existing.Direction, existing.Cost, false);
        }

        /// <summary>
        /// Отладочная визуализация поля.
        /// </summary>
        public void DebugDraw()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Cell cell = _cells[x, y];
                    Vector3 worldPos = GridToWorld(new Vector2Int(x, y));

                    // Цвет по стоимости
                    Color color = cell.IsWalkable 
                        ? Color.HSVToRGB(0.6f, 1f - Mathf.Clamp01((float)cell.Cost / 100), 1f)
                        : Color.red;

                    Gizmos.color = color;
                    Gizmos.DrawSphere(worldPos, 0.1f);

                    // Стрелка направления
                    if (cell.IsWalkable && cell.Cost < int.MaxValue && cell.Cost > 0)
                    {
                        Vector3 dir = new Vector3(cell.Direction.x, 0, cell.Direction.y).normalized;
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(worldPos, worldPos + dir * 0.3f);
                    }
                }
            }
        }
    }
}
