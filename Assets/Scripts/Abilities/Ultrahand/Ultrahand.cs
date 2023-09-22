using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        [field: SerializeField] public Transform Hand { get; private set; }
        [field: SerializeField] public Transform Holder { get; private set; }

        [SerializeField] private GameObject directionalIndicator;

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

        internal void EnableDirectionalIndicator(bool enable) => directionalIndicator.SetActive(enable);

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