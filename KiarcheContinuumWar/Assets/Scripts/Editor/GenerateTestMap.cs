#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Editor
{
    /// <summary>
    /// Editor скрипт для генерации тестовой карты.
    /// </summary>
    public class GenerateTestMap
    {
        [MenuItem("Tools/KCW/Generate Test Map")]
        public static void GenerateTestMapMenu()
        {
            // Проверка: не запущен ли Play Mode
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog(
                    "Play Mode Active",
                    "Please stop Play Mode before generating a new map.\n\nThen run Tools → KCW → Generate Test Map again.",
                    "OK"
                );
                return;
            }

            // Создать новую сцену
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single
            );

            // Создать освещение
            CreateLighting();

            // Создать ландшафт
            CreateTerrain();

            // Создать препятствия
            CreateObstacles();

            // Создать спавн-поинты
            CreateSpawnPoints();

            // Создать менеджеры
            CreateMapManager();
            CreateGameManager();
            CreateUnitController();
            CreateRTSInput();

            // Настроить камеру
            SetupCamera();

            // Сохранить сцену
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Test Map",
                "TestMap",
                "unity",
                "Save the test map scene"
            );

            if (!string.IsNullOrEmpty(path))
            {
                EditorSceneManager.SaveScene(scene, path);
                AssetDatabase.SaveAssets();
                
                Debug.Log("[GenerateTestMap] Карта создана: " + path);
                
                // Открыть сцену
                if (EditorUtility.DisplayDialog(
                    "Test Map Generated",
                    "Test map created successfully!\n\nOpen the scene?",
                    "Open",
                    "Cancel"))
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
        }

        /// <summary>
        /// Создать ландшафт.
        /// </summary>
        private static void CreateTerrain()
        {
            // Создать GameObject для Terrain
            GameObject terrainObj = new GameObject("Terrain");
            Terrain terrain = terrainObj.AddComponent<Terrain>();
            TerrainData terrainData = new TerrainData();
            terrainData.size = new Vector3(100, 50, 100);
            
            // Важно: установить размер heightmap перед SetHeights
            int heightmapResolution = 129;
            terrainData.heightmapResolution = heightmapResolution;
            
            terrain.terrainData = terrainData;
            terrainObj.transform.position = new Vector3(-50, 0, -50);

            // Настройки рендеринга
            terrain.drawHeightmap = true;
            
            // Создать TerrainLayer (без текстуры, только настройки)
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.tileSize = new Vector2(10, 10);
            terrain.terrainData.terrainLayers = new TerrainLayer[] { terrainLayer };
            
            // Создать простой цветной материал для Terrain (Unlit - работает без текстур)
            Shader unlitShader = Shader.Find("Unlit/Color");
            if (unlitShader != null)
            {
                Material terrainMaterial = new Material(unlitShader);
                terrainMaterial.color = new Color(0.2f, 0.4f, 0.15f); // Тёмно-зелёный
                terrain.materialTemplate = terrainMaterial;
                Debug.Log("[GenerateTestMap] Материал Terrain создан (Unlit/Color shader)");
            }
            else
            {
                Debug.LogWarning("[GenerateTestMap] Unlit/Color shader не найден! Terrain может быть розовым");
            }

            // Сгенерировать heightmap (холмы)
            float[,] heights = new float[heightmapResolution, heightmapResolution];
            
            for (int y = 0; y < heightmapResolution; y++)
            {
                for (int x = 0; x < heightmapResolution; x++)
                {
                    float nx = x / (float)(heightmapResolution - 1) * 3;
                    float ny = y / (float)(heightmapResolution - 1) * 3;
                    float height = Mathf.PerlinNoise(nx, ny);
                    height = Mathf.Pow(height, 2);
                    heights[y, x] = height * 0.3f;
                }
            }
            
            terrain.terrainData.SetHeights(0, 0, heights);
            
            // Добавить Terrain Collider для физики
            TerrainCollider terrainCollider = terrainObj.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainData;
            
            Debug.Log($"[GenerateTestMap] Terrain создан: 100x100, heightmap {heightmapResolution}x{heightmapResolution}, холмы до 15 единиц");
            Debug.Log("[GenerateTestMap] TerrainCollider добавлен");
            Debug.Log("[GenerateTestMap] TerrainLayer добавлен");
        }

        /// <summary>
        /// Создать препятствия.
        /// </summary>
        private static void CreateObstacles()
        {
            // Создать родительский объект
            GameObject obstaclesParent = new GameObject("Obstacles");
            
            // Препятствия разных типов
            CreateObstacle(obstaclesParent.transform, new Vector3(-30, 0, -30), 3f, "Rock_1");
            CreateObstacle(obstaclesParent.transform, new Vector3(30, 0, -30), 4f, "Rock_2");
            CreateObstacle(obstaclesParent.transform, new Vector3(-30, 0, 30), 2.5f, "Rock_3");
            CreateObstacle(obstaclesParent.transform, new Vector3(30, 0, 30), 3.5f, "Rock_4");
            CreateObstacle(obstaclesParent.transform, new Vector3(0, 0, 0), 5f, "Central_Rock");
            
            // Дополнительные препятствия
            CreateObstacle(obstaclesParent.transform, new Vector3(-15, 0, 20), 2f, "Rock_5");
            CreateObstacle(obstaclesParent.transform, new Vector3(15, 0, -20), 2.5f, "Rock_6");
            CreateObstacle(obstaclesParent.transform, new Vector3(-20, 0, -10), 3f, "Rock_7");
            CreateObstacle(obstaclesParent.transform, new Vector3(20, 0, 10), 2f, "Rock_8");
            
            Debug.Log("[GenerateTestMap] Создано 9 препятствий");
        }

        /// <summary>
        /// Создать одно препятствие.
        /// </summary>
        private static void CreateObstacle(Transform parent, Vector3 position, float radius, string name)
        {
            // Получить высоту terrain
            float terrainHeight = GetTerrainHeight(position);
            
            // Создать GameObject
            GameObject obstacleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obstacleObj.name = name;
            obstacleObj.transform.parent = parent;
            
            // Масштабировать под радиус
            float height = Random.Range(3f, 8f);
            obstacleObj.transform.localScale = new Vector3(
                radius * 2,
                height,
                radius * 2
            );
            
            // Установить позицию на поверхности terrain
            obstacleObj.transform.position = new Vector3(
                position.x,
                terrainHeight + height / 2,
                position.z
            );

            // Добавить компонент Obstacle
            Obstacle obstacle = obstacleObj.AddComponent<Obstacle>();
            obstacle.SetObstacleRadius(radius);

            // Настроить collider для юнитов
            Collider collider = obstacleObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }

        /// <summary>
        /// Создать спавн-поинты.
        /// </summary>
        private static void CreateSpawnPoints()
        {
            GameObject spawnsParent = new GameObject("SpawnPoints");
            
            // Игрок (синий угол)
            CreateSpawnPoint(spawnsParent.transform, new Vector3(-40, 5, -40), "PlayerSpawn_1", true);
            CreateSpawnPoint(spawnsParent.transform, new Vector3(-30, 5, -30), "PlayerSpawn_2", true);
            
            // Враг (красный угол)
            CreateSpawnPoint(spawnsParent.transform, new Vector3(40, 5, 40), "EnemySpawn_1", false);
            CreateSpawnPoint(spawnsParent.transform, new Vector3(30, 5, 30), "EnemySpawn_2", false);
            
            Debug.Log("[GenerateTestMap] Создано 4 спавн-поинта");
        }

        /// <summary>
        /// Создать спавн-поинт.
        /// </summary>
        private static void CreateSpawnPoint(Transform parent, Vector3 position, string name, bool isPlayer)
        {
            GameObject spawnObj = new GameObject(name);
            spawnObj.transform.parent = parent;
            spawnObj.transform.position = position;
            
            // Визуализация
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.parent = spawnObj.transform;
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = Vector3.one * 2f;
            Collider markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                markerCollider.enabled = false;
            }
            
            // Цвет через sharedMaterial (чтобы не создавать инстанс в редакторе)
            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = isPlayer ? Color.blue : Color.red;
            }
        }

        /// <summary>
        /// Создать MapManager.
        /// </summary>
        private static void CreateMapManager()
        {
            GameObject managerObj = new GameObject("MapManager");
            MapManager mapManager = managerObj.AddComponent<MapManager>();

            // Настроить параметры
            SerializedObject serializedObject = new SerializedObject(mapManager);
            serializedObject.FindProperty("mapSize").vector2Value = new Vector2(100, 100);
            serializedObject.FindProperty("mapOrigin").vector3Value = new Vector3(-50, 0, -50);
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("[GenerateTestMap] MapManager создан: mapSize=(100, 100), mapOrigin=(-50, 0, -50)");

            // Найти Terrain
            Terrain terrain = UnityEngine.Object.FindAnyObjectByType<Terrain>();
            if (terrain != null)
            {
                serializedObject = new SerializedObject(mapManager);
                serializedObject.FindProperty("terrain").objectReferenceValue = terrain;
                serializedObject.ApplyModifiedProperties();
            }
            
            // Найти спавн-поинты по имени
            GameObject playerSpawn1 = GameObject.Find("PlayerSpawn_1");
            GameObject playerSpawn2 = GameObject.Find("PlayerSpawn_2");
            GameObject enemySpawn1 = GameObject.Find("EnemySpawn_1");
            GameObject enemySpawn2 = GameObject.Find("EnemySpawn_2");
            
            if (playerSpawn1 != null || playerSpawn2 != null || enemySpawn1 != null || enemySpawn2 != null)
            {
                serializedObject = new SerializedObject(mapManager);
                
                var playerSpawns = new Transform[2];
                var enemySpawns = new Transform[2];
                
                if (playerSpawn1 != null) playerSpawns[0] = playerSpawn1.transform;
                if (playerSpawn2 != null) playerSpawns[1] = playerSpawn2.transform;
                if (enemySpawn1 != null) enemySpawns[0] = enemySpawn1.transform;
                if (enemySpawn2 != null) enemySpawns[1] = enemySpawn2.transform;
                
                SerializedProperty playerProp = serializedObject.FindProperty("playerSpawnPoints");
                playerProp.arraySize = playerSpawns.Length;
                for (int i = 0; i < playerSpawns.Length; i++)
                {
                    if (playerSpawns[i] != null)
                    {
                        playerProp.GetArrayElementAtIndex(i).objectReferenceValue = playerSpawns[i].gameObject;
                    }
                }
                
                SerializedProperty enemyProp = serializedObject.FindProperty("enemySpawnPoints");
                enemyProp.arraySize = enemySpawns.Length;
                for (int i = 0; i < enemySpawns.Length; i++)
                {
                    if (enemySpawns[i] != null)
                    {
                        enemyProp.GetArrayElementAtIndex(i).objectReferenceValue = enemySpawns[i].gameObject;
                    }
                }
                
                serializedObject.ApplyModifiedProperties();
            }
            
            Debug.Log("[GenerateTestMap] MapManager создан");
        }

        /// <summary>
        /// Настроить камеру.
        /// </summary>
        private static void SetupCamera()
        {
            // Найти Main Camera
            Camera mainCamera = UnityEngine.Object.FindAnyObjectByType<Camera>();
            
            if (mainCamera == null)
            {
                // Создать камеру
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
            }
            
            // Добавить RTSCamera
            if (mainCamera.GetComponent<KiarcheContinuumWar.CameraSystem.RTSCamera>() == null)
            {
                mainCamera.gameObject.AddComponent<KiarcheContinuumWar.CameraSystem.RTSCamera>();
            }
            
            // Установить позицию камеры над центром карты (0, 0) с высотой 40
            Vector3 center = new Vector3(0, 40, 0);
            mainCamera.transform.position = center;
            mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
            
            // Настроить clipping planes
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.farClipPlane = 1000f;
            
            // Настроить фон (тёмно-синий)
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            
            Debug.Log("[GenerateTestMap] Камера настроена: позиция (0, 40, 0), угол 45°");
        }

        /// <summary>
        /// Получить высоту terrain в точке.
        /// </summary>
        private static float GetTerrainHeight(Vector3 position)
        {
            Terrain terrain = UnityEngine.Object.FindAnyObjectByType<Terrain>();
            if (terrain == null) return 0;

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            // Нормализованные координаты (0-1)
            float normalizedX = (position.x - terrainPos.x) / terrainData.size.x;
            float normalizedZ = (position.z - terrainPos.z) / terrainData.size.z;

            // Ограничить [0, 1]
            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedZ = Mathf.Clamp01(normalizedZ);

            float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
            
            // Вернуть мировую высоту (terrainPos.y + высота)
            return terrainPos.y + height;
        }

        /// <summary>
        /// Создать освещение.
        /// </summary>
        private static void CreateLighting()
        {
            // Создать Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            
            // Настроить Skybox и ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.5f;
            RenderSettings.skybox = null; // Убрать skybox (будет синий фон)
            
            Debug.Log("[GenerateTestMap] Освещение создано");
        }

        /// <summary>
        /// Создать GameManager.
        /// </summary>
        private static void CreateGameManager()
        {
            GameObject managerObj = new GameObject("GameManager");
            var gameManager = managerObj.AddComponent<KiarcheContinuumWar.Managers.GameManager>();
            
            // Настроить параметры
            SerializedObject serializedObject = new SerializedObject(gameManager);
            serializedObject.FindProperty("enableTestMode").boolValue = true;
            serializedObject.FindProperty("testUnitCount").intValue = 20;
            serializedObject.FindProperty("spawnPosition1").vector3Value = new Vector3(-10, 0, 0);
            serializedObject.FindProperty("spawnPosition2").vector3Value = new Vector3(10, 0, 0);
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("[GenerateTestMap] GameManager создан");
        }

        /// <summary>
        /// Создать UnitController.
        /// </summary>
        private static void CreateUnitController()
        {
            GameObject controllerObj = new GameObject("UnitController");
            controllerObj.AddComponent<KiarcheContinuumWar.Units.UnitController>();
            Debug.Log("[GenerateTestMap] UnitController создан");
        }

        /// <summary>
        /// Создать RTSInput.
        /// </summary>
        private static void CreateRTSInput()
        {
            GameObject inputObj = new GameObject("RTSInput");
            var rtsInput = inputObj.AddComponent<KiarcheContinuumWar.InputSystem.RTSInput>();
            
            // Назначить камеру
            Camera mainCamera = UnityEngine.Object.FindAnyObjectByType<Camera>();
            if (mainCamera != null)
            {
                SerializedObject serializedObject = new SerializedObject(rtsInput);
                serializedObject.FindProperty("mainCamera").objectReferenceValue = mainCamera;
                serializedObject.ApplyModifiedProperties();
            }
            
            Debug.Log("[GenerateTestMap] RTSInput создан");
        }
    }
}
#endif
