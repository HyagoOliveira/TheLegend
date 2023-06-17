using System;
using UnityEngine;

namespace TheLegend.Abilities
{
    [CreateAssetMenu(fileName = "UltrahandSettings", menuName = "TheLegend/Abilities/Ultrahand Settings", order = 110)]
    public sealed class UltrahandSettings : AbstractAbilitySettings
    {
        [field: SerializeField] public float Speed { get; private set; } = 10F;

        public event Action<bool> OnSelectionChanged;
        public event Action<IUltrahandable> OnInteractionStarted;
        public event Action<IUltrahandable> OnInteractionCanceled;

        internal void StartInteraction(IUltrahandable ultrahandable) => OnInteractionStarted?.Invoke(ultrahandable);
        internal void CancelInteraction(IUltrahandable ultrahandable) => OnInteractionCanceled?.Invoke(ultrahandable);

        internal override void HandleHitChanged(RaycastHit hit)
        {
            var hasUltrahandable =
                hit.transform != null &&
                hit.transform.TryGetComponent(out IUltrahandable ultrahandable);

            OnSelectionChanged?.Invoke(hasUltrahandable);
        }
    }
}