using UnityEngine;
using TheLegend.Players;
using ActionCode.Physics;
using ActionCode.VisualEffects;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AbstractCaster))]
    [RequireComponent(typeof(HighlightableDetector))]
    public sealed class AbilityHighlightHandler : MonoBehaviour
    {
        [SerializeField] private AbstractCaster caster;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private HighlightableDetector detector;
        [SerializeField] private PlayerSettings playerSettings;

        private void Reset()
        {
            caster = GetComponent<AbstractCaster>();
            audioSource = GetComponent<AudioSource>();
            detector = GetComponent<HighlightableDetector>();
        }
        private void Awake() => playerSettings.AbilityCaster = caster;

        private void OnEnable()
        {
            playerSettings.Ultrahand.OnToggled += HandleToggled;
            playerSettings.Ultrahand.OnSelectionChanged += HandleSelectionChanged;
            playerSettings.Ultrahand.OnInteractionStarted += HandleObjectInteractionStarted;
            caster.OnHitChanged += playerSettings.Ultrahand.HandleHitChanged;
        }

        private void OnDisable()
        {
            playerSettings.Ultrahand.OnToggled -= HandleToggled;
            playerSettings.Ultrahand.OnSelectionChanged -= HandleSelectionChanged;
            playerSettings.Ultrahand.OnInteractionStarted -= HandleObjectInteractionStarted;
            caster.OnHitChanged -= playerSettings.Ultrahand.HandleHitChanged;
        }

        private void HandleObjectInteractionStarted(IUltrahandable _) => detector.enabled = false;

        private void HandleToggled(bool enabled) => detector.enabled = enabled;

        private void HandleSelectionChanged(bool enabled)
        {
            if (enabled) audioSource.Play();
        }
    }
}