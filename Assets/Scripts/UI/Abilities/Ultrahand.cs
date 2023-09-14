using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        [SerializeField] private Crosshair crosshair;
        [SerializeField] private AudioSource audioSource;

        private void Reset()
        {
            crosshair = GetComponentInChildren<Crosshair>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Awake() => crosshair.Viewer.Hide();

        protected override void OnEnable()
        {
            base.OnEnable();

            settings.OnObjectRotated += HandleObjectRotated;
            settings.OnInteractionStarted += HandleObjectInteractionStarted;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            settings.OnObjectRotated -= HandleObjectRotated;
            settings.OnInteractionStarted -= HandleObjectInteractionStarted;
        }

        private void HandleObjectRotated() => audioSource.Play();

        private void HandleObjectInteractionStarted(IUltrahandable _) => HandleToggled(false);

        protected override void HandleToggled(bool enabled) => crosshair.Viewer.Toggle(enabled);
    }
}