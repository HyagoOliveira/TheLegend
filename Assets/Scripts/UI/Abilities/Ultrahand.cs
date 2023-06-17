using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.UI
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        [SerializeField] private Crosshair crosshair;

        private void Reset() => crosshair = GetComponentInChildren<Crosshair>();
        private void Awake() => crosshair.Viewer.Hide();

        protected override void OnEnable()
        {
            base.OnEnable();
            settings.OnInteractionStarted += HandleObjectInteractionStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            settings.OnInteractionStarted -= HandleObjectInteractionStarted;
        }

        private void HandleObjectInteractionStarted(IUltrahandable _) => HandleToggled(false);

        protected override void HandleToggled(bool enabled) => crosshair.Viewer.Toggle(enabled);
    }
}