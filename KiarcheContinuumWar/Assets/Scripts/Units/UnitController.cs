using System.Collections.Generic;
using UnityEngine;
using KiarcheContinuumWar.Map;

namespace KiarcheContinuumWar.Units
{
    /// <summary>
    /// Контроллер группы юнитов.
    /// Управляет выделением, перемещением и атакой.
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float selectionRadius = 0.5f;
        [SerializeField] private float formationSpacing = 1.5f;

        private readonly List<Unit> _selectedUnits = new List<Unit>();
        public IReadOnlyList<Unit> SelectedUnits => _selectedUnits;

        private readonly List<Unit> _allUnits = new List<Unit>();

        private Vector3 _targetPosition;
        private Unit _targetUnit;

        public bool HasTargetPosition => _targetPosition != Vector3.zero;
        public bool HasTargetUnit => _targetUnit != null && _targetUnit.IsAlive;

        private void Start()
        {
            FindAllUnits();
        }

        public void FindAllUnits()
        {
            _allUnits.Clear();
            Unit[] units = FindObjectsByType<Unit>(FindObjectsInactive.Include);
            _allUnits.AddRange(units);

            foreach (Unit unit in _allUnits)
            {
                unit.OnUnitDied -= HandleUnitDied;
                unit.OnUnitDied += HandleUnitDied;
            }
        }

        public void SelectUnit(Unit unit)
        {
            DeselectAll();
            AddToSelection(unit);
        }

        public void AddToSelection(Unit unit)
        {
            if (unit == null || _selectedUnits.Contains(unit))
            {
                return;
            }

            _selectedUnits.Add(unit);
            unit.Select();
        }

        public void RemoveFromSelection(Unit unit)
        {
            if (unit == null || !_selectedUnits.Contains(unit))
            {
                return;
            }

            _selectedUnits.Remove(unit);
            unit.Deselect();
        }

        public void DeselectAll()
        {
            foreach (Unit unit in _selectedUnits)
            {
                unit.Deselect();
            }

            _selectedUnits.Clear();
        }

        public void SelectUnitsInRect(Rect rect, Camera camera)
        {
            DeselectAll();

            foreach (Unit unit in _allUnits)
            {
                if (!unit.IsAlive)
                {
                    continue;
                }

                Vector3 screenPoint = camera.WorldToScreenPoint(unit.transform.position);
                if (screenPoint.z < 0f)
                {
                    continue;
                }

                if (screenPoint.x < 0f || screenPoint.x > Screen.width ||
                    screenPoint.y < 0f || screenPoint.y > Screen.height)
                {
                    continue;
                }

                float invertedY = Screen.height - screenPoint.y;
                if (screenPoint.x >= rect.xMin &&
                    screenPoint.x <= rect.xMax &&
                    invertedY >= rect.yMin &&
                    invertedY <= rect.yMax)
                {
                    AddToSelection(unit);
                }
            }
        }

        public void IssueMoveOrder(Vector3 targetPosition)
        {
            MapManager mapManager = MapManager.Instance;
            if (mapManager != null)
            {
                targetPosition = mapManager.ClampToBounds(targetPosition);
                targetPosition = mapManager.GetPositionOnTerrain(targetPosition.x, targetPosition.z);
            }

            _targetPosition = targetPosition;
            _targetUnit = null;

            if (_selectedUnits.Count == 0)
            {
                return;
            }

            var flowFieldManager = FindAnyObjectByType<KiarcheContinuumWar.Pathfinding.FlowFieldManager>();
            flowFieldManager?.GenerateFlowField(targetPosition);
            IssueFormationMove(targetPosition);
        }

        public void IssueAttackOrder(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _targetUnit = null;

            Unit nearestEnemy = FindNearestEnemy(targetPosition);
            if (nearestEnemy != null)
            {
                IssueAttackOrder(nearestEnemy);
            }
        }

        public void IssueAttackOrder(Unit targetUnit)
        {
            _targetUnit = targetUnit;
            _targetPosition = Vector3.zero;

            if (_selectedUnits.Count == 0)
            {
                return;
            }

            foreach (Unit unit in _selectedUnits)
            {
                unit.SetTarget(targetUnit);
            }
        }

        public Unit FindNearestEnemy(Vector3 position)
        {
            Unit nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Unit unit in _allUnits)
            {
                if (!unit.IsAlive || _selectedUnits.Contains(unit))
                {
                    continue;
                }

                float distance = Vector3.Distance(position, unit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = unit;
                }
            }

            return nearest;
        }

        private void IssueFormationMove(Vector3 targetPosition)
        {
            int count = _selectedUnits.Count;
            if (count == 0)
            {
                return;
            }

            MapManager mapManager = MapManager.Instance;

            Vector3[] offsets = new Vector3[count];
            Vector3 centroid = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                offsets[i] = GetFormationOffset(i, count);
                centroid += offsets[i];
            }

            centroid /= count;

            List<Vector3> availableSlots = new List<Vector3>(count);

            for (int i = 0; i < count; i++)
            {
                Vector3 centeredOffset = offsets[i] - centroid;
                Vector3 slot = targetPosition + centeredOffset;
                if (mapManager != null)
                {
                    slot = mapManager.ClampToBounds(slot);
                    slot = mapManager.GetPositionOnTerrain(slot.x, slot.z);
                }

                availableSlots.Add(slot);
            }

            List<Unit> remainingUnits = new List<Unit>(_selectedUnits);
            while (remainingUnits.Count > 0 && availableSlots.Count > 0)
            {
                int bestUnitIndex = 0;
                int bestSlotIndex = 0;
                float bestDistance = float.MaxValue;

                for (int unitIndex = 0; unitIndex < remainingUnits.Count; unitIndex++)
                {
                    Vector3 unitPosition = remainingUnits[unitIndex].transform.position;

                    for (int slotIndex = 0; slotIndex < availableSlots.Count; slotIndex++)
                    {
                        float distance = Vector3.SqrMagnitude(unitPosition - availableSlots[slotIndex]);
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestUnitIndex = unitIndex;
                            bestSlotIndex = slotIndex;
                        }
                    }
                }

                Unit selectedUnit = remainingUnits[bestUnitIndex];
                Vector3 assignedSlot = availableSlots[bestSlotIndex];
                selectedUnit.SetTargetPosition(assignedSlot, false);

                remainingUnits.RemoveAt(bestUnitIndex);
                availableSlots.RemoveAt(bestSlotIndex);
            }
        }

        private Vector3 GetFormationOffset(int index, int totalCount)
        {
            if (totalCount == 1)
            {
                return Vector3.zero;
            }

            float angle;
            float radius;

            if (totalCount <= 6)
            {
                angle = (index / (float)totalCount) * Mathf.PI * 2f;
                radius = formationSpacing * Mathf.Sqrt(totalCount) * 0.8f;
            }
            else
            {
                int ring = Mathf.FloorToInt(Mathf.Sqrt(index));
                int indexInRing = index - ring * ring;
                int countInRing = ring == 0 ? 1 : ring * 6;

                angle = (indexInRing / (float)countInRing) * Mathf.PI * 2f;
                radius = formationSpacing * (ring + 1);
            }

            return new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius);
        }

        private void HandleUnitDied(Unit unit)
        {
            RemoveFromSelection(unit);
            _allUnits.Remove(unit);
        }

        private void OnDrawGizmosSelected()
        {
            foreach (Unit unit in _selectedUnits)
            {
                if (unit == null)
                {
                    continue;
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(unit.transform.position, selectionRadius);
            }
        }
    }
}
