using System;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    [CreateAssetMenu(fileName = "UltrahandSettings", menuName = "TheLegend/Abilities/Ultrahand Settings", order = 110)]
    public sealed class UltrahandSettings : AbstractAbilitySettings
    {
        [field: SerializeField] public float Speed { get; private set; } = 10F;
        [SerializeField, Min(0f)] private float maxDistance = 10f;

        public event Action<bool> OnSelectionChanged;
        public event Action<IUltrahandable> OnInteractionStarted;
        public event Action<IUltrahandable> OnInteractionCanceled;

        public bool IsInteracting => CurrentUltrahandable != null;
        public IUltrahandable CurrentUltrahandable { get; private set; }

        internal void TryStartInteraction()
        {
            Player.Motor.TurnTowardCameraDirection();
            Player.Animator.PlayUltrahandInteraction();

            if (IsInteracting) return;

            var hasUltrahandable = PlayerSettings.AbilityCaster.
                TryGetHittingComponent(out IUltrahandable ultrahandable);
            if (!hasUltrahandable) return;

            ultrahandable.Interact();
            OnInteractionStarted?.Invoke(ultrahandable);

            CurrentUltrahandable = ultrahandable;
        }

        internal void CancelInteraction()
        {
            if (!IsInteracting) return;

            Disable();
            CurrentUltrahandable.CancelInteraction();
            OnInteractionCanceled?.Invoke(CurrentUltrahandable);

            CurrentUltrahandable = null;
        }

        public void MoveLaterally(Vector2 input)
        {
            if (!IsInteracting) return;

            var direction = Player.Motor.GetInputDirectionRelativeToCamera(input);
            var velocity = Speed * Time.deltaTime * direction;
            var nextPosition = CurrentUltrahandable.transform.position + velocity;
            var distance = Vector3.Distance(
                Player.transform.position,
                nextPosition
            );
            var hasMaxDistance = distance > maxDistance;
            if (hasMaxDistance) return;

            CurrentUltrahandable.Move(velocity);

        }

        public void MoveLongitudially(Vector2 input)
        {

        }

        internal override void Toggle(bool enabled)
        {
            base.Toggle(enabled);
            if (!enabled) CancelInteraction();
        }

        internal override void HandleHitChanged(RaycastHit hit)
        {
            var hasUltrahandable =
                hit.transform != null &&
                hit.transform.TryGetComponent(out IUltrahandable ultrahandable);

            OnSelectionChanged?.Invoke(hasUltrahandable);
        }

        internal void Update()
        {
            if (!IsInteracting) return;

            Debug.DrawLine(
                Player.UltrahandTransform.position,
                CurrentUltrahandable.transform.position,
                Color.green
            );
        }
    }
}