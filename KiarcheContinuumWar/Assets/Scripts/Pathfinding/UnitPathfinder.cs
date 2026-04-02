using UnityEngine;
using KiarcheContinuumWar.Pathfinding;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Компонент pathfinding для юнита.
    /// Использует Flow Field для движения к цели с локальным избеганием.
    /// </summary>
    public class UnitPathfinder : MonoBehaviour
    {
        [Header("Pathfinding Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float arrivalDistance = 1.0f;
        [SerializeField] private float finalApproachDistance = 2.5f;
        [SerializeField] private float snapToTargetDistance = 0.35f;
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

        private FlowFieldManager _flowFieldManager;
        private Unit _unit;
        private CharacterController _characterController;

        private Vector3 _targetPosition;
        private Unit _targetUnit;
        private bool _isMoving;
        private bool _hasTarget;
        private float _stuckCheckTimer;
        private Vector3 _lastPosition;
        private int _stuckCount;

        public System.Action OnDestinationReached;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            if (_unit != null)
            {
                moveSpeed = _unit.MoveSpeed;
            }

            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                _characterController = gameObject.AddComponent<CharacterController>();
                _characterController.radius = 0.3f;
                _characterController.height = 1f;
                _characterController.center = new Vector3(0f, 0.5f, 0f);
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
            if (_unit == null || !_unit.IsAlive)
            {
                return;
            }

            if (_isMoving && !_hasTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
                if (distanceToTarget < arrivalDistance)
                {
                    SnapToDestinationIfClose(_targetPosition, distanceToTarget);
                    _isMoving = false;
                    OnDestinationReached?.Invoke();
                    return;
                }
            }

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
                    if (distanceToTarget <= _unit.AttackRange + stoppingDistance)
                    {
                        _isMoving = false;
                    }
                }
            }

            if (_isMoving)
            {
                CheckIfStuck();
                Move();
            }
        }

        private void CheckIfStuck()
        {
            Vector3 desiredTarget = _hasTarget && _targetUnit != null
                ? _targetUnit.transform.position
                : _targetPosition;
            float distanceToTarget = Vector3.Distance(transform.position, desiredTarget);
            if (distanceToTarget < arrivalDistance * 2f)
            {
                _stuckCount = 0;
                _stuckCheckTimer = 0f;
                return;
            }

            _stuckCheckTimer += Time.deltaTime;
            if (_stuckCheckTimer < 0.5f)
            {
                return;
            }

            _stuckCheckTimer = 0f;

            float distanceMoved = Vector3.Distance(_lastPosition, transform.position);
            if (distanceMoved < 0.01f && _isMoving)
            {
                _stuckCount++;
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

        private void RecalculatePath()
        {
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)).normalized * 2f;

            Vector3 newTarget = _hasTarget && _targetUnit != null
                ? _targetUnit.transform.position + offset
                : _targetPosition + offset;

            _flowFieldManager?.GenerateFlowField(newTarget);
        }

        public void SetTargetPosition(Vector3 position)
        {
            SetTargetPosition(position, true);
        }

        public void SetTargetPosition(Vector3 position, bool rebuildFlowField)
        {
            _targetPosition = position;
            _targetUnit = null;
            _hasTarget = false;
            _isMoving = true;
            if (rebuildFlowField)
            {
                _flowFieldManager?.GenerateFlowField(position);
            }
        }

        public void SetTarget(Unit target)
        {
            if (target == null)
            {
                return;
            }

            _targetUnit = target;
            _hasTarget = true;
            _isMoving = true;
            _flowFieldManager?.GenerateFlowField(target.transform.position);
        }

        public void Stop()
        {
            _isMoving = false;
        }

        private void Move()
        {
            float currentY = transform.position.y;
            Vector3 desiredTarget = _hasTarget && _targetUnit != null
                ? _targetUnit.transform.position
                : _targetPosition;
            float distanceToTarget = Vector3.Distance(transform.position, desiredTarget);

            if (!_hasTarget && distanceToTarget <= snapToTargetDistance)
            {
                SnapToDestinationIfClose(desiredTarget, distanceToTarget);
                _isMoving = false;
                OnDestinationReached?.Invoke();
                return;
            }

            Vector3 flowDirection;
            if (distanceToTarget <= finalApproachDistance)
            {
                flowDirection = desiredTarget - transform.position;
                flowDirection.y = 0f;
            }
            else
            {
                flowDirection = _flowFieldManager?.GetDirection(transform.position) ?? Vector3.zero;
                if (flowDirection.sqrMagnitude < minMoveDistance * minMoveDistance)
                {
                    flowDirection = desiredTarget - transform.position;
                    flowDirection.y = 0f;
                }
            }

            float avoidanceWeight = Mathf.Clamp01(
                (distanceToTarget - finalApproachDistance * 0.5f) /
                Mathf.Max(finalApproachDistance, 0.001f));

            Vector3 finalDirection = flowDirection;
            if (useSeparation)
            {
                finalDirection += CalculateSeparation() * avoidanceWeight;
            }

            finalDirection += CalculateObstacleAvoidance(finalDirection) * avoidanceWeight;

            float speedMultiplier = 1f;
            if (distanceToTarget < finalApproachDistance)
            {
                speedMultiplier = Mathf.Lerp(
                    0.45f,
                    1f,
                    Mathf.InverseLerp(arrivalDistance, finalApproachDistance, distanceToTarget));
            }

            if (finalDirection.sqrMagnitude <= minMoveDistance * minMoveDistance)
            {
                return;
            }

            Vector3 movement = finalDirection.normalized * moveSpeed * speedMultiplier * Time.deltaTime;
            movement.y = 0f;

            if (_characterController != null && _characterController.enabled)
            {
                CharacterControllerMove(movement);
            }
            else
            {
                transform.position += movement;
            }

            transform.position = new Vector3(transform.position.x, currentY, transform.position.z);

            if (movement.sqrMagnitude > minMoveDistance * minMoveDistance)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 CalculateObstacleAvoidance(Vector3 currentDirection)
        {
            if (currentDirection.sqrMagnitude < minMoveDistance * minMoveDistance)
            {
                return Vector3.zero;
            }

            Vector3 avoidance = Vector3.zero;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            currentDirection.y = 0f;
            currentDirection.Normalize();

            int rayDivisor = Mathf.Max(obstacleRayCount - 1, 1);
            for (int i = 0; i < obstacleRayCount; i++)
            {
                float angle = -30f + (i / (float)rayDivisor) * 60f;
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 rayDirection = (rotation * currentDirection).normalized;

                if (!Physics.SphereCast(
                        rayOrigin,
                        obstacleAvoidanceRadius * 0.5f,
                        rayDirection,
                        out RaycastHit hit,
                        obstacleRayLength,
                        obstacleLayerMask))
                {
                    continue;
                }

                Obstacle obstacle = hit.collider.GetComponent<Obstacle>();
                if (obstacle == null)
                {
                    continue;
                }

                Vector3 awayFromObstacle = rayOrigin - hit.point;
                awayFromObstacle.y = 0f;
                if (awayFromObstacle.sqrMagnitude < 0.001f)
                {
                    awayFromObstacle = -rayDirection;
                }

                awayFromObstacle.Normalize();
                Vector3 tangent = Vector3.Cross(Vector3.up, awayFromObstacle).normalized;
                if (Vector3.Dot(tangent, currentDirection) < 0f)
                {
                    tangent = -tangent;
                }

                float distanceStrength = 1f - (hit.distance / obstacleRayLength);
                Vector3 steering = (awayFromObstacle * 0.55f) + (tangent * 0.85f);
                avoidance += steering * distanceStrength * obstacleAvoidanceStrength;

                Debug.DrawLine(transform.position, hit.point, Color.red, 0.1f);
            }

            return avoidance;
        }

        private void CharacterControllerMove(Vector3 movement)
        {
            if (_characterController == null || !_characterController.enabled)
            {
                return;
            }

            CollisionFlags flags = _characterController.Move(movement);
            if (flags == CollisionFlags.None)
            {
                return;
            }

            Vector3 slideDirection = movement;
            slideDirection.y = 0f;
            if (slideDirection.sqrMagnitude < minMoveDistance * minMoveDistance)
            {
                return;
            }

            slideDirection.Normalize();

            if (!Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.3f, slideDirection, out _, 0.5f, obstacleLayerMask))
            {
                return;
            }

            Vector3 rightDir = Vector3.Cross(slideDirection, Vector3.up).normalized;
            if (!_characterController.Raycast(new Ray(transform.position, rightDir), out _, 0.5f))
            {
                _characterController.Move(rightDir * movement.magnitude * 0.5f);
            }
            else if (!_characterController.Raycast(new Ray(transform.position, -rightDir), out _, 0.5f))
            {
                _characterController.Move(-rightDir * movement.magnitude * 0.5f);
            }
        }

        private void SnapToDestinationIfClose(Vector3 destination, float distanceToTarget)
        {
            if (distanceToTarget > snapToTargetDistance || _hasTarget)
            {
                return;
            }

            transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        }

        private Vector3 CalculateSeparation()
        {
            Vector3 separation = Vector3.zero;
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, separationDistance);

            foreach (Collider collider in nearbyColliders)
            {
                if (collider.gameObject == gameObject)
                {
                    continue;
                }

                UnitPathfinder otherUnit = collider.GetComponent<UnitPathfinder>();
                if (otherUnit == null || otherUnit == this)
                {
                    continue;
                }

                Vector3 away = transform.position - collider.transform.position;
                float distance = away.magnitude;
                if (distance <= 0.01f)
                {
                    continue;
                }

                float strength = (separationDistance - distance) / separationDistance;
                strength *= strength;
                separation += away.normalized * strength * separationStrength;
            }

            return separation;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(_targetPosition, 0.3f);

            if (drawDebugGizmos)
            {
                Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
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

            if (useSeparation)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawWireSphere(transform.position, separationDistance);
            }
        }
    }
}
