using UnityEngine;
using UnityEngine.UI;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.Units;
using System.Text;

namespace KiarcheContinuumWar.UI
{
    /// <summary>
    /// Базовый HUD для RTS.
    /// Отображает ресурсы, выделенных юнитов, панель команд.
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

        private void Awake()
        {
            _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureHudReferences();
        }

        private void Start()
        {
            if (resourceManager == null)
                resourceManager = FindAnyObjectByType<ResourceManager>();

            if (unitController == null)
                unitController = FindAnyObjectByType<UnitController>();

            // Подписаться на события
            if (resourceManager != null)
            {
                resourceManager.OnResourceChanged += UpdateResourceDisplay;
            }

            UpdateResourceDisplay();
        }

        private void Update()
        {
            UpdateUnitDisplay();
        }

        private void UpdateResourceDisplay(ResourceType type = ResourceType.Materials, int amount = 0)
        {
            if (resourceManager == null) return;

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
            if (unitController == null) return;

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
            {
                return string.Empty;
            }

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
                unitPanel = transform.Find("UnitPanel")?.gameObject;

            EnsureResourcesPanel();
            EnsureUnitPanel();
        }

        private void EnsureResourcesPanel()
        {
            RectTransform resourcesPanel = transform.Find("ResourcesPanel") as RectTransform;
            if (resourcesPanel == null)
            {
                resourcesPanel = CreatePanel("ResourcesPanel", new Vector2(16f, -16f), new Vector2(260f, 132f), TextAnchor.UpperLeft);
            }

            materialsText = EnsurePanelText(resourcesPanel, "MaterialsText", new Vector2(12f, -12f), "Материалы: 0");
            energyText = EnsurePanelText(resourcesPanel, "EnergyText", new Vector2(12f, -38f), "Энергия: 0");
            foodText = EnsurePanelText(resourcesPanel, "FoodText", new Vector2(12f, -64f), "Еда: 0");
            knowledgeText = EnsurePanelText(resourcesPanel, "KnowledgeText", new Vector2(12f, -90f), "Знания: 0");
        }

        private void EnsureUnitPanel()
        {
            RectTransform panel = unitPanel != null ? unitPanel.GetComponent<RectTransform>() : null;
            if (panel == null)
            {
                panel = CreatePanel("UnitPanel", new Vector2(16f, 16f), new Vector2(320f, 120f), TextAnchor.LowerLeft);
                unitPanel = panel.gameObject;
            }

            selectedUnitsText = EnsurePanelText(panel, "SelectedUnitsText", new Vector2(12f, -12f), "Нет выделенных юнитов");
            selectedUnitsText.alignment = TextAnchor.UpperLeft;
            selectedUnitsText.fontSize = 18;

            unitDetailsText = EnsurePanelText(panel, "UnitDetailsText", new Vector2(12f, -42f), "Выделите юнита или группу, чтобы увидеть состав и состояние.");
            unitDetailsText.alignment = TextAnchor.UpperLeft;
            unitDetailsText.fontSize = 14;
            unitDetailsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            unitDetailsText.verticalOverflow = VerticalWrapMode.Overflow;

            if (unitPanel != null && (unitController == null || unitController.SelectedUnits.Count == 0))
                unitPanel.SetActive(false);
        }

        private RectTransform CreatePanel(string name, Vector2 anchoredPosition, Vector2 size, TextAnchor anchor)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObject.transform.SetParent(transform, false);

            RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
            switch (anchor)
            {
                case TextAnchor.UpperLeft:
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.pivot = new Vector2(0f, 1f);
                    break;
                default:
                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    rectTransform.anchorMax = new Vector2(0f, 0f);
                    rectTransform.pivot = new Vector2(0f, 0f);
                    break;
            }

            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            Image image = panelObject.GetComponent<Image>();
            image.color = new Color(0.05f, 0.08f, 0.12f, 0.78f);
            return rectTransform;
        }

        private Text EnsurePanelText(RectTransform parent, string objectName, Vector2 anchoredPosition, string content)
        {
            Text text = FindText(objectName);
            if (text != null && text.transform.parent == parent)
            {
                return text;
            }

            Transform existingChild = parent.Find(objectName);
            if (existingChild != null)
            {
                text = existingChild.GetComponent<Text>();
                if (text != null)
                    return text;
            }

            GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(-24f, 24f);

            text = textObject.GetComponent<Text>();
            text.font = _defaultFont;
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = content;
            return text;
        }

        private Text FindText(string objectName)
        {
            Transform target = transform.Find(objectName);
            if (target != null)
                return target.GetComponent<Text>();

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
