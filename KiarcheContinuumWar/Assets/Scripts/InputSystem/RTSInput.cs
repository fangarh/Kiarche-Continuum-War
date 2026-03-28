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
        [SerializeField] private float minDragDistance = 10f;
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
            
            Debug.Log("[RTSInput] Инициализирован");
        }

        private void Update()
        {
            // Левая кнопка мыши - выделение
            if (Input.GetMouseButtonDown(0))
            {
                _startMousePosition = Input.mousePosition;
                _currentMousePosition = Input.mousePosition;
                _isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                _currentMousePosition = Input.mousePosition;
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
                    // Выделение рамкой
                    EndDragSelection();
                }
                else
                {
                    // Клик - выделение одного юнита или снятие выделения
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

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // Ищем юнита: проверяем hit.collider.gameObject и его родителей
                Unit unit = hit.collider.GetComponent<Unit>();

                if (unit == null)
                {
                    unit = hit.collider.GetComponentInParent<Unit>();
                }

                // Если всё ещё null, проверяем children (вдруг попали в родителя)
                if (unit == null)
                {
                    unit = hit.collider.GetComponentInChildren<Unit>();
                }

                if (unit != null && unit.IsAlive)
                {
                    // Клик по юниту — выделить только его (заменить выделение)
                    unitController.SelectUnit(unit);
                }
                else
                {
                    // Клик по земле — снять выделение
                    unitController.DeselectAll();
                }
            }
            else
            {
                unitController.DeselectAll();
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
            // Преобразуем координаты для SelectUnitsInRect (GUI использует Y от верха)
            float startX = _startMousePosition.x;
            float startY = Screen.height - _startMousePosition.y;
            float endX = _currentMousePosition.x;
            float endY = Screen.height - _currentMousePosition.y;

            float x = Mathf.Min(startX, endX);
            float y = Mathf.Min(startY, endY);
            float width = Mathf.Abs(endX - startX);
            float height = Mathf.Abs(endY - startY);

            _selectionRect = new Rect(x, y, width, height);
            
            Debug.Log($"[RTSInput] Selection rect: {_selectionRect}");
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
