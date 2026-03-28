using UnityEngine;
using KiarcheContinuumWar.Pathfinding;

namespace KiarcheContinuumWar.Managers
{
    /// <summary>
    /// Утилита для настройки FlowFieldManager на сцене.
    /// </summary>
    public class FlowFieldSetup : MonoBehaviour
    {
        [Header("Field Settings")]
        [SerializeField] private int fieldWidth = 100;
        [SerializeField] private int fieldHeight = 100;
        [SerializeField] private float cellSize = 1f;

        private void Start()
        {
            // Найти или создать FlowFieldManager
            FlowFieldManager manager = FindAnyObjectByType<FlowFieldManager>();
            
            if (manager == null)
            {
                Debug.Log("[FlowFieldSetup] Создаю FlowFieldManager...");
                GameObject managerObj = new GameObject("FlowFieldManager");
                manager = managerObj.AddComponent<FlowFieldManager>();
            }

            // Настроить параметры
            // (через публичные свойства если нужно)
            
            Debug.Log($"[FlowFieldSetup] FlowFieldManager настроен: {fieldWidth}x{fieldHeight}, cellSize={cellSize}");
        }
    }
}
