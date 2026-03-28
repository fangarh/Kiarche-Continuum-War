using UnityEngine;
using KiarcheContinuumWar.Units;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.InputSystem;

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

            if (unitPrefab == null)
            {
                UnityEngine.Debug.LogWarning("[GameManager] Unit prefab не назначен! Создаю программно...");
                CreateUnitPrefab();
                return;
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
            // Создать GameObject
            unitPrefab = new GameObject("UnitPrefab");
            
            // Добавить Unit компонент
            Unit unit = unitPrefab.AddComponent<Unit>();
            
            // Добавить Collider
            CapsuleCollider collider = unitPrefab.AddComponent<CapsuleCollider>();
            collider.radius = 0.5f;
            collider.height = 2f;
            collider.center = new Vector3(0, 1, 0);
            
            // Добавить Rigidbody
            Rigidbody rb = unitPrefab.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Визуализация
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.transform.SetParent(unitPrefab.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            visual.name = "Visual";
            Destroy(visual.GetComponent<Collider>());
            
            UnityEngine.Debug.Log("[GameManager] Unit prefab создан программно");
        }

        /// <summary>
        /// Создать тестовые юниты.
        /// </summary>
        private void SpawnTestUnits(Vector3 position, int count, Color color)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2;
                float radius = 3f;
                Vector3 spawnPos = position + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                GameObject unitObj = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
                
                // Настроить цвет (для визуального различия)
                Renderer renderer = unitObj.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }

            // Обновить список юнитов
            unitController?.FindAllUnits();
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
            int currentCount = FindObjectsByType<Unit>().Length;
            int toSpawn = targetCount - currentCount;

            if (toSpawn > 0)
            {
                UnityEngine.Debug.Log($"[GameManager] Создание {toSpawn} юнитов для теста...");
                SpawnTestUnits(Vector3.zero, toSpawn, Color.yellow);
                UnityEngine.Debug.Log($"[GameManager] Всего юнитов: {FindObjectsByType<Unit>().Length}");
            }
            else
            {
                UnityEngine.Debug.Log($"[GameManager] Уже есть {currentCount} юнитов (цель: {targetCount})");
            }
        }
    }
}
