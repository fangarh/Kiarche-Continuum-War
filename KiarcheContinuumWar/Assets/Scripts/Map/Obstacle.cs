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
        private bool _isRegistered;
        private Vector3 _lastPosition;

        public float ObstacleRadius => obstacleRadius;
        public bool IsRegistered => _isRegistered;

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
            if (!updateOnMove || !_isRegistered)
            {
                return;
            }

            if (Vector3.Distance(_lastPosition, transform.position) > 0.1f)
            {
                UpdateObstacle();
                _lastPosition = transform.position;
            }
        }

        public void RegisterObstacle()
        {
            if (_flowFieldManager == null)
            {
                _flowFieldManager = FlowFieldManager.Instance;
            }

            Collider[] childColliders = GetComponentsInChildren<Collider>(true);
            foreach (Collider childCollider in childColliders)
            {
                childCollider.gameObject.layer = gameObject.layer;
            }

            _flowFieldManager?.SetObstacle(transform.position, obstacleRadius);
            _isRegistered = true;
        }

        public void UpdateObstacle()
        {
            if (_isRegistered)
            {
                RegisterObstacle();
            }
        }

        public bool IsPointInside(Vector3 point)
        {
            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(point.x, 0, point.z));

            return distance <= obstacleRadius;
        }

        private void OnDrawGizmos()
        {
            if (!drawDebugGizmos)
            {
                return;
            }

            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, obstacleRadius);
        }
    }
}
