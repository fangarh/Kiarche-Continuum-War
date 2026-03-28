#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using KiarcheContinuumWar.Managers;
using KiarcheContinuumWar.Units;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.InputSystem;
using KiarcheContinuumWar.UI;

namespace KiarcheContinuumWar.Editor
{
    /// <summary>
    /// Editor скрипт для генерации сцены MVP прототипа.
    /// Меню: Tools → KCW → Generate MVP Scene
    /// </summary>
    public class GenerateMVPScene
    {
        [MenuItem("Tools/KCW/Generate MVP Scene")]
        public static void GenerateScene()
        {
            // Создать новую сцену
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            
            // 1. Создать Main Camera
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0, 20, -10);
            cameraObj.transform.rotation = Quaternion.Euler(45, 0, 0);
            camera.orthographic = false;
            camera.fieldOfView = 60;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000;
            cameraObj.AddComponent<AudioListener>();
            
            // 2. Создать Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            
            // 3. Создать Ground
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            groundObj.name = "Ground";
            groundObj.transform.localScale = new Vector3(5, 1, 5);
            
            // 4. Создать GameManager
            GameObject managerObj = new GameObject("GameManager");
            GameManager gameManager = managerObj.AddComponent<GameManager>();
            gameManager.enableTestMode = true;
            gameManager.testUnitCount = 20;
            gameManager.spawnPosition1 = new Vector3(-10, 0, 0);
            gameManager.spawnPosition2 = new Vector3(10, 0, 0);
            
            // 5. Создать ResourceManager
            GameObject resourceObj = new GameObject("ResourceManager");
            ResourceManager resourceManager = resourceObj.AddComponent<ResourceManager>();
            
            // 6. Создать UnitController
            GameObject controllerObj = new GameObject("UnitController");
            UnitController unitController = controllerObj.AddComponent<UnitController>();
            
            // 7. Создать RTSInput
            GameObject inputObj = new GameObject("RTSInput");
            RTSInput rtsInput = inputObj.AddComponent<RTSInput>();
            
            // 8. Создать Canvas для UI
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Добавить GameUI
            GameUI gameUI = canvasObj.AddComponent<GameUI>();
            
            // Создать тексты ресурсов
            GameObject resourcesPanel = CreateResourcesPanel(canvasObj.transform);
            GameObject selectedText = CreateSelectedUnitsText(canvasObj.transform);
            
            // Назначить ссылки в GameUI (через Find после создания)
            EditorApplication.delayCall += () =>
            {
                gameUI.materialsText = GameObject.Find("MaterialsText")?.GetComponent<Text>();
                gameUI.energyText = GameObject.Find("EnergyText")?.GetComponent<Text>();
                gameUI.foodText = GameObject.Find("FoodText")?.GetComponent<Text>();
                gameUI.knowledgeText = GameObject.Find("KnowledgeText")?.GetComponent<Text>();
                gameUI.selectedUnitsText = GameObject.Find("SelectedUnitsText")?.GetComponent<Text>();
            };
            
            // Назначить ссылки в GameManager
            gameManager.resourceManager = resourceManager;
            gameManager.unitController = unitController;
            
            // 9. Создать EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            var eventSystem = eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            
            // Сохранить сцену
            string path = "Assets/Scenes/MVP_Prototype.unity";
            EditorSceneManager.SaveScene(scene, path);
            
            Debug.Log($"MVP Scene generated at {path}");
        }
        
        private static GameObject CreateResourcesPanel(Transform parent)
        {
            GameObject panel = new GameObject("ResourcesPanel");
            panel.transform.SetParent(parent);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(10, -10);
            rect.sizeDelta = new Vector2(200, 150);
            
            // Background
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.5f);
            
            // Создать тексты
            CreateText(panel.transform, "MaterialsText", "Материалы: 100", new Vector2(10, -10));
            CreateText(panel.transform, "EnergyText", "Энергия: 100", new Vector2(10, -30));
            CreateText(panel.transform, "FoodText", "Еда: 100", new Vector2(10, -50));
            CreateText(panel.transform, "KnowledgeText", "Знания: 50", new Vector2(10, -70));
            
            return panel;
        }
        
        private static GameObject CreateSelectedUnitsText(Transform parent)
        {
            GameObject textObj = new GameObject("SelectedUnitsText");
            textObj.transform.SetParent(parent);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 10);
            rect.sizeDelta = new Vector2(200, 30);
            
            Text text = CreateText(textObj.transform, "", "Выделено: 0", Vector2.zero);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 20;
            
            return textObj;
        }
        
        private static Text CreateText(Transform parent, string name, string content, Vector2 position)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(180, 20);
            
            Text text = textObj.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.color = Color.white;
            
            return text;
        }
    }
}
#endif
