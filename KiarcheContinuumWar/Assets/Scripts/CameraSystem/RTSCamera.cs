using UnityEngine;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.CameraSystem
{
    /// <summary>
    /// RTS камера с панорамированием (WSAD) и зумом.
    /// </summary>
    public class RTSCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 50f;
        [SerializeField] private float defaultZoom = 30f;

        [Header("Rotation Settings")]
        [SerializeField] private float defaultRotationX = 45f;

        [Header("Bounds")]
        [SerializeField] private bool limitToMapBounds = true;
        [SerializeField] private float mapWidth = 100f;
        [SerializeField] private float mapHeight = 100f;
        [SerializeField] private float mapOriginX = -50f;
        [SerializeField] private float mapOriginZ = -50f;
        [SerializeField] private float boundsPadding = 2f;

        [Header("Smooth Follow")]
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 30, -10);

        // Состояние камеры
        private Vector3 _targetPosition;
        private float _currentZoom;
        private float _currentRotationX;
        private float _currentRotationY;

        // Singleton (опционально)
        private static RTSCamera _instance;
        public static RTSCamera Instance => _instance;

        public Vector3 CameraPosition => transform.position;
        public float CurrentZoom => _currentZoom;

        // Public свойства для границ
        public float MapWidth => mapWidth;
        public float MapHeight => mapHeight;
        public float MapOriginX => mapOriginX;
        public float MapOriginZ => mapOriginZ;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Инициализация позиции
            _targetPosition = transform.position;
            _currentZoom = defaultZoom;
            _currentRotationX = defaultRotationX;
            _currentRotationY = transform.eulerAngles.y;

            // Получить границы карты из MapManager (если есть)
            if (MapManager.Instance != null)
            {
                mapWidth = MapManager.Instance.MapWidth;
                mapHeight = MapManager.Instance.MapHeight;
                mapOriginX = MapManager.Instance.MapOrigin.x;
                mapOriginZ = MapManager.Instance.MapOrigin.z;
                Debug.Log($"[RTSCamera] Границы из MapManager: ({mapOriginX}, {mapOriginZ}) - ({mapOriginX + mapWidth}, {mapOriginZ + mapHeight})");
            }
            else
            {
                Debug.Log($"[RTSCamera] Границы по умолчанию: ({mapOriginX}, {mapOriginZ}) - ({mapOriginX + mapWidth}, {mapOriginZ + mapHeight})");
            }

            // Начальная позиция над центром карты (всегда на высоте zoom)
            Vector3 center = new Vector3(
                mapOriginX + mapWidth / 2,
                defaultZoom,
                mapOriginZ + mapHeight / 2
            );
            
            // Если камера в (0,0,0) или слишком низко — переместить в центр
            if (transform.position == Vector3.zero || transform.position.y < 5)
            {
                transform.position = center;
                _targetPosition = center;
            }
            
            // Установить правильный угол наклона
            transform.rotation = Quaternion.Euler(defaultRotationX, _currentRotationY, 0);
        }

        private void Update()
        {
            HandleInput();
            UpdateCameraPosition();
        }

        /// <summary>
        /// Обработка ввода (WSAD + зум).
        /// </summary>
        private void HandleInput()
        {
            // Панорамирование WSAD
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection += Vector3.right;
            }

            if (moveDirection.magnitude > 0.1f)
            {
                // Нормализовать
                moveDirection = moveDirection.normalized;
                
                // Для изометрической камеры: W = вверх экрана (forward + up), S = вниз
                // Движение относительно камеры, но только по XZ плоскости
                Vector3 camForward = transform.forward;
                camForward.y = 0;
                camForward.Normalize();
                
                Vector3 camRight = transform.right;
                camRight.y = 0;
                camRight.Normalize();
                
                Vector3 moveVector = (camForward * moveDirection.z) + (camRight * moveDirection.x);
                float zoomFactor = Mathf.Lerp(1.2f, 4.2f, Mathf.InverseLerp(minZoom, maxZoom, _currentZoom));
                _targetPosition += moveVector * moveSpeed * zoomFactor * Time.deltaTime;
            }

            // Зум колёсиком
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _currentZoom -= scroll * zoomSpeed;
                _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            }
        }

        /// <summary>
        /// Обновление позиции камеры с плавностью.
        /// </summary>
        private void UpdateCameraPosition()
        {
            // Ограничение границами карты
            if (limitToMapBounds)
            {
                _targetPosition.x = Mathf.Clamp(
                    _targetPosition.x,
                    mapOriginX + boundsPadding,
                    mapOriginX + mapWidth - boundsPadding
                );
                _targetPosition.z = Mathf.Clamp(
                    _targetPosition.z,
                    mapOriginZ + boundsPadding,
                    mapOriginZ + mapHeight - boundsPadding
                );
            }

            // Высота камеры (zoom)
            _targetPosition.y = _currentZoom;

            // Плавное движение к целевой позиции
            transform.position = Vector3.Lerp(
                transform.position,
                _targetPosition,
                smoothSpeed * Time.deltaTime
            );

            // Вращение камеры (изометрический вид)
            transform.rotation = Quaternion.Euler(
                _currentRotationX,
                _currentRotationY,
                0
            );
        }

        /// <summary>
        /// Установить целевую позицию для камеры (follow юнита).
        /// </summary>
        public void SetTarget(Vector3 target)
        {
            _targetPosition = new Vector3(
                target.x,
                _currentZoom,
                target.z
            );
        }

        /// <summary>
        /// Переместить камеру мгновенно.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
            _targetPosition = position;
        }

        /// <summary>
        /// Переместить камеру на спавн игрока.
        /// </summary>
        public void MoveToPlayerSpawn(int index = 0)
        {
            if (MapManager.Instance != null)
            {
                Vector3 spawnPoint = MapManager.Instance.GetPlayerSpawnPoint(index);
                SetTarget(spawnPoint + new Vector3(0, 10, 0));
            }
        }

        /// <summary>
        /// Отладочная визуализация.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Границы камеры
            if (limitToMapBounds)
            {
                Gizmos.color = Color.cyan;
                Vector3 center = new Vector3(
                    mapOriginX + mapWidth / 2,
                    0,
                    mapOriginZ + mapHeight / 2
                );
                Gizmos.DrawWireCube(center, new Vector3(mapWidth, 0, mapHeight));
            }

            // Текущая позиция
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1f);

            // Целевая позиция
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_targetPosition, 1f);

            // Линия к цели
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _targetPosition);
        }
    }
}
