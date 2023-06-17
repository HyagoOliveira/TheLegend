using UnityEngine;

namespace TheLegend.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class CanvasViewer : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private void Reset() => canvas = GetComponent<Canvas>();

        public void Show() => canvas.enabled = true;
        public void Hide() => canvas.enabled = false;
        public void Toggle(bool enabled) => canvas.enabled = enabled;
    }
}