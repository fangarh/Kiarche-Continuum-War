using UnityEngine;

namespace KiarcheContinuumWar.Map
{
    /// <summary>
    /// Менеджер карты.
    /// Хранит данные о карте: размер, границы, спавн-поинты.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("Map Settings")]
        [SerializeField] private Vector2 mapSize = new Vector2(100, 100);
        [SerializeField] private Vector3 mapOrigin = Vector3.zero;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] playerSpawnPoints;
        [SerializeField] private Transform[] enemySpawnPoints;

        [Header("References")]
        [SerializeField] private Terrain terrain;

        // Singleton
        private static MapManager _instance;
        public static MapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<MapManager>();
                }
                return _instance;
            }
        }

        // Public свойства
        public Vector2 MapSize => mapSize;
        public Vector3 MapOrigin => mapOrigin;
        public float MapWidth => mapSize.x;
        public float MapHeight => mapSize.y;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            // Инициализация карты
            if (terrain == null)
            {
                terrain = GetComponent<Terrain>();
            }

            // Настройка origin если не задан
            if (mapOrigin == Vector3.zero && terrain != null)
            {
                mapOrigin = terrain.transform.position;
            }
        }

        /// <summary>
        /// Проверка, находится ли позиция в границах карты.
        /// </summary>
        public bool IsWithinBounds(Vector3 position)
        {
            return position.x >= mapOrigin.x &&
                   position.x <= mapOrigin.x + mapSize.x &&
                   position.z >= mapOrigin.z &&
                   position.z <= mapOrigin.z + mapSize.y;
        }

        /// <summary>
        /// Ограничить позицию границами карты.
        /// </summary>
        public Vector3 ClampToBounds(Vector3 position)
        {
            return new Vector3(
                Mathf.Clamp(position.x, mapOrigin.x, mapOrigin.x + mapSize.x),
                position.y,
                Mathf.Clamp(position.z, mapOrigin.z, mapOrigin.z + mapSize.y)
            );
        }

        /// <summary>
        /// Получить высоту ландшафта в точке.
        /// </summary>
        public float GetTerrainHeight(Vector3 position)
        {
            if (terrain == null) return 0;

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            float normalizedX = (position.x - terrainPos.x) / terrainData.size.x;
            float normalizedZ = (position.z - terrainPos.z) / terrainData.size.z;

            return terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
        }

        /// <summary>
        /// Получить позицию с учётом высоты ландшафта.
        /// </summary>
        public Vector3 GetPositionOnTerrain(float x, float z)
        {
            float y = GetTerrainHeight(new Vector3(x, 0, z));
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Получить случайную позицию на карте.
        /// </summary>
        public Vector3 GetRandomPosition()
        {
            float x = Random.Range(mapOrigin.x, mapOrigin.x + mapSize.x);
            float z = Random.Range(mapOrigin.z, mapOrigin.z + mapSize.y);
            return GetPositionOnTerrain(x, z);
        }

        /// <summary>
        /// Получить спавн-поинт игрока по индексу.
        /// </summary>
        public Vector3 GetPlayerSpawnPoint(int index = 0)
        {
            if (playerSpawnPoints == null || playerSpawnPoints.Length == 0)
            {
                return GetRandomPosition();
            }
            
            index = Mathf.Clamp(index, 0, playerSpawnPoints.Length - 1);
            return playerSpawnPoints[index].position;
        }

        /// <summary>
        /// Получить спавн-поинт врага по индексу.
        /// </summary>
        public Vector3 GetEnemySpawnPoint(int index = 0)
        {
            if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
            {
                // Если нет вражеских спавнов, возвращаем противоположную сторону
                return GetRandomPosition() + new Vector3(mapSize.x / 2, 0, mapSize.y / 2);
            }
            
            index = Mathf.Clamp(index, 0, enemySpawnPoints.Length - 1);
            return enemySpawnPoints[index].position;
        }

        /// <summary>
        /// Отладочная визуализация границ карты.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Границы карты
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector3(mapOrigin.x, 0, mapOrigin.z),
                new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.z)
            );
            Gizmos.DrawLine(
                new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.z),
                new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.z + mapSize.y)
            );
            Gizmos.DrawLine(
                new Vector3(mapOrigin.x + mapSize.x, 0, mapOrigin.z + mapSize.y),
                new Vector3(mapOrigin.x, 0, mapOrigin.z + mapSize.y)
            );
            Gizmos.DrawLine(
                new Vector3(mapOrigin.x, 0, mapOrigin.z + mapSize.y),
                new Vector3(mapOrigin.x, 0, mapOrigin.z)
            );

            // Спавн-поинты
            if (playerSpawnPoints != null)
            {
                foreach (var spawn in playerSpawnPoints)
                {
                    if (spawn == null) continue;
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(spawn.position, 2f);
                }
            }

            if (enemySpawnPoints != null)
            {
                foreach (var spawn in enemySpawnPoints)
                {
                    if (spawn == null) continue;
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(spawn.position, 2f);
                }
            }
        }
    }
}
