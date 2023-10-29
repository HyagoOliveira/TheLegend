using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        [field: SerializeField] public Transform Hand { get; private set; }
        [field: SerializeField] public Transform Holder { get; private set; }

        [SerializeField] private GameObject directionalIndicator;

        public bool IsShowingDirectionalIndicator
        {
            get => directionalIndicator.activeInHierarchy;
            set => directionalIndicator.SetActive(value);
        }

        private void Update() => DrawLineBetweenHandAndObject();

        internal void AttachHolder(Transform ultrahandable)
        {
            Holder.position = ultrahandable.position;

            ultrahandable.parent = Holder;

            var localPos = Holder.localPosition;
            localPos.x = 0;
            Holder.localPosition = localPos;
        }

        internal void UnttachHolder(Transform ultrahandable)
        {
            ultrahandable.parent = null;
        }

        private void DrawLineBetweenHandAndObject()
        {
            if (!settings.IsHolding()) return;

            Debug.DrawLine(
                Hand.position,
                Holder.position,
                Color.green
            );
        }
    }
}