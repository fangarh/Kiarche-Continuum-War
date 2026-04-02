#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using KiarcheContinuumWar.Map;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.UI;
using UnityEngine.UI;

namespace KiarcheContinuumWar.Editor
{
    /// <summary>
    /// Editor script для генерации тестовой карты.
    /// </summary>
    public class GenerateTestMap
    {
        private const string GeneratedAssetsRoot = "Assets/Generated/TestMap";
        private const float MapSize = 500f;
        private const float HalfMapSize = MapSize * 0.5f;

        private enum ObstacleVisualType
        {
            Tree,
            Rock,
            Water
        }

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
            CreateResourceManager();
            CreateGameManager();
            CreateUnitController();
            CreateRTSInput();
            SetupCamera();
            CreateHUD();
            CreateEventSystem();
            RemoveMissingScriptsInScene(scene);

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
                size = new Vector3(MapSize, 350f, MapSize),
                heightmapResolution = 129,
                alphamapResolution = 128
            };

            terrain.terrainData = terrainData;
            terrainObj.transform.position = new Vector3(-HalfMapSize, 0f, -HalfMapSize);
            terrain.drawHeightmap = true;
            terrain.materialTemplate = CreateTerrainMaterial();
            terrain.terrainData.terrainLayers = CreateTerrainLayers();

            float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    float nx = x / (float)(terrainData.heightmapResolution - 1);
                    float ny = y / (float)(terrainData.heightmapResolution - 1);
                    float centerDistance = Vector2.Distance(new Vector2(nx, ny), new Vector2(0.5f, 0.5f));
                    float edgeWeight = Mathf.Clamp01(centerDistance * 2.15f);
                    float macroNoise = Mathf.PerlinNoise(nx * 1.75f + 3f, ny * 1.65f + 5f);
                    float ridgeNoise = Mathf.PerlinNoise(nx * 0.9f + 17f, ny * 1.05f + 9f);
                    float detailNoise = Mathf.PerlinNoise(nx * 10.5f + 13f, ny * 10.5f + 7f);
                    float peakNoise = Mathf.PerlinNoise(nx * 4.6f + 31f, ny * 4.6f + 19f);
                    float mountainNoise = Mathf.PerlinNoise(nx * 2.6f + 21f, ny * 2.3f + 14f);
                    float centralBasin = 1f - Mathf.Clamp01(centerDistance * 2.45f);
                    float ridgeMask = Mathf.Pow(Mathf.SmoothStep(0.14f, 1f, ridgeNoise), 1.55f);
                    float peaks = Mathf.Pow(Mathf.Clamp01(peakNoise - 0.35f) * 1.95f, 1.8f);
                    float mountains = Mathf.Pow(Mathf.Clamp01(mountainNoise - 0.28f) * 1.55f, 1.45f);
                    float terraces = Mathf.Floor((macroNoise * 0.4f + ridgeMask * 0.6f) * 7f) / 7f;

                    float height = 0.06f;
                    height += macroNoise * 0.16f;
                    height += ridgeMask * 0.28f * edgeWeight;
                    height += peaks * 0.26f * edgeWeight;
                    height += mountains * 0.22f * edgeWeight;
                    height += detailNoise * 0.045f;
                    height += terraces * 0.08f * edgeWeight;
                    height -= centralBasin * 0.14f;

                    heights[y, x] = Mathf.Clamp01(height);
                }
            }

            terrain.terrainData.SetHeights(0, 0, heights);
            PaintTerrainLayers(terrainData, heights);

            TerrainCollider terrainCollider = terrainObj.AddComponent<TerrainCollider>();
            terrainCollider.terrainData = terrainData;

            Debug.Log("[GenerateTestMap] Terrain создан: крупные горные перепады, выраженные гряды и широкая центральная низина");
        }

        private static TerrainLayer[] CreateTerrainLayers()
        {
            return new[]
            {
                CreateTerrainLayer("Lowland", CreateTerrainTexture(
                    64,
                    64,
                    new Color(0.24f, 0.22f, 0.14f),
                    new Color(0.34f, 0.3f, 0.18f),
                    7.5f), new Vector2(9f, 9f)),
                CreateTerrainLayer("Grassland", CreateTerrainTexture(
                    64,
                    64,
                    new Color(0.18f, 0.34f, 0.12f),
                    new Color(0.35f, 0.52f, 0.2f),
                    9f), new Vector2(11f, 11f)),
                CreateTerrainLayer("Rock", CreateTerrainTexture(
                    64,
                    64,
                    new Color(0.38f, 0.38f, 0.4f),
                    new Color(0.58f, 0.58f, 0.6f),
                    12f), new Vector2(10f, 10f)),
                CreateTerrainLayer("Peak", CreateTerrainTexture(
                    64,
                    64,
                    new Color(0.67f, 0.64f, 0.56f),
                    new Color(0.86f, 0.84f, 0.78f),
                    8f), new Vector2(14f, 14f)),
            };
        }

        private static Material CreateTerrainMaterial()
        {
            EnsureGeneratedAssetsFolder();

            Shader shader = Shader.Find("Universal Render Pipeline/Terrain/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Nature/Terrain/Standard");
            }
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            string materialPath = $"{GeneratedAssetsRoot}/TerrainMaterial.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(shader) { name = "TerrainMaterial" };
                AssetDatabase.CreateAsset(material, materialPath);
            }
            else
            {
                material.shader = shader;
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.04f);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static TerrainLayer CreateTerrainLayer(string name, Texture2D texture, Vector2 tileSize)
        {
            EnsureGeneratedAssetsFolder();

            string texturePath = $"{GeneratedAssetsRoot}/{name}_Texture.asset";
            SaveOrReplaceAsset(texture, texturePath);

            TerrainLayer layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>($"{GeneratedAssetsRoot}/{name}_Layer.terrainlayer");
            if (layer == null)
            {
                layer = new TerrainLayer();
                AssetDatabase.CreateAsset(layer, $"{GeneratedAssetsRoot}/{name}_Layer.terrainlayer");
            }

            layer.name = $"{name}_Layer";
            layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            layer.tileSize = tileSize;
            EditorUtility.SetDirty(layer);

            return layer;
        }

        private static void PaintTerrainLayers(TerrainData terrainData, float[,] heights)
        {
            int alphamapWidth = terrainData.alphamapWidth;
            int alphamapHeight = terrainData.alphamapHeight;
            int layerCount = terrainData.terrainLayers.Length;
            float[,,] alphamaps = new float[alphamapHeight, alphamapWidth, layerCount];

            for (int y = 0; y < alphamapHeight; y++)
            {
                for (int x = 0; x < alphamapWidth; x++)
                {
                    float normalizedX = x / (float)(alphamapWidth - 1);
                    float normalizedY = y / (float)(alphamapHeight - 1);
                    float height = heights[
                        Mathf.RoundToInt(normalizedY * (terrainData.heightmapResolution - 1)),
                        Mathf.RoundToInt(normalizedX * (terrainData.heightmapResolution - 1))];
                    float steepness = terrainData.GetSteepness(normalizedX, normalizedY) / 90f;

                    float lowland = Mathf.Clamp01(1f - Mathf.InverseLerp(0.12f, 0.34f, height));
                    float grass = Mathf.Clamp01(1f - Mathf.Abs(height - 0.38f) / 0.22f) * (1f - steepness * 0.55f);
                    float rock = Mathf.Clamp01(Mathf.InverseLerp(0.38f, 0.7f, height) + steepness * 0.9f);
                    float peak = Mathf.Clamp01(Mathf.InverseLerp(0.68f, 0.95f, height)) * (0.4f + steepness * 0.6f);

                    float total = lowland + grass + rock + peak;
                    if (total <= 0.0001f)
                    {
                        grass = 1f;
                        total = 1f;
                    }

                    alphamaps[y, x, 0] = lowland / total;
                    alphamaps[y, x, 1] = grass / total;
                    alphamaps[y, x, 2] = rock / total;
                    alphamaps[y, x, 3] = peak / total;
                }
            }

            terrainData.SetAlphamaps(0, 0, alphamaps);
        }

        private static void CreateObstacles()
        {
            GameObject obstaclesParent = new GameObject("Obstacles");
            CreateSectorContent(obstaclesParent.transform);
            Debug.Log("[GenerateTestMap] Карта заполнена секторами по всей площади");
        }

        private static void CreateObstacle(Transform parent, Vector3 position, float radius, string name, ObstacleVisualType visualType)
        {
            float terrainHeight = GetTerrainHeight(position);
            GameObject obstacleObj = new GameObject(name);
            obstacleObj.transform.parent = parent;

            float obstacleHeight = visualType == ObstacleVisualType.Water
                ? 0.35f
                : Random.Range(3f, 8f);

            obstacleObj.transform.position = new Vector3(position.x, terrainHeight + obstacleHeight * 0.5f, position.z);

            Obstacle obstacle = obstacleObj.AddComponent<Obstacle>();
            obstacle.SetObstacleRadius(radius);

            CreateObstacleVisual(obstacleObj.transform, radius, obstacleHeight, visualType);
        }

        private static void CreateSectorContent(Transform parent)
        {
            int sectorIndex = 0;
            const float sectorSize = 35f;

            for (float x = -227.5f; x <= 227.5f; x += sectorSize)
            {
                for (float z = -227.5f; z <= 227.5f; z += sectorSize)
                {
                    Vector3 sectorCenter = new Vector3(x, 0f, z);
                    bool isCentralSector = Mathf.Abs(x) < 42f && Mathf.Abs(z) < 42f;
                    float noise = Mathf.PerlinNoise((x + 50f) * 0.06f, (z + 50f) * 0.06f);

                    if (isCentralSector)
                    {
                        sectorIndex++;
                        continue;
                    }

                    CreateObstacleGroupMarker($"Sector_{sectorIndex:00}", sectorCenter, parent);

                    ObstacleVisualType primaryType = noise > 0.7f
                        ? ObstacleVisualType.Rock
                        : (noise > 0.42f ? ObstacleVisualType.Tree : ObstacleVisualType.Water);
                    float primaryRadius = 1.6f + noise * 0.95f;
                    CreateObstacle(parent, sectorCenter, primaryRadius, $"SectorCore_{sectorIndex:00}", primaryType);

                    int extraObjects = noise > 0.72f ? 3 : 2;
                    for (int extraIndex = 0; extraIndex < extraObjects; extraIndex++)
                    {
                        float angle = (sectorIndex * 1.19f) + (extraIndex * 2.15f);
                        Vector3 secondaryOffset = new Vector3(
                            Mathf.Cos(angle) * (10f + extraIndex * 6f),
                            0f,
                            Mathf.Sin(angle) * (10f + extraIndex * 6f));
                        ObstacleVisualType secondaryType = primaryType == ObstacleVisualType.Water
                            ? ObstacleVisualType.Tree
                            : (extraIndex == 0 ? ObstacleVisualType.Rock : ObstacleVisualType.Tree);
                        CreateObstacle(
                            parent,
                            sectorCenter + secondaryOffset,
                            1.2f + noise + extraIndex * 0.25f,
                            $"SectorSide_{sectorIndex:00}_{extraIndex}",
                            secondaryType);
                    }

                    sectorIndex++;
                }
            }
        }

        private static void CreateSpawnPoints()
        {
            GameObject spawnsParent = new GameObject("SpawnPoints");

            CreateSpawnPoint(spawnsParent.transform, new Vector3(-35f, 5f, -18f), "PlayerSpawn_1", new Color(0.2f, 0.45f, 1f));
            CreateSpawnPoint(spawnsParent.transform, new Vector3(35f, 5f, -18f), "PlayerSpawn_2", new Color(1f, 0.25f, 0.25f));
            CreateSpawnPoint(spawnsParent.transform, new Vector3(0f, 5f, 36f), "PlayerSpawn_3", new Color(0.2f, 0.85f, 0.35f));

            Debug.Log("[GenerateTestMap] Стартовые позиции поставлены вокруг центра карты");
        }

        private static void CreateSpawnPoint(Transform parent, Vector3 position, string name, Color markerColor)
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
                renderer.sharedMaterial.color = markerColor;
            }
        }

        private static void CreateMapManager()
        {
            GameObject managerObj = new GameObject("MapManager");
            MapManager mapManager = managerObj.AddComponent<MapManager>();

            SerializedObject serializedObject = new SerializedObject(mapManager);
            serializedObject.FindProperty("mapSize").vector2Value = new Vector2(MapSize, MapSize);
            serializedObject.FindProperty("mapOrigin").vector3Value = new Vector3(-HalfMapSize, 0f, -HalfMapSize);
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
            GameObject playerSpawn3 = GameObject.Find("PlayerSpawn_3");

            serializedObject = new SerializedObject(mapManager);
            SerializedProperty playerProp = serializedObject.FindProperty("playerSpawnPoints");
            playerProp.arraySize = 3;
            if (playerSpawn1 != null) playerProp.GetArrayElementAtIndex(0).objectReferenceValue = playerSpawn1.transform;
            if (playerSpawn2 != null) playerProp.GetArrayElementAtIndex(1).objectReferenceValue = playerSpawn2.transform;
            if (playerSpawn3 != null) playerProp.GetArrayElementAtIndex(2).objectReferenceValue = playerSpawn3.transform;

            SerializedProperty enemyProp = serializedObject.FindProperty("enemySpawnPoints");
            enemyProp.arraySize = 0;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[GenerateTestMap] MapManager создан");
        }

        private static void CreateFlowFieldManager()
        {
            GameObject flowFieldObj = new GameObject("FlowFieldManager");
            var flowFieldManager = flowFieldObj.AddComponent<KiarcheContinuumWar.Pathfinding.FlowFieldManager>();
            SerializedObject serializedObject = new SerializedObject(flowFieldManager);
            serializedObject.FindProperty("fieldWidth").intValue = Mathf.RoundToInt(MapSize);
            serializedObject.FindProperty("fieldHeight").intValue = Mathf.RoundToInt(MapSize);
            serializedObject.FindProperty("cellSize").floatValue = 1f;
            serializedObject.FindProperty("obstacleScanRadius").floatValue = HalfMapSize + 25f;
            serializedObject.ApplyModifiedProperties();
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

            var rtsCamera = mainCamera.GetComponent<KiarcheContinuumWar.CameraSystem.RTSCamera>();
            SerializedObject cameraObject = new SerializedObject(rtsCamera);
            cameraObject.FindProperty("moveSpeed").floatValue = 160f;
            cameraObject.FindProperty("minZoom").floatValue = 40f;
            cameraObject.FindProperty("maxZoom").floatValue = 360f;
            cameraObject.FindProperty("defaultZoom").floatValue = 180f;
            cameraObject.FindProperty("defaultRotationX").floatValue = 58f;
            cameraObject.ApplyModifiedProperties();

            mainCamera.transform.position = new Vector3(0f, 180f, 0f);
            mainCamera.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            mainCamera.nearClipPlane = 0.1f;
            mainCamera.farClipPlane = 1000f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;

            Debug.Log("[GenerateTestMap] Камера настроена на центр карты и полный охват области");
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
            serializedObject.FindProperty("spawnPosition1").vector3Value = new Vector3(-35f, 0f, -18f);
            serializedObject.FindProperty("spawnPosition2").vector3Value = new Vector3(35f, 0f, -18f);
            serializedObject.FindProperty("spawnPosition3").vector3Value = new Vector3(0f, 0f, 36f);
            serializedObject.FindProperty("resourceManager").objectReferenceValue = Object.FindAnyObjectByType<ResourceManager>();
            serializedObject.FindProperty("unitController").objectReferenceValue = Object.FindAnyObjectByType<KiarcheContinuumWar.Units.UnitController>();
            serializedObject.ApplyModifiedProperties();

            Debug.Log("[GenerateTestMap] GameManager создан");
        }

        private static void CreateResourceManager()
        {
            GameObject resourceObj = new GameObject("ResourceManager");
            resourceObj.AddComponent<ResourceManager>();
            Debug.Log("[GenerateTestMap] ResourceManager создан");
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

        private static void CreateHUD()
        {
            GameObject canvasObj = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.anchorMin = Vector2.zero;
            canvasRect.anchorMax = Vector2.one;
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;
            canvasRect.pivot = new Vector2(0.5f, 0.5f);

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GameObject hudRoot = CreateStretchRoot(canvasObj.transform, "HUDRoot");
            GameUI gameUI = hudRoot.AddComponent<GameUI>();
            GameObject resourcesPanel = CreatePanel(hudRoot.transform, "ResourcesPanel", new Vector2(16f, -16f), new Vector2(320f, 140f), true);
            gameUI.materialsText = CreateHudText(resourcesPanel.transform, "MaterialsText", new Vector2(12f, -12f), "Материалы: 100", 18);
            gameUI.energyText = CreateHudText(resourcesPanel.transform, "EnergyText", new Vector2(12f, -40f), "Энергия: 100", 18);
            gameUI.foodText = CreateHudText(resourcesPanel.transform, "FoodText", new Vector2(12f, -68f), "Еда: 100", 18);
            gameUI.knowledgeText = CreateHudText(resourcesPanel.transform, "KnowledgeText", new Vector2(12f, -96f), "Знания: 50", 18);

            GameObject unitPanel = CreatePanel(hudRoot.transform, "UnitPanel", new Vector2(16f, 16f), new Vector2(380f, 120f), false);
            gameUI.unitPanel = unitPanel;
            gameUI.selectedUnitsText = CreateHudText(unitPanel.transform, "SelectedUnitsText", new Vector2(12f, -12f), "Нет выделенных юнитов", 20);
            gameUI.unitDetailsText = CreateHudText(unitPanel.transform, "UnitDetailsText", new Vector2(12f, -42f), "Выделите юнита или группу, чтобы увидеть состав и состояние.", 14);
            gameUI.unitDetailsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            gameUI.unitDetailsText.verticalOverflow = VerticalWrapMode.Overflow;
            unitPanel.SetActive(false);

            CreateMinimap(hudRoot.transform);

            Debug.Log("[GenerateTestMap] HUD создан");
        }

        private static void CreateEventSystem()
        {
            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            Debug.Log("[GenerateTestMap] EventSystem создан");
        }

        private static void RemoveMissingScriptsInScene(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                RemoveMissingScriptsRecursive(rootObject.transform);
            }
        }

        private static void RemoveMissingScriptsRecursive(Transform current)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(current.gameObject);

            for (int i = 0; i < current.childCount; i++)
            {
                RemoveMissingScriptsRecursive(current.GetChild(i));
            }
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, bool topLeft)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.GetComponent<RectTransform>();
            if (topLeft)
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
            }
            else
            {
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0f, 0f);
            }

            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = panel.GetComponent<Image>();
            image.color = new Color(0.05f, 0.08f, 0.12f, 0.78f);
            return panel;
        }

        private static GameObject CreateStretchRoot(Transform parent, string name)
        {
            GameObject root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);

            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            return root;
        }

        private static Text CreateHudText(Transform parent, string name, Vector2 position, string content, int fontSize)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObj.transform.SetParent(parent, false);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(292f, 26f);

            Text text = textObj.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = content;
            return text;
        }

        private static void CreateMinimap(Transform hudRoot)
        {
            GameObject minimapPanel = CreatePanel(hudRoot, "MinimapPanel", new Vector2(-16f, -16f), new Vector2(240f, 240f), true);
            RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(1f, 1f);

            GameObject minimapImageObject = new GameObject("MinimapImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            minimapImageObject.transform.SetParent(minimapPanel.transform, false);
            RectTransform imageRect = minimapImageObject.GetComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0f, 0f);
            imageRect.anchorMax = new Vector2(1f, 1f);
            imageRect.offsetMin = new Vector2(8f, 8f);
            imageRect.offsetMax = new Vector2(-8f, -8f);

            RawImage minimapImage = minimapImageObject.GetComponent<RawImage>();
            minimapImage.color = Color.white;

            GameObject markersRootObject = new GameObject("MarkersRoot", typeof(RectTransform));
            markersRootObject.transform.SetParent(minimapImageObject.transform, false);
            RectTransform markersRootRect = markersRootObject.GetComponent<RectTransform>();
            markersRootRect.anchorMin = new Vector2(0f, 0f);
            markersRootRect.anchorMax = new Vector2(0f, 0f);
            markersRootRect.pivot = new Vector2(0f, 0f);
            markersRootRect.anchoredPosition = Vector2.zero;
            markersRootRect.sizeDelta = new Vector2(224f, 224f);

            GameObject markerPrefabObject = new GameObject("MarkerPrefab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            markerPrefabObject.transform.SetParent(markersRootObject.transform, false);
            RectTransform markerRect = markerPrefabObject.GetComponent<RectTransform>();
            markerRect.anchorMin = new Vector2(0f, 0f);
            markerRect.anchorMax = new Vector2(0f, 0f);
            markerRect.pivot = new Vector2(0.5f, 0.5f);
            markerRect.sizeDelta = new Vector2(6f, 6f);
            Image markerImage = markerPrefabObject.GetComponent<Image>();
            markerImage.color = Color.white;
            markerPrefabObject.SetActive(false);

            GameObject minimapCameraObject = new GameObject("MinimapCamera");
            Camera minimapCamera = minimapCameraObject.AddComponent<Camera>();
            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = MapSize * 0.28f;
            minimapCamera.transform.position = new Vector3(0f, 320f, 0f);
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = new Color(0.06f, 0.08f, 0.1f);
            minimapCamera.nearClipPlane = 1f;
            minimapCamera.farClipPlane = 600f;
            minimapCamera.depth = -50f;

            RenderTexture renderTexture = new RenderTexture(256, 256, 16)
            {
                name = "MinimapTexture"
            };
            minimapCamera.targetTexture = renderTexture;
            minimapImage.texture = renderTexture;

            var minimapUI = minimapPanel.AddComponent<KiarcheContinuumWar.UI.MinimapUI>();
            SerializedObject minimapObject = new SerializedObject(minimapUI);
            minimapObject.FindProperty("minimapImage").objectReferenceValue = minimapImage;
            minimapObject.FindProperty("markersRoot").objectReferenceValue = markersRootRect;
            minimapObject.FindProperty("markerPrefab").objectReferenceValue = markerImage;
            minimapObject.FindProperty("minimapCamera").objectReferenceValue = minimapCamera;
            minimapObject.FindProperty("mapManager").objectReferenceValue = Object.FindAnyObjectByType<MapManager>();
            minimapObject.ApplyModifiedProperties();
        }

        private static void CreateObstacleVisual(Transform parent, float radius, float height, ObstacleVisualType visualType)
        {
            switch (visualType)
            {
                case ObstacleVisualType.Tree:
                    CreateTreeVisual(parent, radius, height);
                    break;
                case ObstacleVisualType.Rock:
                    CreateRockVisual(parent, radius, height);
                    break;
                case ObstacleVisualType.Water:
                    CreateWaterVisual(parent, radius);
                    break;
            }
        }

        private static void CreateTreeVisual(Transform parent, float radius, float height)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "TreeTrunk";
            trunk.transform.SetParent(parent, false);
            trunk.transform.localScale = new Vector3(radius * 0.35f, height * 0.35f, radius * 0.35f);
            trunk.transform.localPosition = new Vector3(0f, height * 0.35f, 0f);
            ApplyMaterial(trunk, CreateMaterial(
                "TreeTrunk",
                new Color(0.36f, 0.22f, 0.1f),
                CreateBarkTexture(),
                0.08f));

            GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "TreeCanopy";
            canopy.transform.SetParent(parent, false);
            canopy.transform.localScale = new Vector3(radius * 1.9f, height * 0.65f, radius * 1.9f);
            canopy.transform.localPosition = new Vector3(0f, height * 0.9f, 0f);
            ApplyMaterial(canopy, CreateMaterial(
                "TreeCanopy",
                new Color(0.2f, 0.5f, 0.2f),
                CreateLeafTexture(),
                0.15f));

            ResizeCollider(trunk, radius);
            DisableCollider(canopy);
        }

        private static void CreateRockVisual(Transform parent, float radius, float height)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = "RockBody";
            rock.transform.SetParent(parent, false);
            rock.transform.localScale = new Vector3(radius * 1.7f, height * 0.55f, radius * 1.45f);
            rock.transform.localPosition = new Vector3(0f, height * 0.275f, 0f);
            rock.transform.localRotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0f, 180f), Random.Range(-10f, 10f));
            ApplyMaterial(rock, CreateMaterial(
                "RockBody",
                new Color(0.42f, 0.44f, 0.48f),
                CreateRockTexture(),
                0.28f));

            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            top.name = "RockTop";
            top.transform.SetParent(parent, false);
            top.transform.localScale = new Vector3(radius * 1.2f, height * 0.32f, radius * 1.1f);
            top.transform.localPosition = new Vector3(radius * 0.12f, height * 0.55f, -radius * 0.08f);
            ApplyMaterial(top, CreateMaterial(
                "RockTop",
                new Color(0.5f, 0.52f, 0.56f),
                CreateRockTexture(),
                0.24f));

            ResizeCollider(rock, radius);
            DisableCollider(top);
        }

        private static void CreateWaterVisual(Transform parent, float radius)
        {
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            water.name = "WaterPool";
            water.transform.SetParent(parent, false);
            water.transform.localScale = new Vector3(radius * 1.9f, 0.08f, radius * 1.9f);
            water.transform.localPosition = new Vector3(0f, 0f, 0f);
            ApplyMaterial(water, CreateMaterial(
                "WaterPool",
                new Color(0.14f, 0.46f, 0.72f),
                CreateWaterTexture(),
                0.72f));

            GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "WaterRim";
            rim.transform.SetParent(parent, false);
            rim.transform.localScale = new Vector3(radius * 2.15f, 0.03f, radius * 2.15f);
            rim.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            ApplyMaterial(rim, CreateMaterial(
                "WaterRim",
                new Color(0.23f, 0.28f, 0.18f),
                CreateDirtTexture(),
                0.14f));

            ResizeCollider(water, radius);
            DisableCollider(rim);
        }

        private static void ResizeCollider(GameObject target, float radius)
        {
            if (target.TryGetComponent(out CapsuleCollider capsuleCollider))
            {
                capsuleCollider.radius = radius;
                capsuleCollider.height = Mathf.Max(radius * 2f, capsuleCollider.height);
            }
            else if (target.TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider.size = new Vector3(radius * 2f, boxCollider.size.y, radius * 2f);
            }
        }

        private static void DisableCollider(GameObject target)
        {
            Collider collider = target.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        private static void ApplyMaterial(GameObject target, Material material)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material CreateMaterial(string materialName, Color tint, Texture2D texture, float smoothness)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                shader = Shader.Find("Unlit/Texture");
            }

            Material material = new Material(shader)
            {
                name = materialName
            };

            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
            }
            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", tint);
            }
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", tint);
            }
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", smoothness);
            }

            return material;
        }

        private static Texture2D CreateBarkTexture()
        {
            return CreateTexture(64, 64, (x, y) =>
            {
                float lineNoise = Mathf.PerlinNoise(x * 0.15f, y * 0.08f);
                float verticalBands = Mathf.Abs(Mathf.Sin(x * 0.38f)) * 0.35f;
                Color dark = new Color(0.25f, 0.14f, 0.06f);
                Color light = new Color(0.45f, 0.29f, 0.14f);
                return Color.Lerp(dark, light, Mathf.Clamp01(lineNoise * 0.7f + verticalBands));
            });
        }

        private static Texture2D CreateLeafTexture()
        {
            return CreateTexture(64, 64, (x, y) =>
            {
                float nx = x / 63f;
                float ny = y / 63f;
                float blotch = Mathf.PerlinNoise(nx * 4.5f + 2f, ny * 4.5f + 7f);
                float highlight = Mathf.PerlinNoise(nx * 9f + 12f, ny * 9f + 3f) * 0.35f;
                Color dark = new Color(0.12f, 0.32f, 0.1f);
                Color light = new Color(0.33f, 0.58f, 0.22f);
                return Color.Lerp(dark, light, Mathf.Clamp01(blotch + highlight));
            });
        }

        private static Texture2D CreateRockTexture()
        {
            return CreateTexture(64, 64, (x, y) =>
            {
                float nx = x / 63f;
                float ny = y / 63f;
                float largeNoise = Mathf.PerlinNoise(nx * 5f + 4f, ny * 5f + 9f);
                float grain = Mathf.PerlinNoise(nx * 15f + 1f, ny * 15f + 6f) * 0.35f;
                Color dark = new Color(0.28f, 0.3f, 0.33f);
                Color light = new Color(0.58f, 0.6f, 0.63f);
                return Color.Lerp(dark, light, Mathf.Clamp01(largeNoise * 0.8f + grain));
            });
        }

        private static Texture2D CreateWaterTexture()
        {
            return CreateTexture(64, 64, (x, y) =>
            {
                float nx = x / 63f;
                float ny = y / 63f;
                float waveA = Mathf.Sin((nx + ny) * 18f) * 0.5f + 0.5f;
                float waveB = Mathf.Sin((nx - ny) * 14f + 1.7f) * 0.5f + 0.5f;
                float ripple = Mathf.PerlinNoise(nx * 8f + 5f, ny * 8f + 11f) * 0.25f;
                Color deep = new Color(0.05f, 0.24f, 0.45f);
                Color shallow = new Color(0.22f, 0.58f, 0.82f);
                return Color.Lerp(deep, shallow, Mathf.Clamp01((waveA * 0.4f) + (waveB * 0.35f) + ripple));
            });
        }

        private static Texture2D CreateDirtTexture()
        {
            return CreateTexture(64, 64, (x, y) =>
            {
                float nx = x / 63f;
                float ny = y / 63f;
                float noise = Mathf.PerlinNoise(nx * 6f + 2f, ny * 6f + 8f);
                float specks = Mathf.PerlinNoise(nx * 18f + 14f, ny * 18f + 4f) * 0.2f;
                Color dark = new Color(0.22f, 0.18f, 0.12f);
                Color light = new Color(0.42f, 0.35f, 0.22f);
                return Color.Lerp(dark, light, Mathf.Clamp01(noise + specks));
            });
        }

        private static Texture2D CreateTerrainTexture(int width, int height, Color dark, Color light, float noiseScale)
        {
            return CreateTexture(width, height, (x, y) =>
            {
                float nx = x / (float)(width - 1);
                float ny = y / (float)(height - 1);
                float baseNoise = Mathf.PerlinNoise(nx * noiseScale + 3f, ny * noiseScale + 5f);
                float detail = Mathf.PerlinNoise(nx * (noiseScale * 2.4f) + 11f, ny * (noiseScale * 2.4f) + 17f) * 0.28f;
                return Color.Lerp(dark, light, Mathf.Clamp01(baseNoise * 0.82f + detail));
            });
        }

        private static Texture2D CreateTexture(int width, int height, System.Func<int, int, Color> colorProvider)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear,
                name = "GeneratedTexture"
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, colorProvider(x, y));
                }
            }

            texture.Apply();
            return texture;
        }

        private static void EnsureGeneratedAssetsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Generated"))
            {
                AssetDatabase.CreateFolder("Assets", "Generated");
            }

            if (!AssetDatabase.IsValidFolder(GeneratedAssetsRoot))
            {
                AssetDatabase.CreateFolder("Assets/Generated", "TestMap");
            }
        }

        private static void SaveOrReplaceAsset(Object asset, string path)
        {
            Object existingAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (existingAsset != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            AssetDatabase.CreateAsset(asset, path);
        }

        private static void CreateObstacleGroupMarker(string name, Vector3 position, Transform parent)
        {
            GameObject markerRoot = new GameObject(name);
            markerRoot.transform.parent = parent;
            float terrainHeight = GetTerrainHeight(position);
            markerRoot.transform.position = new Vector3(position.x, terrainHeight, position.z);

            GameObject baseStone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseStone.name = $"{name}_Base";
            baseStone.transform.parent = markerRoot.transform;
            baseStone.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            baseStone.transform.localScale = new Vector3(2.8f, 0.4f, 2.8f);
            ApplyMaterial(baseStone, CreateMaterial($"{name}_Base_Mat", new Color(0.42f, 0.44f, 0.48f), CreateRockTexture(), 0.22f));

            GameObject obelisk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obelisk.name = $"{name}_Obelisk";
            obelisk.transform.parent = markerRoot.transform;
            obelisk.transform.localPosition = new Vector3(0f, 2.1f, 0f);
            obelisk.transform.localRotation = Quaternion.Euler(0f, 18f, 0f);
            obelisk.transform.localScale = new Vector3(0.9f, 3.4f, 0.9f);
            ApplyMaterial(obelisk, CreateMaterial($"{name}_Obelisk_Mat", new Color(0.5f, 0.47f, 0.4f), CreateDirtTexture(), 0.12f));

            GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cap.name = $"{name}_Cap";
            cap.transform.parent = markerRoot.transform;
            cap.transform.localPosition = new Vector3(0f, 3.9f, 0f);
            cap.transform.localScale = new Vector3(1.25f, 0.45f, 1.25f);
            ApplyMaterial(cap, CreateMaterial($"{name}_Cap_Mat", new Color(0.65f, 0.62f, 0.55f), CreateRockTexture(), 0.16f));
            DisableCollider(cap);
        }
    }
}
#endif
