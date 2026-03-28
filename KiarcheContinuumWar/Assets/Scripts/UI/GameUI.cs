using UnityEngine;
using UnityEngine.UI;
using KiarcheContinuumWar.Core;
using KiarcheContinuumWar.Units;

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
        public GameObject unitPanel;

        [Header("References")]
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private UnitController unitController;

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
            if (selectedUnitsText == null) return;

            int selectedCount = unitController.SelectedUnits.Count;
            if (selectedCount > 0)
            {
                selectedUnitsText.text = $"Выделено: {selectedCount}";
                
                // Показать панель юнитов
                if (unitPanel != null)
                    unitPanel.SetActive(true);
            }
            else
            {
                selectedUnitsText.text = "Нет выделенных";
                
                // Скрыть панель юнитов
                if (unitPanel != null)
                    unitPanel.SetActive(false);
            }
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
