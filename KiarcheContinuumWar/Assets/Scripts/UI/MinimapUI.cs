using UnityEngine;
using UnityEngine.UI;
using KiarcheContinuumWar.Map;
using KiarcheContinuumWar.Units;
using System.Collections.Generic;

namespace KiarcheContinuumWar.UI
{
    /// <summary>
    /// Простая миникарта с маркерами юнитов.
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private RectTransform markersRoot;
        [SerializeField] private Image markerPrefab;

        [Header("References")]
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private MapManager mapManager;

        private readonly List<Image> _markers = new List<Image>();

        private void Start()
        {
            if (mapManager == null)
                mapManager = FindAnyObjectByType<MapManager>();
        }

        private void LateUpdate()
        {
            if (mapManager == null || markersRoot == null || markerPrefab == null)
            {
                return;
            }

            Unit[] units = FindObjectsByType<Unit>(FindObjectsInactive.Exclude);
            EnsureMarkerPool(units.Length);

            for (int i = 0; i < _markers.Count; i++)
            {
                bool active = i < units.Length;
                _markers[i].gameObject.SetActive(active);
                if (!active)
                    continue;

                UpdateMarker(_markers[i].rectTransform, units[i]);
            }
        }

        public void SetMinimapTexture(RenderTexture texture)
        {
            if (minimapImage != null)
                minimapImage.texture = texture;
        }

        public void SetMinimapCamera(Camera camera)
        {
            minimapCamera = camera;
        }

        private void EnsureMarkerPool(int requiredCount)
        {
            while (_markers.Count < requiredCount)
            {
                Image marker = Instantiate(markerPrefab, markersRoot);
                RectTransform markerRect = marker.rectTransform;
                markerRect.anchorMin = Vector2.zero;
                markerRect.anchorMax = Vector2.zero;
                markerRect.pivot = new Vector2(0.5f, 0.5f);
                marker.gameObject.SetActive(false);
                _markers.Add(marker);
            }
        }

        private void UpdateMarker(RectTransform markerTransform, Unit unit)
        {
            Vector3 position = unit.transform.position;
            float normalizedX = Mathf.Clamp01(Mathf.InverseLerp(mapManager.MapOrigin.x, mapManager.MapOrigin.x + mapManager.MapWidth, position.x));
            float normalizedY = Mathf.Clamp01(Mathf.InverseLerp(mapManager.MapOrigin.z, mapManager.MapOrigin.z + mapManager.MapHeight, position.z));

            Rect rect = markersRoot.rect;
            float x = normalizedX * rect.width;
            float y = normalizedY * rect.height;
            markerTransform.anchoredPosition = new Vector2(x, y);

            Image marker = markerTransform.GetComponent<Image>();
            marker.color = GetUnitColor(unit);
        }

        private Color GetUnitColor(Unit unit)
        {
            Renderer renderer = unit.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                return renderer.sharedMaterial.color;
            }

            return Color.white;
        }
    }
}
