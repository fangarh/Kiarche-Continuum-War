using System;
using UnityEngine;

namespace KiarcheContinuumWar.Core
{
    /// <summary>
    /// Типы ресурсов в игре.
    /// </summary>
    public enum ResourceType
    {
        Materials,    // Материалы
        Energy,       // Энергия
        Food,         // Еда
        Knowledge     // Знания
    }

    /// <summary>
    /// Менеджер ресурсов игрока.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private int startingMaterials = 100;
        [SerializeField] private int startingEnergy = 100;
        [SerializeField] private int startingFood = 100;
        [SerializeField] private int startingKnowledge = 50;

        // Текущие ресурсы
        private int _materials;
        private int _energy;
        private int _food;
        private int _knowledge;

        // Доход ресурсов в секунду
        private float _materialsPerSecond;
        private float _energyPerSecond;
        private float _foodPerSecond;
        private float _knowledgePerSecond;

        // События
        public event Action<ResourceType, int> OnResourceChanged;
        public event Action<ResourceType, float> OnResourceRateChanged;

        public int Materials => _materials;
        public int Energy => _energy;
        public int Food => _food;
        public int Knowledge => _knowledge;

        private void Start()
        {
            _materials = startingMaterials;
            _energy = startingEnergy;
            _food = startingFood;
            _knowledge = startingKnowledge;
        }

        private void Update()
        {
            // Пассивный доход ресурсов
            if (_materialsPerSecond > 0)
                AddResource(ResourceType.Materials, Mathf.FloorToInt(_materialsPerSecond * Time.deltaTime));
            
            if (_energyPerSecond > 0)
                AddResource(ResourceType.Energy, Mathf.FloorToInt(_energyPerSecond * Time.deltaTime));
            
            if (_foodPerSecond > 0)
                AddResource(ResourceType.Food, Mathf.FloorToInt(_foodPerSecond * Time.deltaTime));
            
            if (_knowledgePerSecond > 0)
                AddResource(ResourceType.Knowledge, Mathf.FloorToInt(_knowledgePerSecond * Time.deltaTime));
        }

        /// <summary>
        /// Добавить ресурс.
        /// </summary>
        public void AddResource(ResourceType type, int amount)
        {
            int oldValue = GetResource(type);
            int newValue = oldValue + amount;
            SetResource(type, newValue);
        }

        /// <summary>
        /// Потратить ресурс. Возвращает true если успешно.
        /// </summary>
        public bool SpendResource(ResourceType type, int amount)
        {
            int currentValue = GetResource(type);
            if (currentValue >= amount)
            {
                SetResource(type, currentValue - amount);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверить достаточно ли ресурсов.
        /// </summary>
        public bool CanAfford(ResourceType type, int amount)
        {
            return GetResource(type) >= amount;
        }

        /// <summary>
        /// Установить доход ресурса в секунду.
        /// </summary>
        public void SetResourceRate(ResourceType type, float perSecond)
        {
            switch (type)
            {
                case ResourceType.Materials:
                    _materialsPerSecond = perSecond;
                    break;
                case ResourceType.Energy:
                    _energyPerSecond = perSecond;
                    break;
                case ResourceType.Food:
                    _foodPerSecond = perSecond;
                    break;
                case ResourceType.Knowledge:
                    _knowledgePerSecond = perSecond;
                    break;
            }
            OnResourceRateChanged?.Invoke(type, perSecond);
        }

        private int GetResource(ResourceType type)
        {
            return type switch
            {
                ResourceType.Materials => _materials,
                ResourceType.Energy => _energy,
                ResourceType.Food => _food,
                ResourceType.Knowledge => _knowledge,
                _ => 0
            };
        }

        private void SetResource(ResourceType type, int amount)
        {
            int oldValue = GetResource(type);
            
            switch (type)
            {
                case ResourceType.Materials:
                    _materials = Mathf.Max(0, amount);
                    break;
                case ResourceType.Energy:
                    _energy = Mathf.Max(0, amount);
                    break;
                case ResourceType.Food:
                    _food = Mathf.Max(0, amount);
                    break;
                case ResourceType.Knowledge:
                    _knowledge = Mathf.Max(0, amount);
                    break;
            }

            int newValue = GetResource(type);
            if (oldValue != newValue)
            {
                OnResourceChanged?.Invoke(type, newValue);
            }
        }
    }
}
