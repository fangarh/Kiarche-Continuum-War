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
