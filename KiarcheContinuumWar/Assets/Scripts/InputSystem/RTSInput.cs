using UnityEngine;
using KiarcheContinuumWar.Units;
using System.Linq;

namespace KiarcheContinuumWar.InputSystem
{
    /// <summary>
    /// Обработка ввода для RTS управления (Input Manager).
    /// Выделение юнитов, перемещение, атака.
    /// </summary>
    public class RTSInput : MonoBehaviour
    {
        [Header("References")]
        public Camera mainCamera;
        public UnitController unitController;

        [Header("Selection Settings")]
        [SerializeField] private float minDragDistance = 5f;
        [SerializeField] private Color dragColor = new Color(0, 1, 0, 0.3f);

        // Состояние ввода
        private Vector2 _startMousePosition;
        private Vector2 _currentMousePosition;
        private bool _isDragging = false;

        // Rect выделения
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

        private void Update()
        {
            _currentMousePosition = Input.mousePosition;

            // Левая кнопка мыши - выделение
            if (Input.GetMouseButtonDown(0))
            {
                _startMousePosition = Input.mousePosition;
                _isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                float dragDistance = Vector2.Distance(Input.mousePosition, _startMousePosition);
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

            // Правая кнопка мыши - приказ
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
        }

        private void HandleClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Unit unit = hit.collider.GetComponent<Unit>();
                if (unit == null)
                {
                    unit = hit.collider.GetComponentInParent<Unit>();
                }
                
                if (unit != null && unit.IsAlive)
                {
                    unitController.SelectUnit(unit);
                }
                else
                {
                    unitController.DeselectAll();
                }
            }
        }

        private void HandleRightClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Unit unit = hit.collider.GetComponent<Unit>();
                if (unit != null && unit.IsAlive)
                {
                    if (!unitController.SelectedUnits.Contains(unit))
                    {
                        unitController.IssueAttackOrder(unit);
                    }
                }
                else
                {
                    unitController.IssueMoveOrder(hit.point);
                }
            }
        }

        private void EndDragSelection()
        {
            // Input.mousePosition и WorldToScreenPoint используют одинаковые координаты (Y от низа)
            float x = Mathf.Min(_startMousePosition.x, _currentMousePosition.x);
            float y = Mathf.Min(_startMousePosition.y, _currentMousePosition.y);
            float width = Mathf.Abs(_currentMousePosition.x - _startMousePosition.x);
            float height = Mathf.Abs(_currentMousePosition.y - _startMousePosition.y);

            _selectionRect = new Rect(x, y, width, height);
            unitController.SelectUnitsInRect(_selectionRect, mainCamera);
        }

        private void OnGUI()
        {
            if (_isDragging)
            {
                // GUI использует Y от верха экрана, инвертируем
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

                GUI.Box(rect, "", GetSelectionStyle());
            }
        }

        private GUIStyle GetSelectionStyle()
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = Texture2D.whiteTexture;
            style.normal.textColor = Color.green;
            return style;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && mainCamera != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(ray.origin, ray.direction * 100);
            }
        }
    }
}
