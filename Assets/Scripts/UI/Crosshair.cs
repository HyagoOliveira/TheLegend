using UnityEngine;
using UnityEngine.UI;
using TheLegend.Abilities;

namespace TheLegend.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasViewer))]
    public sealed class Crosshair : MonoBehaviour
    {
        [SerializeField] private UltrahandSettings settings;
        [field: SerializeField] public CanvasViewer Viewer { get; private set; }
        [SerializeField] private Image[] lines;
        [SerializeField] private Image[] arrows;

        private void Reset() => Viewer = GetComponent<CanvasViewer>();
        private void Awake() => Unselect();

        private void OnEnable() => settings.OnSelectionChanged += HandleSelectionChanged;
        private void OnDisable() => settings.OnSelectionChanged -= HandleSelectionChanged;

        public void Select()
        {
            SetLinesEnabled(false);
            SetArrowsEnabled(true);
        }

        public void Unselect()
        {
            SetLinesEnabled(true);
            SetArrowsEnabled(false);
        }

        private void SetLinesEnabled(bool enabled) => SetImagesEnabled(lines, enabled);
        private void SetArrowsEnabled(bool enabled) => SetImagesEnabled(arrows, enabled);

        private void HandleSelectionChanged(bool isAvailable)
        {
            if (isAvailable) Select();
            else Unselect();
        }

        private static void SetImagesEnabled(Image[] images, bool enabled)
        {
            foreach (var image in images)
            {
                image.enabled = enabled;
            }
        }
    }
}