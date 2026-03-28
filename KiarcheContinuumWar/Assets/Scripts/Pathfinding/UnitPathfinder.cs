using UnityEngine;
using KiarcheContinuumWar.Pathfinding;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Компонент pathfinding для юнита.
    /// Использует Flow Field для движения к цели с улучшенным локальным избеганием.
    /// </summary>
    public class UnitPathfinder : MonoBehaviour
    {
        [Header("Pathfinding Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float arrivalDistance = 1.0f;
        [SerializeField] private float minMoveDistance = 0.001f;

        [Header("Debug")]
        [SerializeField] private bool drawDebugGizmos = true;

        [Header("Separation (avoidance)")]
        [SerializeField] private bool useSeparation = true;
        [SerializeField] private float separationDistance = 1.5f;
        [SerializeField] private float separationStrength = 3f;

        [Header("Obstacle Avoidance")]
        [SerializeField] private float obstacleAvoidanceRadius = 1f;
        [SerializeField] private float obstacleAvoidanceStrength = 5f;
        [SerializeField] private int obstacleRayCount = 5;
        [SerializeField] private float obstacleRayLength = 3f;
        [SerializeField] private LayerMask obstacleLayerMask = -1;

        // Ссылки
        private FlowFieldManager _flowFieldManager;
        private Unit _unit;
        private CharacterController _characterController;

        // Состояние
        private Vector3 _targetPosition;
        private Unit _targetUnit;
        private bool _isMoving = false;
        private bool _hasTarget = false;
        private float _lastPathUpdateTime;
        private Vector3 _currentDirection;
        private Vector3 _currentVelocity;
        private float _stuckCheckTimer;
        private Vector3 _lastPosition;
        private int _stuckCount;

        // События
        public System.Action OnDestinationReached;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            if (_unit != null)
            {
                moveSpeed = _unit.MoveSpeed;
            }

            // Добавляем CharacterController для физической коллизии
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                _characterController = gameObject.AddComponent<CharacterController>();
                _characterController.radius = 0.3f;
                _characterController.height = 1f;
                _characterController.center = new Vector3(0, 0.5f, 0);
                _characterController.detectCollisions = true;
            }
        }

        private void Start()
        {
            _flowFieldManager = FlowFieldManager.Instance;
            _targetPosition = transform.position;
            _lastPosition = transform.position;
            _stuckCheckTimer = 0f;
            _stuckCount = 0;
        }

        private void Update()
        {
            if (_unit == null || !_unit.IsAlive) return;

            // Проверка достижения цели (с увеличенным радиусом для предотвращения дрожания)
            if (_isMoving && !_hasTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
                
                // Отладка
                if (drawDebugGizmos && distanceToTarget < arrivalDistance * 2f)
                {
                    Debug.Log($"[UnitPathfinder] Distance to target: {distanceToTarget:F3} (arrival: {arrivalDistance})");
                }
                
                if (distanceToTarget < arrivalDistance)
                {
                    _isMoving = false;
                    OnDestinationReached?.Invoke();
                    
                    if (drawDebugGizmos)
                    {
                        Debug.Log($"[UnitPathfinder] Destination reached at {transform.position}");
                    }
                    return; // Выходим сразу, чтобы не проверять застревание
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

            // Проверка застревания (только если не близко к цели)
            if (_isMoving)
            {
                CheckIfStuck();
            }

            // Движение
            if (_isMoving)
            {
                Move();
            }
        }

        /// <summary>
        /// Проверка, застрял ли юнит.
        /// </summary>
        private void CheckIfStuck()
        {
            // Не проверяем застревание, если близко к цели (чтобы избежать ложных срабатываний)
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            if (distanceToTarget < arrivalDistance * 2f)
            {
                _stuckCount = 0;
                _stuckCheckTimer = 0f;
                return;
            }

            _stuckCheckTimer += Time.deltaTime;
            if (_stuckCheckTimer < 0.5f) return;

            _stuckCheckTimer = 0f;

            float distanceMoved = Vector3.Distance(_lastPosition, transform.position);
            if (distanceMoved < 0.01f && _isMoving)
            {
                _stuckCount++;

                // Если застрял 3 раза подряд - пересчитать путь
                if (_stuckCount >= 3)
                {
                    _stuckCount = 0;
                    RecalculatePath();
                }
            }
            else
            {
                _stuckCount = 0;
            }

            _lastPosition = transform.position;
        }

        /// <summary>
        /// Пересчитать путь при застревании.
        /// </summary>
        private void RecalculatePath()
        {
            // Небольшое смещение цели для обхода препятствия
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized * 2f;
            
            Vector3 newTarget = _hasTarget && _targetUnit != null 
                ? _targetUnit.transform.position + offset
                : _targetPosition + offset;
            
            _flowFieldManager?.GenerateFlowField(newTarget);
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
        /// Движение по полю потока с плавной остановкой near destination.
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

            // Применяем separation (избегание других юнитов)
            Vector3 finalDirection = flowDirection;
            if (useSeparation)
            {
                finalDirection += CalculateSeparation();
            }

            // Применяем avoidance препятствий (веер raycast)
            Vector3 obstacleAvoidance = CalculateObstacleAvoidance(finalDirection);
            finalDirection += obstacleAvoidance;

            // Вычисляем расстояние до цели для плавной остановки
            float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
            float speedMultiplier = 1f;
            
            // Плавное замедление при приближении к цели (только в радиусе arrivalDistance)
            if (distanceToTarget < arrivalDistance)
            {
                speedMultiplier = distanceToTarget / arrivalDistance;
            }

            // Нормализуем и применяем движение
            if (finalDirection.sqrMagnitude > 0.001f)
            {
                // Движение только по XZ плоскости с плавным замедлением
                Vector3 movement = finalDirection.normalized * moveSpeed * speedMultiplier * Time.deltaTime;
                movement.y = 0;

                // Используем CharacterController для движения с коллизиями
                if (_characterController != null && _characterController.enabled)
                {
                    CharacterControllerMove(movement);
                }
                else
                {
                    transform.position += movement;
                }

                // Сохраняем Y позицию (чтобы юнит не падал/летал)
                transform.position = new Vector3(transform.position.x, currentY, transform.position.z);

                // Поворот по направлению движения (только если двигаемся достаточно быстро)
                if (movement.magnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Расчёт избегания препятствий с помощью веера raycast.
        /// </summary>
        private Vector3 CalculateObstacleAvoidance(Vector3 currentDirection)
        {
            Vector3 avoidance = Vector3.zero;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

            // Стреляем лучами вперёд веером
            for (int i = 0; i < obstacleRayCount; i++)
            {
                // Вычисляем угол для каждого луча
                float angle = -30f + (i / (float)(obstacleRayCount - 1)) * 60f;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 rayDirection = rotation * currentDirection;

                if (rayDirection.magnitude < 0.01f)
                {
                    rayDirection = transform.forward;
                }

                // Пускаем raycast
                if (Physics.SphereCast(rayOrigin, obstacleAvoidanceRadius * 0.5f, rayDirection, out RaycastHit hit, obstacleRayLength, obstacleLayerMask))
                {
                    // Проверяем на препятствие (через компонент Obstacle)
                    Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
                    if (obstacle != null)
                    {
                        // Вычисляем направление обхода (перпендикулярно к препятствию)
                        Vector3 toObstacle = hit.point - transform.position;
                        toObstacle.y = 0;

                        // Направление обхода - перпендикулярно к препятствию и направлению движения
                        Vector3 perpendicular = Vector3.Cross(toObstacle.normalized, Vector3.up).normalized;

                        // Выбираем направление обхода (вправо или влево)
                        float rightDot = Vector3.Dot(perpendicular, currentDirection);
                        if (rightDot < 0)
                        {
                            perpendicular = -perpendicular;
                        }

                        // Сила избегания зависит от расстояния
                        float distanceStrength = 1f - (hit.distance / obstacleRayLength);
                        avoidance += perpendicular * distanceStrength * obstacleAvoidanceStrength;

                        Debug.DrawLine(transform.position, hit.point, Color.red, 0.1f);
                    }
                }
            }

            return avoidance;
        }

        /// <summary>
        /// Движение через CharacterController с обработкой коллизий.
        /// </summary>
        private void CharacterControllerMove(Vector3 movement)
        {
            if (_characterController == null || !_characterController.enabled) return;

            // Пытаемся двигаться
            CollisionFlags flags = _characterController.Move(movement);
            
            // Если столкнулись - пытаемся обойти
            if (flags != CollisionFlags.None)
            {
                // Если столкнулись - пытаемся обойти
                Vector3 slideDirection = movement;
                slideDirection.y = 0;

                // Проверяем, есть ли пространство для обхода
                if (Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.3f, slideDirection, out RaycastHit hit, 0.5f, obstacleLayerMask))
                {
                    // Пытаемся обойти справа
                    Vector3 rightDir = Vector3.Cross(slideDirection, Vector3.up).normalized;
                    if (!_characterController.Raycast(new Ray(transform.position, rightDir), out RaycastHit rightHit, 0.5f))
                    {
                        _characterController.Move(rightDir * movement.magnitude * 0.5f);
                    }
                    else if (!_characterController.Raycast(new Ray(transform.position, -rightDir), out RaycastHit leftHit, 0.5f))
                    {
                        _characterController.Move(-rightDir * movement.magnitude * 0.5f);
                    }
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
                        // Чем ближе юнит, тем сильнее отталкивание (квадратичное затухание)
                        float strength = (separationDistance - distance) / separationDistance;
                        strength = strength * strength; // Усиливаем близкое отталкивание
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
            // Показываем цель
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_targetPosition, 0.3f);

            // Показываем радиус остановки
            if (drawDebugGizmos)
            {
                Gizmos.color = new Color(1, 0, 1, 0.3f);
                Gizmos.DrawWireSphere(_targetPosition, arrivalDistance);
            }

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
