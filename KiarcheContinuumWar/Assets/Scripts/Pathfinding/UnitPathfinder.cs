using UnityEngine;
using KiarcheContinuumWar.Pathfinding;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Компонент pathfinding для юнита.
    /// Использует Flow Field для движения к цели.
    /// </summary>
    public class UnitPathfinder : MonoBehaviour
    {
        [Header("Pathfinding Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.1f;
        
        [Header("Separation (avoidance)")]
        [SerializeField] private bool useSeparation = true;
        [SerializeField] private float separationDistance = 1f;
        [SerializeField] private float separationStrength = 2f;

        // Ссылки
        private FlowFieldManager _flowFieldManager;
        private Unit _unit;
        
        // Состояние
        private Vector3 _targetPosition;
        private Unit _targetUnit;
        private bool _isMoving = false;
        private bool _hasTarget = false;
        private float _lastPathUpdateTime;
        private Vector3 _currentDirection;

        // События
        public System.Action OnDestinationReached;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            if (_unit != null)
            {
                moveSpeed = _unit.MoveSpeed;
            }
        }

        private void Start()
        {
            _flowFieldManager = FlowFieldManager.Instance;
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (_unit == null || !_unit.IsAlive) return;

            // Проверка достижения цели
            if (_isMoving && !_hasTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
                if (distanceToTarget < stoppingDistance)
                {
                    _isMoving = false;
                    OnDestinationReached?.Invoke();
                }
            }

            // Проверка цели-юнита
            if (_hasTarget && _targetUnit != null)
            {
                if (!_targetUnit.IsAlive)
                {
                    _hasTarget = false;
                    _targetUnit = null;
                    _isMoving = false;
                }
                else
                {
                    float distanceToTarget = Vector3.Distance(transform.position, _targetUnit.transform.position);
                    if (distanceToTarget <= _unit.AttackRange)
                    {
                        _isMoving = false;
                    }
                }
            }

            // Движение
            if (_isMoving)
            {
                Move();
            }
        }

        /// <summary>
        /// Установить целевую позицию для движения.
        /// </summary>
        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
            _targetUnit = null;
            _hasTarget = false;
            _isMoving = true;
            
            // Генерируем поле потока от цели
            _flowFieldManager?.GenerateFlowField(position);
        }

        /// <summary>
        /// Установить целевой юнит для атаки.
        /// </summary>
        public void SetTarget(Unit target)
        {
            if (target == null) return;
            
            _targetUnit = target;
            _hasTarget = true;
            _isMoving = true;
            
            // Генерируем поле потока от цели
            _flowFieldManager?.GenerateFlowField(target.transform.position);
        }

        /// <summary>
        /// Остановить движение.
        /// </summary>
        public void Stop()
        {
            _isMoving = false;
        }

        /// <summary>
        /// Движение по полю потока.
        /// </summary>
        private void Move()
        {
            // Сохраняем текущую Y позицию (на terrain)
            float currentY = transform.position.y;

            // Получаем направление от Flow Field
            Vector3 flowDirection = Vector3.zero;

            if (_hasTarget && _targetUnit != null)
            {
                // Если цель - юнит, обновляем поле от её позиции
                flowDirection = _flowFieldManager?.GetDirection(_targetUnit.transform.position) ?? Vector3.zero;
            }
            else
            {
                // Если цель - позиция, используем её
                flowDirection = _flowFieldManager?.GetDirection(transform.position) ?? Vector3.zero;
            }

            // Применяем separation (избегание столкновений с другими юнитами)
            Vector3 finalDirection = flowDirection;
            if (useSeparation)
            {
                finalDirection += CalculateSeparation();
            }
            
            // Проверка препятствий впереди (кастинг вперёд на 2 единицы)
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(rayOrigin, finalDirection, out RaycastHit hit, 2f))
            {
                // Проверка на препятствие (по Obstacle компоненту)
                if (hit.collider.GetComponent<Obstacle>() != null)
                {
                    // Обойти препятствие - перпендикулярное направление
                    Vector3 avoidanceDir = Vector3.Cross(finalDirection, Vector3.up).normalized;
                    
                    // Проверить, можно ли пойти в сторону обхода
                    if (!Physics.Raycast(rayOrigin, avoidanceDir, 1f))
                    {
                        finalDirection = avoidanceDir;
                    }
                    else if (!Physics.Raycast(rayOrigin, -avoidanceDir, 1f))
                    {
                        finalDirection = -avoidanceDir;
                    }
                    else
                    {
                        // Если везде препятствия - остановиться
                        finalDirection = Vector3.zero;
                    }
                    
                    Debug.DrawLine(transform.position, transform.position + finalDirection * 2, Color.cyan, 0.2f);
                }
            }

            if (finalDirection.sqrMagnitude > 0.001f)
            {
                // Движение только по XZ плоскости
                Vector3 movement = finalDirection.normalized * moveSpeed * Time.deltaTime;
                movement.y = 0; // Движение только по горизонтали

                transform.position += movement;

                // Сохраняем Y позицию (чтобы юнит не падал/летал)
                transform.position = new Vector3(transform.position.x, currentY, transform.position.z);

                // Поворот по направлению движения
                if (movement.magnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Расчёт силы разделения (avoidance других юнитов).
        /// </summary>
        private Vector3 CalculateSeparation()
        {
            Vector3 separation = Vector3.zero;
            
            // Ищем ближайших юнитов
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, separationDistance);
            
            foreach (Collider collider in nearbyColliders)
            {
                if (collider.gameObject == gameObject) continue;
                
                UnitPathfinder otherUnit = collider.GetComponent<UnitPathfinder>();
                if (otherUnit != null && otherUnit != this)
                {
                    Vector3 away = transform.position - collider.transform.position;
                    float distance = away.magnitude;
                    
                    if (distance > 0.01f)
                    {
                        // Чем ближе юнит, тем сильнее отталкивание
                        float strength = (separationDistance - distance) / separationDistance;
                        separation += away.normalized * strength * separationStrength;
                    }
                }
            }

            return separation;
        }

        /// <summary>
        /// Отладочная визуализация.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (_isMoving)
            {
                Gizmos.color = Color.yellow;
                
                if (_hasTarget && _targetUnit != null)
                {
                    Gizmos.DrawLine(transform.position, _targetUnit.transform.position);
                }
                else
                {
                    Gizmos.DrawLine(transform.position, _targetPosition);
                }
            }

            // Радиус separation
            if (useSeparation)
            {
                Gizmos.color = new Color(1, 1, 0, 0.3f);
                Gizmos.DrawWireSphere(transform.position, separationDistance);
            }
        }
    }
}
