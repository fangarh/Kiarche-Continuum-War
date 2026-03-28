using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Контроллер группы юнитов.
    /// Управляет выделением, перемещением и атакой группы.
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float selectionRadius = 0.5f;
        [SerializeField] private float formationSpacing = 1.5f;

        // Выделенные юниты
        private List<Unit> _selectedUnits = new List<Unit>();
        public IReadOnlyList<Unit> SelectedUnits => _selectedUnits;

        // Все юниты на сцене
        private List<Unit> _allUnits = new List<Unit>();

        // Позиция для атаки/перемещения
        private Vector3 _targetPosition;
        private Unit _targetUnit;

        public bool HasTargetPosition => _targetPosition != Vector3.zero;
        public bool HasTargetUnit => _targetUnit != null && _targetUnit.IsAlive;

        private void Start()
        {
            // Найти все юниты на сцене
            FindAllUnits();
        }

        /// <summary>
        /// Найти все юниты на сцене.
        /// </summary>
        public void FindAllUnits()
        {
            _allUnits.Clear();
            Unit[] units = FindObjectsByType<Unit>(FindObjectsInactive.Include);
            _allUnits.AddRange(units);

            // Подписаться на события смерти
            foreach (var unit in _allUnits)
            {
                unit.OnUnitDied += HandleUnitDied;
            }
        }

        /// <summary>
        /// Выделить одного юнита.
        /// </summary>
        public void SelectUnit(Unit unit)
        {
            DeselectAll();
            AddToSelection(unit);
        }

        /// <summary>
        /// Добавить юнита к выделенным.
        /// </summary>
        public void AddToSelection(Unit unit)
        {
            if (!_selectedUnits.Contains(unit))
            {
                _selectedUnits.Add(unit);
                unit.Select();
            }
        }

        /// <summary>
        /// Снять выделение с юнита.
        /// </summary>
        public void RemoveFromSelection(Unit unit)
        {
            if (_selectedUnits.Contains(unit))
            {
                _selectedUnits.Remove(unit);
                unit.Deselect();
            }
        }

        /// <summary>
        /// Снять выделение со всех юнитов.
        /// </summary>
        public void DeselectAll()
        {
            foreach (var unit in _selectedUnits)
            {
                unit.Deselect();
            }
            _selectedUnits.Clear();
        }

        /// <summary>
        /// Выделить юнитов в прямоугольной области.
        /// </summary>
        public void SelectUnitsInRect(Rect rect, Camera camera)
        {
            DeselectAll();

            foreach (var unit in _allUnits)
            {
                if (!unit.IsAlive) continue;

                Vector3 screenPoint = camera.WorldToScreenPoint(unit.transform.position);

                // Проверка: юнит перед камерой
                if (screenPoint.z < 0) continue;

                // Проверка: экранные координаты в пределах экрана
                if (screenPoint.x < 0 || screenPoint.x > Screen.width ||
                    screenPoint.y < 0 || screenPoint.y > Screen.height)
                {
                    continue;
                }

                // WorldToScreenPoint использует Y от низа (0..Screen.height)
                // rect использует Y от верха (0..Screen.height), поэтому инвертируем
                float invertedY = Screen.height - screenPoint.y;

                // Проверка: юнит в пределах rect
                if (screenPoint.x >= rect.xMin &&
                    screenPoint.x <= rect.xMax &&
                    invertedY >= rect.yMin &&
                    invertedY <= rect.yMax)
                {
                    AddToSelection(unit);
                }
            }
        }

        /// <summary>
        /// Отдать приказ о перемещении.
        /// </summary>
        public void IssueMoveOrder(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _targetUnit = null;

            if (_selectedUnits.Count == 0) return;

            // Распределить юнитов в формации
            IssueFormationMove(targetPosition);
        }

        /// <summary>
        /// Отдать приказ об атаке позиции.
        /// </summary>
        public void IssueAttackOrder(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _targetUnit = null;

            // Найти ближайшего врага к позиции
            Unit nearestEnemy = FindNearestEnemy(targetPosition);
            if (nearestEnemy != null)
            {
                IssueAttackOrder(nearestEnemy);
            }
        }

        /// <summary>
        /// Отдать приказ об атаке юнита.
        /// </summary>
        public void IssueAttackOrder(Unit targetUnit)
        {
            _targetUnit = targetUnit;
            _targetPosition = Vector3.zero;

            if (_selectedUnits.Count == 0) return;

            // Все юниты атакуют цель
            foreach (var unit in _selectedUnits)
            {
                unit.SetTarget(targetUnit);
            }
        }

        /// <summary>
        /// Найти ближайшего врага к позиции.
        /// </summary>
        public Unit FindNearestEnemy(Vector3 position)
        {
            Unit nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var unit in _allUnits)
            {
                if (!unit.IsAlive) continue;
                if (_selectedUnits.Contains(unit)) continue; // Не атаковать своих

                float distance = Vector3.Distance(position, unit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = unit;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Распределить юнитов в формации.
        /// </summary>
        private void IssueFormationMove(Vector3 targetPosition)
        {
            int count = _selectedUnits.Count;
            if (count == 0) return;

            // Простая формация: линия или круг
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = GetFormationOffset(i, count);
                Vector3 finalPosition = targetPosition + offset;
                _selectedUnits[i].SetTargetPosition(finalPosition);
            }
        }

        /// <summary>
        /// Получить смещение для формации.
        /// </summary>
        private Vector3 GetFormationOffset(int index, int totalCount)
        {
            if (totalCount == 1) return Vector3.zero;

            // Расположить по кругу
            float angle = (index / (float)totalCount) * Mathf.PI * 2;
            float radius = formationSpacing * Mathf.Sqrt(totalCount);
            
            return new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
        }

        private void HandleUnitDied(Unit unit)
        {
            RemoveFromSelection(unit);
            _allUnits.Remove(unit);
        }

        private void OnDrawGizmosSelected()
        {
            // Отобразить выделенных юнитов
            foreach (var unit in _selectedUnits)
            {
                if (unit == null) continue;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(unit.transform.position, selectionRadius);
            }
        }
    }
}
