using UnityEngine;
using KiarcheContinuumWar.Pathfinding;

namespace KiarcheContinuumWar.Map
{
    /// <summary>
    /// Компонент препятствия.
    /// Автоматически регистрируется в FlowFieldManager.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [Header("Obstacle Settings")]
        [SerializeField] private float obstacleRadius = 2f;
        [SerializeField] private bool registerOnStart = true;
        [SerializeField] private bool updateOnMove = true;

        [Header("Debug")]
        [SerializeField] private bool drawDebugGizmos = true;

        private FlowFieldManager _flowFieldManager;
        private bool _isRegistered = false;
        private Vector3 _lastPosition;

        public float ObstacleRadius => obstacleRadius;
        public bool IsRegistered => _isRegistered;

        // Public setter для редактора
        public void SetObstacleRadius(float radius)
        {
            obstacleRadius = radius;
        }

        private void Start()
        {
            _flowFieldManager = FlowFieldManager.Instance;
            _lastPosition = transform.position;

            if (registerOnStart)
            {
                RegisterObstacle();
            }
        }

        private void Update()
        {
            // Обновление препятствия при перемещении
            if (updateOnMove && _isRegistered)
            {
                if (Vector3.Distance(_lastPosition, transform.position) > 0.1f)
                {
                    UpdateObstacle();
                    _lastPosition = transform.position;
                }
            }
        }

        private void OnDestroy()
        {
            // TODO: Добавить удаление препятствия из FlowFieldManager
        }

        /// <summary>
        /// Зарегистрировать препятствие в FlowField.
        /// </summary>
        public void RegisterObstacle()
        {
            if (_flowFieldManager == null)
            {
                _flowFieldManager = FlowFieldManager.Instance;
            }

            // Регистрируем несколько точек вокруг препятствия
            int pointsCount = Mathf.CeilToInt(obstacleRadius * 4); // Точки каждые ~0.25 единицы
            
            for (int i = 0; i < pointsCount; i++)
            {
                float angle = (i / (float)pointsCount) * Mathf.PI * 2;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * obstacleRadius,
                    0,
                    Mathf.Sin(angle) * obstacleRadius
                );
                
                Vector3 worldPos = transform.position + offset;
                _flowFieldManager?.SetObstacle(worldPos);
            }

            _isRegistered = true;
        }

        /// <summary>
        /// Обновить препятствие (если переместилось).
        /// </summary>
        public void UpdateObstacle()
        {
            if (_isRegistered)
            {
                // Для простоты — полная перерегистрация
                // В будущем можно оптимизировать
                RegisterObstacle();
            }
        }

        /// <summary>
        /// Проверка, находится ли точка внутри препятствия.
        /// </summary>
        public bool IsPointInside(Vector3 point)
        {
            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(point.x, 0, point.z)
            );
            return distance <= obstacleRadius;
        }

        private void OnDrawGizmos()
        {
            if (!drawDebugGizmos) return;

            // Основной радиус препятствия
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, obstacleRadius);

            // Точки регистрации
            if (_isRegistered || Application.isPlaying)
            {
                int pointsCount = Mathf.CeilToInt(obstacleRadius * 4);
                
                for (int i = 0; i < pointsCount; i++)
                {
                    float angle = (i / (float)pointsCount) * Mathf.PI * 2;
                    Vector3 offset = new Vector3(
                        Mathf.Cos(angle) * obstacleRadius,
                        0,
                        Mathf.Sin(angle) * obstacleRadius
                    );
                    
                    Vector3 worldPos = transform.position + offset;
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(worldPos, 0.2f);
                }
            }
        }
    }
}
