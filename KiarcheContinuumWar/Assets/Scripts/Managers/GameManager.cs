using UnityEngine;
using KiarcheContinuumWar.Units;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.InputSystem;
using KiarcheContinuumWar.Pooling;

namespace KiarcheContinuumWar.Managers
{
    /// <summary>
    /// Главный менеджер игры для MVP прототипа.
    /// Инициализирует системы, управляет состоянием игры.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Systems")]
        public ResourceManager resourceManager;
        public UnitController unitController;
        public UnitPoolManager unitPoolManager;

        [Header("Test Settings")]
        public bool enableTestMode = true;
        public int testUnitCount = 20;
        public Vector3 spawnPosition1 = new Vector3(-10, 0, 0);
        public Vector3 spawnPosition2 = new Vector3(10, 0, 0);

        [Header("Unit Prefab")]
        public GameObject unitPrefab;

        private void Awake()
        {
            // Найти системы если не назначены
            if (resourceManager == null)
                resourceManager = FindAnyObjectByType<ResourceManager>();

            if (unitController == null)
                unitController = FindAnyObjectByType<UnitController>();
            
            if (unitPoolManager == null)
                unitPoolManager = FindAnyObjectByType<UnitPoolManager>();
        }

        private void Start()
        {
            UnityEngine.Debug.Log("[GameManager] Игра запущена");

            if (enableTestMode)
            {
                SetupTestScene();
            }
        }

        /// <summary>
        /// Настроить тестовую сцену.
        /// </summary>
        private void SetupTestScene()
        {
            UnityEngine.Debug.Log("[GameManager] Настройка тестовой сцены");

            // Проверить/создать менеджеры
            if (unitPoolManager == null)
            {
                unitPoolManager = FindAnyObjectByType<UnitPoolManager>();
                if (unitPoolManager == null)
                {
                    GameObject poolObj = new GameObject("UnitPoolManager");
                    unitPoolManager = poolObj.AddComponent<UnitPoolManager>();
                }
            }

            if (unitPrefab == null)
            {
                UnityEngine.Debug.LogWarning("[GameManager] Unit prefab не назначен! Создаю программно...");
                CreateUnitPrefab();
                // Не return! Продолжаем спавн юнитов
            }

            // Создать тестовые юниты
            SpawnTestUnits(spawnPosition1, testUnitCount, Color.blue);
            SpawnTestUnits(spawnPosition2, testUnitCount, Color.red);
        }

        /// <summary>
        /// Создать префаб юнита программно.
        /// </summary>
        private void CreateUnitPrefab()
        {
            Debug.Log("[GameManager] Создаю UnitPrefab...");
            
            // Создать GameObject
            unitPrefab = new GameObject("UnitPrefab");

            // Добавить Unit компонент
            Unit unit = unitPrefab.AddComponent<Unit>();
            Debug.Log($"[GameManager] Добавлен Unit компонент: {unit != null}");

            // Добавить Collider (увеличенный для удобства выделения)
            CapsuleCollider collider = unitPrefab.AddComponent<CapsuleCollider>();
            collider.radius = 0.8f;  // Увеличенный радиус для выделения
            collider.height = 2f;
            collider.center = new Vector3(0, 1, 0);

            // Добавить Rigidbody (для RTS юнитов используем кинематический)
            Rigidbody rb = unitPrefab.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            rb.isKinematic = true; // RTS юниты не подвержены гравитации

            // Добавить UnitPathfinder (обязательно для движения!)
            var pathfinder = unitPrefab.AddComponent<UnitPathfinder>();
            Debug.Log($"[GameManager] Добавлен UnitPathfinder: {pathfinder != null}");

            // Визуализация
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(unitPrefab.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            visual.name = "Visual";
            Destroy(visual.GetComponent<Collider>());

            // Установить цвет по умолчанию
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }

            unitPrefab.SetActive(true); // Активировать префаб перед спавном
            Debug.Log("[GameManager] Unit prefab создан");
        }

        /// <summary>
        /// Создать тестовые юниты.
        /// </summary>
        private void SpawnTestUnits(Vector3 position, int count, Color color)
        {
            Debug.Log($"[GameManager] Спавн {count} юнитов в {position}");
            
            // Получить высоту terrain для спавна
            float terrainHeight = GetTerrainHeight(position);
            Debug.Log($"[GameManager] Высота terrain: {terrainHeight}");
            
            int spawnedCount = 0;
            
            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2;
                float radius = 3f;
                Vector3 spawnPos = position + new Vector3(
                    Mathf.Cos(angle) * radius,
                    terrainHeight + 1f,  // Спавн НАД поверхностью terrain (+1 чтобы не проваливались)
                    Mathf.Sin(angle) * radius
                );

                // Использовать Instantiate вместо пула (для надёжности)
                if (unitPrefab != null)
                {
                    GameObject unitObj = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
                    unitObj.SetActive(true);

                    // Настроить цвет (для визуального различия)
                    Renderer renderer = unitObj.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = color;
                    }
                    spawnedCount++;
                }
            }

            Debug.Log($"[GameManager] Успешно заспавнено {spawnedCount} из {count} юнитов ({color})");

            // Обновить список юнитов
            if (unitController != null)
            {
                unitController.FindAllUnits();
                Debug.Log($"[GameManager] Юнитов на сцене: {unitController.SelectedUnits.Count}");
            }
        }

        /// <summary>
        /// Получить высоту terrain в точке.
        /// </summary>
        private float GetTerrainHeight(Vector3 position)
        {
            Terrain terrain = FindAnyObjectByType<Terrain>();
            if (terrain == null) return 0;

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            float normalizedX = (position.x - terrainPos.x) / terrainData.size.x;
            float normalizedZ = (position.z - terrainPos.z) / terrainData.size.z;
            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedZ = Mathf.Clamp01(normalizedZ);

            return terrainPos.y + terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
        }

        private void Update()
        {
            // Тестовые команды
            if (enableTestMode)
            {
                HandleTestInput();
            }
        }

        private void HandleTestInput()
        {
            // Клавиша T - создать ещё юнитов
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnTestUnits(Vector3.zero, 10, Color.green);
                Debug.Log("[GameManager] Создано 10 тестовых юнитов");
            }

            // Клавиша P - тест производительности
            if (Input.GetKeyDown(KeyCode.P))
            {
                RunPerformanceTest();
            }

            // Клавиша R - показать ресурсы
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log($"[GameManager] Ресурсы: M={resourceManager.Materials}, E={resourceManager.Energy}, F={resourceManager.Food}, K={resourceManager.Knowledge}");
            }
        }

        /// <summary>
        /// Запустить тест производительности.
        /// </summary>
        private void RunPerformanceTest()
        {
            UnityEngine.Debug.Log("[GameManager] Запуск теста производительности...");

            int targetCount = 400;
            int currentCount = FindObjectsByType<Unit>(FindObjectsInactive.Include).Length;
            int toSpawn = targetCount - currentCount;

            if (toSpawn > 0)
            {
                UnityEngine.Debug.Log($"[GameManager] Создание {toSpawn} юнитов для теста...");
                SpawnTestUnits(Vector3.zero, toSpawn, Color.yellow);
                
                // Запустить таймер для замера FPS
                Invoke(nameof(ReportPerformance), 2f);
            }
            else
            {
                UnityEngine.Debug.Log($"[GameManager] Уже есть {currentCount} юнитов (цель: {targetCount})");
            }
        }

        /// <summary>
        /// Отчёт о производительности.
        /// </summary>
        private void ReportPerformance()
        {
            int fps = Mathf.RoundToInt(1.0f / Time.deltaTime);
            int unitCount = FindObjectsByType<Unit>(FindObjectsInactive.Include).Length;
            UnityEngine.Debug.Log($"[Performance] Юнитов: {unitCount}, FPS: {fps}");
        }
    }
}
