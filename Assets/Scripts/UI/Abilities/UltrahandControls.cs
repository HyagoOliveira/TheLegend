using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class UltrahandControls : AbstractAbility<UltrahandSettings>
    {
        [SerializeField] private Canvas canvas;

        [Space]
        [SerializeField] private GameObject searching;
        [SerializeField] private GameObject interacting;
        [SerializeField] private GameObject unstick;

        private void Reset() => canvas = GetComponent<Canvas>();

        protected override void OnEnable()
        {
            base.OnEnable();
            settings.OnInteractionStarted += HandleInteractionStarted;
            settings.OnInteractionCanceled += HandleInteractionCanceled;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            settings.OnInteractionStarted -= HandleInteractionStarted;
            settings.OnInteractionCanceled -= HandleInteractionCanceled;
        }

        protected override void HandleToggled(bool enabled)
        {
            canvas.enabled = enabled;
            searching.SetActive(true);
            unstick.SetActive(false);
            interacting.SetActive(false);
        }

        private void HandleInteractionStarted(IUltrahandable _)
        {
            searching.SetActive(false);
            unstick.SetActive(false);
            interacting.SetActive(true);
        }

        private void HandleInteractionCanceled(IUltrahandable _)
        {
            searching.SetActive(false);
            unstick.SetActive(false);
            interacting.SetActive(false);
        }
    }
}