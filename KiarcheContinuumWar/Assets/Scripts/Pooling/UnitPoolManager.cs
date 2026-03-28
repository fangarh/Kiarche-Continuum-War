using UnityEngine;
using KiarcheContinuumWar.Units;
using KiarcheContinuumWar.Pooling;

namespace KiarcheContinuumWar.Managers
{
    /// <summary>
    /// Менеджер пула юнитов.
    /// Управляет созданием и переиспользованием юнитов.
    /// </summary>
    public class UnitPoolManager : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private int initialPoolSize = 200;
        [SerializeField] private bool autoExpand = true;

        [Header("Debug")]
        [SerializeField] private int currentPoolSize;
        [SerializeField] private int activeUnitsCount;

        private ObjectPool<Unit> _unitPool;

        // Singleton
        private static UnitPoolManager _instance;
        public static UnitPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<UnitPoolManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("UnitPoolManager");
                        _instance = go.AddComponent<UnitPoolManager>();
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
        }

        private void Start()
        {
            if (unitPrefab == null)
            {
                Debug.LogWarning("[UnitPoolManager] Unit prefab не назначен! Создаю программно...");
                CreateDefaultPrefab();
            }

            // Инициализация пула
            _unitPool = new ObjectPool<Unit>(unitPrefab, initialPoolSize, transform, autoExpand);
            
            Debug.Log($"[UnitPoolManager] Пул инициализирован: {initialPoolSize} юнитов");
        }

        /// <summary>
        /// Создать юнита в указанной позиции.
        /// </summary>
        public Unit SpawnUnit(Vector3 position, Quaternion rotation, Color? color = null)
        {
            if (_unitPool == null)
            {
                Debug.LogError("[UnitPoolManager] Пул не инициализирован!");
                return null;
            }

            Unit unit = _unitPool.Get(position, rotation);

            // Настроить цвет если указан
            if (color.HasValue && unit != null)
            {
                SetUnitColor(unit, color.Value);
            }

            return unit;
        }

        /// <summary>
        /// Уничтожить юнита (вернуть в пул).
        /// </summary>
        public void DespawnUnit(Unit unit)
        {
            if (_unitPool == null || unit == null) return;

            _unitPool.Return(unit);
        }

        /// <summary>
        /// Уничтожить всех юнитов.
        /// </summary>
        public void DespawnAll()
        {
            if (_unitPool == null) return;

            _unitPool.ReturnAll();
        }

        /// <summary>
        /// Предзагрузить ещё юнитов в пул.
        /// </summary>
        public void PreloadUnits(int count)
        {
            if (_unitPool == null) return;

            _unitPool.Preload(count);
            Debug.Log($"[UnitPoolManager] Пре загружено {count} юнитов");
        }

        /// <summary>
        /// Очистить пул (полное уничтожение).
        /// </summary>
        public void ClearPool()
        {
            if (_unitPool == null) return;

            _unitPool.Clear();
            Debug.Log("[UnitPoolManager] Пул очищен");
        }

        /// <summary>
        /// Установить цвет юнита.
        /// </summary>
        private void SetUnitColor(Unit unit, Color color)
        {
            Renderer renderer = unit.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }

        /// <summary>
        /// Создать дефолтный префаб программно.
        /// </summary>
        private void CreateDefaultPrefab()
        {
            // Создать GameObject
            GameObject prefabObj = new GameObject("UnitPrefab");
            prefabObj.SetActive(false);

            // Добавить Unit компонент
            Unit unit = prefabObj.AddComponent<Unit>();

            // Добавить Collider (увеличенный для удобства выделения)
            CapsuleCollider collider = prefabObj.AddComponent<CapsuleCollider>();
            collider.radius = 0.8f;  // Увеличенный радиус для выделения
            collider.height = 2f;
            collider.center = new Vector3(0, 1, 0);

            // Добавить Rigidbody
            Rigidbody rb = prefabObj.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // Добавить UnitPathfinder
            prefabObj.AddComponent<UnitPathfinder>();

            // Визуализация
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(prefabObj.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            visual.name = "Visual";
            Destroy(visual.GetComponent<Collider>());

            // Создать префаб в Resources
            unitPrefab = prefabObj.GetComponent<Unit>();
            
            Debug.Log("[UnitPoolManager] Default prefab создан");
        }

        private void Update()
        {
            // Обновление статистики
            if (_unitPool != null)
            {
                currentPoolSize = _unitPool.TotalCount;
                activeUnitsCount = _unitPool.ActiveCount;
            }
        }

        // Публичный доступ к пулу для отладки
        public ObjectPool<Unit> UnitPool => _unitPool;
    }
}
