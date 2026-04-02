#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Editor
{
    /// <summary>
    /// Editor script для генерации тестовой карты.
    /// </summary>
    public class GenerateTestMap
    {
        [MenuItem("Tools/KCW/Generate Test Map")]
        public static void GenerateTestMapMenu()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog(
                    "Play Mode Active",
                    "Please stop Play Mode before generating a new map.\n\nThen run Tools -> KCW -> Generate Test Map again.",
                    "OK");
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateLighting();
            CreateTerrain();
            CreateObstacles();
            CreateSpawnPoints();
            CreateMapManager();
            CreateFlowFieldManager();
            CreateGameManager();
            CreateUnitController();
            CreateRTSInput();
            SetupCamera();

            string path = EditorUtility.SaveFilePanelInProject(
                "Save Test Map",
                "TestMap",
                "unity",
                "Save the test map scene");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            EditorSceneManager.SaveScene(scene, path);
            AssetDatabase.SaveAssets();
            Debug.Log("[GenerateTestMap] Карта создана: " + path);

            if (EditorUtility.DisplayDialog("Test Map Generated", "Test map created successfully!\n\nOpen the scene?", "Open", "Cancel"))
            {
                EditorSceneManager.OpenScene(path);
            }
        }

        private static void CreateTerrain()
        {
            GameObject terrainObj = new GameObject("Terrain");
            Terrain terrain = terrainObj.AddComponent<Terrain>();
            TerrainData terrainData = new TerrainData
            {
                size = new Vector3(100f, 50f, 100f),
                heightmapResolution = 129
            };

            terrain.terrainData = terrainData;
            terrainObj.transform.position = new Vector3(-50f, 0f, -50f);
            terrain.drawHeightmap = true;

            TerrainLayer terrainLayer = new TerrainLayer
            {
                tileSize = new Vector2(10f, 10f)
            };
            terrain.terrainData.terrainLayers = new[] { terrainLayer };

            Shader unlitShader = Shader.Find("Unlit/Color");
            if (unlitShader != null)
            {
                Material terrainMaterial = new Material(unlitShader);
                terrainMaterial.color = new Color(0.2f, 0.4f, 0.15f);
                terrain.materialTemplate = terrainMaterial;
            }

            float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    float nx = x / (float)(terrainData.heightmapResolution - 1);
                    float ny = y / (float)(terrainData.heightmapResolution - 1);
                    float edgeFalloff = Mathf.Clamp01(Mathf.Max(Mathf.Abs(nx - 0.5f), Mathf.Abs(ny - 0.5f)) * 2f);
                    float macroNoise = Mathf.PerlinNoise(nx * 2.2f, ny * 2.2f);
                    float detailNoise = Mathf.PerlinNoise(nx * 6f + 13f, ny * 6f + 7f);
                    heights[y, x] = Mathf.Lerp(0.02f, macroNoise * 0.18f + detailNoise * 0.04f, edgeFalloff);
                }
            }

            terrain.terrainData.SetHeights(0, 0, heights);

            TerrainCollider terrainCollider = terrainObj.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainData;

            Debug.Log("[GenerateTestMap] Terrain создан: плоский центр и более шумные края для тестов");
        }

        private static void CreateObstacles()
        {
            GameObject obstaclesParent = new GameObject("Obstacles");

            CreateObstacleGroupMarker("OpenField", new Vector3(-34f, 0f, 0f), obstaclesParent.transform);
            CreateObstacleGroupMarker("Slalom", new Vector3(0f, 0f, -26f), obstaclesParent.transform);
            CreateObstacleGroupMarker("Chokepoint", new Vector3(26f, 0f, 0f), obstaclesParent.transform);

            CreateObstacle(obstaclesParent.transform, new Vector3(-10f, 0f, -20f), 2.0f, "Slalom_1");
            CreateObstacle(obstaclesParent.transform, new Vector3(0f, 0f, -14f), 2.4f, "Slalom_2");
            CreateObstacle(obstaclesParent.transform, new Vector3(-8f, 0f, -6f), 2.1f, "Slalom_3");
            CreateObstacle(obstaclesParent.transform, new Vector3(8f, 0f, 4f), 2.3f, "Slalom_4");
            CreateObstacle(obstaclesParent.transform, new Vector3(-4f, 0f, 14f), 2.6f, "Slalom_5");

            CreateObstacle(obstaclesParent.transform, new Vector3(18f, 0f, -16f), 3.5f, "Gate_Left_1");
            CreateObstacle(obstaclesParent.transform, new Vector3(33f, 0f, -16f), 3.5f, "Gate_Right_1");
            CreateObstacle(obstaclesParent.transform, new Vector3(18f, 0f, 0f), 3.5f, "Gate_Left_2");
            CreateObstacle(obstaclesParent.transform, new Vector3(33f, 0f, 0f), 3.5f, "Gate_Right_2");
            CreateObstacle(obstaclesParent.transform, new Vector3(18f, 0f, 16f), 3.5f, "Gate_Left_3");
            CreateObstacle(obstaclesParent.transform, new Vector3(33f, 0f, 16f), 3.5f, "Gate_Right_3");

            CreateObstacle(obstaclesParent.transform, new Vector3(-28f, 0f, -24f), 2.8f, "Corner_1");
            CreateObstacle(obstaclesParent.transform, new Vector3(-32f, 0f, 24f), 3.2f, "Corner_2");
            CreateObstacle(obstaclesParent.transform, new Vector3(40f, 0f, 28f), 4.0f, "Corner_3");

            Debug.Log("[GenerateTestMap] Созданы зоны для тестов: открытое поле, слалом и узкие проходы");
        }

        private static void CreateObstacle(Transform parent, Vector3 position, float radius, string name)
        {
            float terrainHeight = GetTerrainHeight(position);

            GameObject obstacleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obstacleObj.name = name;
            obstacleObj.transform.parent = parent;

            float height = Random.Range(3f, 8f);
            obstacleObj.transform.localScale = new Vector3(radius * 2f, height, radius * 2f);
            obstacleObj.transform.position = new Vector3(position.x, terrainHeight + height / 2f, position.z);

            Obstacle obstacle = obstacleObj.AddComponent<Obstacle>();
            obstacle.SetObstacleRadius(radius);

            Collider collider = obstacleObj.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }

        private static void CreateSpawnPoints()
        {
            GameObject spawnsParent = new GameObject("SpawnPoints");

            CreateSpawnPoint(spawnsParent.transform, new Vector3(-40f, 5f, -8f), "PlayerSpawn_1", true);
            CreateSpawnPoint(spawnsParent.transform, new Vector3(-34f, 5f, 8f), "PlayerSpawn_2", true);
            CreateSpawnPoint(spawnsParent.transform, new Vector3(40f, 5f, -8f), "EnemySpawn_1", false);
            CreateSpawnPoint(spawnsParent.transform, new Vector3(34f, 5f, 8f), "EnemySpawn_2", false);

            Debug.Log("[GenerateTestMap] Спавны поставлены напротив тестовых полос");
        }

        private static void CreateSpawnPoint(Transform parent, Vector3 position, string name, bool isPlayer)
        {
            GameObject spawnObj = new GameObject(name);
            spawnObj.transform.parent = parent;
            spawnObj.transform.position = position;

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.parent = spawnObj.transform;
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = Vector3.one * 2f;

            Collider markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                markerCollider.enabled = false;
            }

            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = isPlayer ? Color.blue : Color.red;
            }
        }

        private static void CreateMapManager()
        {
            GameObject managerObj = new GameObject("MapManager");
            MapManager mapManager = managerObj.AddComponent<MapManager>();

            SerializedObject serializedObject = new SerializedObject(mapManager);
            serializedObject.FindProperty("mapSize").vector2Value = new Vector2(100f, 100f);
            serializedObject.FindProperty("mapOrigin").vector3Value = new Vector3(-50f, 0f, -50f);
            serializedObject.ApplyModifiedProperties();

            Terrain terrain = Object.FindAnyObjectByType<Terrain>();
            if (terrain != null)
            {
                serializedObject = new SerializedObject(mapManager);
                serializedObject.FindProperty("terrain").objectReferenceValue = terrain;
                serializedObject.ApplyModifiedProperties();
            }

            GameObject playerSpawn1 = GameObject.Find("PlayerSpawn_1");
            GameObject playerSpawn2 = GameObject.Find("PlayerSpawn_2");
            GameObject enemySpawn1 = GameObject.Find("EnemySpawn_1");
            GameObject enemySpawn2 = GameObject.Find("EnemySpawn_2");

            serializedObject = new SerializedObject(mapManager);
            SerializedProperty playerProp = serializedObject.FindProperty("playerSpawnPoints");
            playerProp.arraySize = 2;
            if (playerSpawn1 != null) playerProp.GetArrayElementAtIndex(0).objectReferenceValue = playerSpawn1.transform;
            if (playerSpawn2 != null) playerProp.GetArrayElementAtIndex(1).objectReferenceValue = playerSpawn2.transform;

            SerializedProperty enemyProp = serializedObject.FindProperty("enemySpawnPoints");
            enemyProp.arraySize = 2;
            if (enemySpawn1 != null) enemyProp.GetArrayElementAtIndex(0).objectReferenceValue = enemySpawn1.transform;
            if (enemySpawn2 != null) enemyProp.GetArrayElementAtIndex(1).objectReferenceValue = enemySpawn2.transform;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[GenerateTestMap] MapManager создан");
        }

        private static void CreateFlowFieldManager()
        {
            GameObject flowFieldObj = new GameObject("FlowFieldManager");
            flowFieldObj.AddComponent<KiarcheContinuumWar.Pathfinding.FlowFieldManager>();
            Debug.Log("[GenerateTestMap] FlowFieldManager создан");
        }

        private static void SetupCamera()
        {
            Camera mainCamera = Object.FindAnyObjectByType<Camera>();
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.AddComponent<AudioListener>();
            }

            if (mainCamera.GetComponent<KiarcheContinuumWar.CameraSystem.RTSCamera>() == null)
            {
                mainCamera.gameObject.AddComponent<KiarcheContinuumWar.CameraSystem.RTSCamera>();
            }

            mainCamera.transform.position = new Vector3(0f, 42f, -18f);
            mainCamera.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.farClipPlane = 1000f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;

            Debug.Log("[GenerateTestMap] Камера настроена для обзора центральных тестовых зон");
        }

        private static float GetTerrainHeight(Vector3 position)
        {
            Terrain terrain = Object.FindAnyObjectByType<Terrain>();
            if (terrain == null)
            {
                return 0f;
            }

            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            float normalizedX = Mathf.Clamp01((position.x - terrainPos.x) / terrainData.size.x);
            float normalizedZ = Mathf.Clamp01((position.z - terrainPos.z) / terrainData.size.z);

            return terrainPos.y + terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
        }

        private static void CreateLighting()
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.5f;
            RenderSettings.skybox = null;

            Debug.Log("[GenerateTestMap] Освещение создано");
        }

        private static void CreateGameManager()
        {
            GameObject managerObj = new GameObject("GameManager");
            var gameManager = managerObj.AddComponent<KiarcheContinuumWar.Managers.GameManager>();

            SerializedObject serializedObject = new SerializedObject(gameManager);
            serializedObject.FindProperty("enableTestMode").boolValue = true;
            serializedObject.FindProperty("testUnitCount").intValue = 12;
            serializedObject.FindProperty("spawnPosition1").vector3Value = new Vector3(-36f, 0f, 0f);
            serializedObject.FindProperty("spawnPosition2").vector3Value = new Vector3(36f, 0f, 0f);
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[GenerateTestMap] GameManager создан");
        }

        private static void CreateUnitController()
        {
            GameObject controllerObj = new GameObject("UnitController");
            controllerObj.AddComponent<KiarcheContinuumWar.Units.UnitController>();
            Debug.Log("[GenerateTestMap] UnitController создан");
        }

        private static void CreateRTSInput()
        {
            GameObject inputObj = new GameObject("RTSInput");
            var rtsInput = inputObj.AddComponent<KiarcheContinuumWar.InputSystem.RTSInput>();

            Camera mainCamera = Object.FindAnyObjectByType<Camera>();
            if (mainCamera != null)
            {
                SerializedObject serializedObject = new SerializedObject(rtsInput);
                serializedObject.FindProperty("mainCamera").objectReferenceValue = mainCamera;
                serializedObject.ApplyModifiedProperties();
            }

            Debug.Log("[GenerateTestMap] RTSInput создан");
        }

        private static void CreateObstacleGroupMarker(string name, Vector3 position, Transform parent)
        {
            GameObject markerRoot = new GameObject(name);
            markerRoot.transform.parent = parent;
            markerRoot.transform.position = new Vector3(position.x, GetTerrainHeight(position) + 0.2f, position.z);

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = $"{name}_Marker";
            quad.transform.parent = markerRoot.transform;
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            quad.transform.localScale = new Vector3(6f, 2f, 1f);

            Collider collider = quad.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            Renderer renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                Shader shader = Shader.Find("Standard");
                if (shader != null)
                {
                    renderer.sharedMaterial = new Material(shader);
                    renderer.sharedMaterial.color = new Color(1f, 1f, 1f, 0.2f);
                }
            }
        }
    }
}
#endif
