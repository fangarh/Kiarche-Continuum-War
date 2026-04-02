using System.Linq;
using UnityEngine;
using KiarcheContinuumWar.Map;
using KiarcheContinuumWar.Units;

namespace KiarcheContinuumWar.InputSystem
{
    /// <summary>
    /// Обработка ввода для RTS управления.
    /// Выделение юнитов, перемещение и атака.
    /// </summary>
    public class RTSInput : MonoBehaviour
    {
        [Header("References")]
        public Camera mainCamera;
        public UnitController unitController;

        [Header("Selection Settings")]
        [SerializeField] private float minDragDistance = 10f;
        [SerializeField] private Color dragColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private float clickRayDistance = 2000f;

        private Vector2 _startMousePosition;
        private Vector2 _currentMousePosition;
        private bool _isDragging;
        private Rect _selectionRect;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (unitController == null)
            {
                unitController = FindAnyObjectByType<UnitController>();
            }
        }

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startMousePosition = Input.mousePosition;
                _currentMousePosition = Input.mousePosition;
                _isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                _currentMousePosition = Input.mousePosition;
                float dragDistance = Vector2.Distance(_currentMousePosition, _startMousePosition);
                if (dragDistance > minDragDistance)
                {
                    _isDragging = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                {
                    EndDragSelection();
                }
                else
                {
                    HandleClick();
                }

                _isDragging = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
        }

        private void HandleClick()
        {
            if (TryGetTopmostUnitUnderMouse(Input.mousePosition, out Unit unit))
            {
                unitController.SelectUnit(unit);
                return;
            }

            unitController.DeselectAll();
        }

        private void HandleRightClick()
        {
            if (unitController == null || unitController.SelectedUnits.Count == 0)
            {
                return;
            }

            Vector2 pointerPosition = Input.mousePosition;

            if (TryGetTopmostUnitUnderMouse(pointerPosition, out Unit targetUnit) &&
                !unitController.SelectedUnits.Contains(targetUnit))
            {
                unitController.IssueAttackOrder(targetUnit);
                return;
            }

            if (!TryGetGroundPosition(pointerPosition, out Vector3 targetPosition))
            {
                return;
            }

            unitController.IssueMoveOrder(targetPosition);
        }

        private bool TryGetTopmostUnitUnderMouse(Vector2 mousePosition, out Unit unit)
        {
            unit = null;

            if (mainCamera == null)
            {
                return false;
            }

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(
                    ray,
                    clickRayDistance,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore)
                .OrderBy(hit => hit.distance)
                .ToArray();

            foreach (RaycastHit hit in hits)
            {
                unit = hit.collider.GetComponent<Unit>()
                    ?? hit.collider.GetComponentInParent<Unit>()
                    ?? hit.collider.GetComponentInChildren<Unit>();

                if (unit != null && unit.IsAlive)
                {
                    return true;
                }
            }

            unit = null;
            return false;
        }

        private bool TryGetGroundPosition(Vector2 mousePosition, out Vector3 groundPosition)
        {
            groundPosition = Vector3.zero;

            if (mainCamera == null)
            {
                return false;
            }

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(
                    ray,
                    clickRayDistance,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore)
                .OrderBy(hit => hit.distance)
                .ToArray();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider is TerrainCollider)
                {
                    groundPosition = hit.point;
                    return true;
                }
            }

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (!groundPlane.Raycast(ray, out float enter))
            {
                return false;
            }

            groundPosition = ProjectPointToTerrain(ray.GetPoint(enter));
            return true;
        }

        private Vector3 ProjectPointToTerrain(Vector3 position)
        {
            MapManager mapManager = MapManager.Instance;
            if (mapManager == null)
            {
                return position;
            }

            position = mapManager.ClampToBounds(position);
            return new Vector3(position.x, mapManager.GetTerrainHeight(position), position.z);
        }

        private void EndDragSelection()
        {
            float startX = _startMousePosition.x;
            float startY = Screen.height - _startMousePosition.y;
            float endX = _currentMousePosition.x;
            float endY = Screen.height - _currentMousePosition.y;

            float x = Mathf.Min(startX, endX);
            float y = Mathf.Min(startY, endY);
            float width = Mathf.Abs(endX - startX);
            float height = Mathf.Abs(endY - startY);

            _selectionRect = new Rect(x, y, width, height);
            unitController.SelectUnitsInRect(_selectionRect, mainCamera);
        }

        private void OnGUI()
        {
            if (!_isDragging)
            {
                return;
            }

            float startX = _startMousePosition.x;
            float startY = Screen.height - _startMousePosition.y;
            float endX = _currentMousePosition.x;
            float endY = Screen.height - _currentMousePosition.y;

            float x = Mathf.Min(startX, endX);
            float y = Mathf.Min(startY, endY);
            float width = Mathf.Abs(endX - startX);
            float height = Mathf.Abs(endY - startY);

            Rect rect = new Rect(x, y, width, height);

            Color originalColor = GUI.color;
            GUI.color = dragColor;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = originalColor;

            GUI.Box(rect, string.Empty, GetSelectionStyle());
        }

        private GUIStyle GetSelectionStyle()
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            style.normal.textColor = Color.green;
            return style;
        }
    }
}
