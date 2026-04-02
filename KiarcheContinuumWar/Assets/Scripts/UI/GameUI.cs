using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.Units;

namespace KiarcheContinuumWar.UI
{
    /// <summary>
    /// Базовый HUD для RTS.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("Resources Panel")]
        public Text materialsText;
        public Text energyText;
        public Text foodText;
        public Text knowledgeText;

        [Header("Unit Panel")]
        public Text selectedUnitsText;
        public Text unitDetailsText;
        public GameObject unitPanel;

        [Header("References")]
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private UnitController unitController;

        private Font _defaultFont;
        private Vector2Int _lastScreenSize;
        private GUIStyle _panelStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;

        private void Awake()
        {
            _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureCanvasLayout();
            EnsureHudReferences();
        }

        private void Start()
        {
            if (resourceManager == null)
                resourceManager = FindAnyObjectByType<ResourceManager>();

            if (unitController == null)
                unitController = FindAnyObjectByType<UnitController>();

            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged += UpdateResourceDisplay;
            }

            UpdateResourceDisplay();
            UpdateUnitDisplay();
        }

        private void Update()
        {
            EnsureCanvasLayout();
            UpdateUnitDisplay();
        }

        private void OnGUI()
        {
            if (!ShouldUseImmediateGuiFallback())
            {
                return;
            }

            EnsureGuiStyles();
            DrawResourceFallback();
            DrawUnitFallback();
        }

        private void UpdateResourceDisplay(ResourceType type = ResourceType.Materials, int amount = 0)
        {
            if (resourceManager == null)
                return;

            if (materialsText != null)
                materialsText.text = $"Материалы: {resourceManager.Materials}";
            if (energyText != null)
                energyText.text = $"Энергия: {resourceManager.Energy}";
            if (foodText != null)
                foodText.text = $"Еда: {resourceManager.Food}";
            if (knowledgeText != null)
                knowledgeText.text = $"Знания: {resourceManager.Knowledge}";
        }

        private void UpdateUnitDisplay()
        {
            if (unitController == null)
                return;

            int selectedCount = unitController.SelectedUnits.Count;
            if (selectedCount > 0)
            {
                if (selectedUnitsText != null)
                    selectedUnitsText.text = $"Выделено юнитов: {selectedCount}";

                if (unitDetailsText != null)
                    unitDetailsText.text = BuildUnitDetailsText();

                if (unitPanel != null)
                    unitPanel.SetActive(true);
            }
            else
            {
                if (selectedUnitsText != null)
                    selectedUnitsText.text = "Нет выделенных юнитов";

                if (unitDetailsText != null)
                    unitDetailsText.text = "Выделите юнита или группу, чтобы увидеть состав и состояние.";

                if (unitPanel != null)
                    unitPanel.SetActive(false);
            }
        }

        private string BuildUnitDetailsText()
        {
            var selectedUnits = unitController.SelectedUnits;
            if (selectedUnits.Count == 0)
                return string.Empty;

            float totalHealth = 0f;
            float totalDamage = 0f;
            float maxRange = 0f;

            foreach (Unit unit in selectedUnits)
            {
                totalHealth += unit.CurrentHealth;
                totalDamage += unit.Damage;
                if (unit.AttackRange > maxRange)
                    maxRange = unit.AttackRange;
            }

            float averageHealth = totalHealth / selectedUnits.Count;
            float averageDamage = totalDamage / selectedUnits.Count;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Среднее здоровье: {averageHealth:0}");
            builder.AppendLine($"Средний урон: {averageDamage:0}");
            builder.Append($"Макс. дальность атаки: {maxRange:0.0}");
            return builder.ToString();
        }

        private void EnsureHudReferences()
        {
            if (materialsText == null)
                materialsText = FindText("MaterialsText");
            if (energyText == null)
                energyText = FindText("EnergyText");
            if (foodText == null)
                foodText = FindText("FoodText");
            if (knowledgeText == null)
                knowledgeText = FindText("KnowledgeText");

            if (selectedUnitsText == null)
                selectedUnitsText = FindText("SelectedUnitsText");
            if (unitDetailsText == null)
                unitDetailsText = FindText("UnitDetailsText");
            if (unitPanel == null)
                unitPanel = FindPanel("UnitPanel")?.gameObject;

            EnsureResourcesPanel();
            EnsureUnitPanel();
        }

        private void EnsureCanvasLayout()
        {
            Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);
            if (_lastScreenSize == currentScreenSize && _lastScreenSize != Vector2Int.zero)
            {
                return;
            }

            _lastScreenSize = currentScreenSize;

            RectTransform rootRect = GetComponent<RectTransform>();
            if (rootRect != null)
            {
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;
                rootRect.pivot = new Vector2(0.5f, 0.5f);
                rootRect.localScale = Vector3.one;
                rootRect.anchoredPosition = Vector2.zero;
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                if (canvasRect != null)
                {
                    canvasRect.anchorMin = Vector2.zero;
                    canvasRect.anchorMax = Vector2.one;
                    canvasRect.offsetMin = Vector2.zero;
                    canvasRect.offsetMax = Vector2.zero;
                    canvasRect.pivot = new Vector2(0.5f, 0.5f);
                    canvasRect.localScale = Vector3.one;
                    canvasRect.anchoredPosition = Vector2.zero;
                }

                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920f, 1080f);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                }
            }
        }

        private bool ShouldUseImmediateGuiFallback()
        {
            RectTransform rootRect = GetComponent<RectTransform>();
            if (rootRect == null)
            {
                return true;
            }

            float width = rootRect.rect.width;
            float height = rootRect.rect.height;
            return width < Screen.width * 0.8f || height < Screen.height * 0.8f;
        }

        private void EnsureGuiStyles()
        {
            if (_panelStyle != null)
            {
                return;
            }

            _panelStyle = new GUIStyle(GUI.skin.box);
            _panelStyle.normal.background = Texture2D.whiteTexture;
            _panelStyle.normal.textColor = Color.white;
            _panelStyle.alignment = TextAnchor.UpperLeft;
            _panelStyle.padding = new RectOffset(12, 12, 10, 10);

            _titleStyle = new GUIStyle(GUI.skin.label);
            _titleStyle.fontSize = 18;
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.normal.textColor = Color.white;

            _bodyStyle = new GUIStyle(GUI.skin.label);
            _bodyStyle.fontSize = 16;
            _bodyStyle.normal.textColor = Color.white;
            _bodyStyle.richText = false;
            _bodyStyle.wordWrap = true;
        }

        private void DrawResourceFallback()
        {
            Color previousColor = GUI.color;
            GUI.color = new Color(0.05f, 0.08f, 0.12f, 0.9f);
            GUI.Box(new Rect(16f, 16f, 340f, 126f), GUIContent.none, _panelStyle);
            GUI.color = previousColor;

            GUI.Label(new Rect(28f, 24f, 300f, 24f), "Ресурсы", _titleStyle);
            GUI.Label(new Rect(28f, 50f, 300f, 22f), materialsText != null ? materialsText.text : "Материалы: 0", _bodyStyle);
            GUI.Label(new Rect(28f, 72f, 300f, 22f), energyText != null ? energyText.text : "Энергия: 0", _bodyStyle);
            GUI.Label(new Rect(28f, 94f, 300f, 22f), foodText != null ? foodText.text : "Еда: 0", _bodyStyle);
            GUI.Label(new Rect(28f, 116f, 300f, 22f), knowledgeText != null ? knowledgeText.text : "Знания: 0", _bodyStyle);
        }

        private void DrawUnitFallback()
        {
            bool hasSelection = unitController != null && unitController.SelectedUnits.Count > 0;
            if (!hasSelection)
            {
                return;
            }

            float panelHeight = 108f;
            float panelY = Screen.height - panelHeight - 16f;

            Color previousColor = GUI.color;
            GUI.color = new Color(0.05f, 0.08f, 0.12f, 0.9f);
            GUI.Box(new Rect(16f, panelY, 380f, panelHeight), GUIContent.none, _panelStyle);
            GUI.color = previousColor;

            GUI.Label(new Rect(28f, panelY + 10f, 340f, 24f), selectedUnitsText != null ? selectedUnitsText.text : "Выделение", _titleStyle);
            GUI.Label(new Rect(28f, panelY + 38f, 340f, 56f), unitDetailsText != null ? unitDetailsText.text : string.Empty, _bodyStyle);
        }

        private void EnsureResourcesPanel()
        {
            RectTransform resourcesPanel = FindPanel("ResourcesPanel");
            if (resourcesPanel == null)
            {
                resourcesPanel = CreatePanel("ResourcesPanel", new Vector2(16f, -16f), new Vector2(340f, 136f), true);
            }

            ConfigurePanelLayout(resourcesPanel, new Vector2(16f, -16f), new Vector2(340f, 136f), true);
            resourcesPanel.gameObject.SetActive(true);
            resourcesPanel.SetAsLastSibling();

            materialsText = EnsurePanelText(resourcesPanel, "MaterialsText", new Vector2(12f, -12f), "Материалы: 0", 18);
            energyText = EnsurePanelText(resourcesPanel, "EnergyText", new Vector2(12f, -38f), "Энергия: 0", 18);
            foodText = EnsurePanelText(resourcesPanel, "FoodText", new Vector2(12f, -64f), "Еда: 0", 18);
            knowledgeText = EnsurePanelText(resourcesPanel, "KnowledgeText", new Vector2(12f, -90f), "Знания: 0", 18);
        }

        private void EnsureUnitPanel()
        {
            RectTransform panel = unitPanel != null ? unitPanel.GetComponent<RectTransform>() : FindPanel("UnitPanel");
            if (panel == null)
            {
                panel = CreatePanel("UnitPanel", new Vector2(16f, 16f), new Vector2(380f, 120f), false);
                unitPanel = panel.gameObject;
            }

            ConfigurePanelLayout(panel, new Vector2(16f, 16f), new Vector2(380f, 120f), false);
            panel.SetAsLastSibling();
            unitPanel = panel.gameObject;

            selectedUnitsText = EnsurePanelText(panel, "SelectedUnitsText", new Vector2(12f, -12f), "Нет выделенных юнитов", 18);
            unitDetailsText = EnsurePanelText(panel, "UnitDetailsText", new Vector2(12f, -42f), "Выделите юнита или группу, чтобы увидеть состав и состояние.", 14);
            unitDetailsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            unitDetailsText.verticalOverflow = VerticalWrapMode.Overflow;

            if (unitPanel != null && (unitController == null || unitController.SelectedUnits.Count == 0))
                unitPanel.SetActive(false);
        }

        private RectTransform CreatePanel(string name, Vector2 anchoredPosition, Vector2 size, bool topLeft)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObject.transform.SetParent(transform, false);

            RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
            ConfigurePanelLayout(rectTransform, anchoredPosition, size, topLeft);

            Image image = panelObject.GetComponent<Image>();
            image.color = new Color(0.05f, 0.08f, 0.12f, 0.82f);
            return rectTransform;
        }

        private void ConfigurePanelLayout(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size, bool topLeft)
        {
            if (topLeft)
            {
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.pivot = new Vector2(0f, 0f);
            }

            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private Text EnsurePanelText(RectTransform parent, string objectName, Vector2 anchoredPosition, string content, int fontSize)
        {
            Text text = FindText(objectName);
            if (text == null || text.transform.parent != parent)
            {
                Transform existingChild = parent.Find(objectName);
                if (existingChild != null)
                {
                    text = existingChild.GetComponent<Text>();
                }
            }

            if (text == null || text.transform.parent != parent)
            {
                GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
                textObject.transform.SetParent(parent, false);
                text = textObject.GetComponent<Text>();
            }

            ApplyTextLayout(text.rectTransform, anchoredPosition, parent.rect.width - 24f);
            text.font = _defaultFont;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = content;
            return text;
        }

        private void ApplyTextLayout(RectTransform rectTransform, Vector2 anchoredPosition, float width)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(Mathf.Max(width, 220f), 26f);
        }

        private RectTransform FindPanel(string objectName)
        {
            Transform direct = transform.Find(objectName);
            if (direct != null)
                return direct as RectTransform;

            RectTransform[] panels = GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform panel in panels)
            {
                if (panel.name == objectName)
                    return panel;
            }

            return null;
        }

        private Text FindText(string objectName)
        {
            Transform direct = transform.Find(objectName);
            if (direct != null)
                return direct.GetComponent<Text>();

            Text[] texts = GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text.name == objectName)
                    return text;
            }

            return null;
        }

        private void OnDestroy()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged -= UpdateResourceDisplay;
            }
        }
    }
}
