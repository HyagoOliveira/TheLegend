using System;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    [CreateAssetMenu(fileName = "UltrahandSettings", menuName = "TheLegend/Abilities/Ultrahand Settings", order = 110)]
    public sealed class UltrahandSettings : AbstractAbilitySettings
    {
        [SerializeField, Min(0f)] private float distalSpeed = 6F;
        [SerializeField, Min(0f)] private float verticalSpeed = 4F;
        [SerializeField, Min(0f)] private float rotateTime = 0.2f;
        [SerializeField, Min(0f)] private float rotateAngle = 45f;
        [SerializeField, Min(0f)] private float minDistalDistance = 2f;
        [SerializeField, Min(0f)] private float maxDistalDistance = 10f;
        [SerializeField, Min(0f)] private float maxVerticalDistance = 3f;

        public event Action OnObjectRotated;
        public event Action<bool> OnSelectionChanged;
        public event Action<IUltrahandable> OnInteractionStarted;
        public event Action<IUltrahandable> OnInteractionCanceled;

        public IUltrahandable CurrentUltrahandable { get; private set; }

        public bool IsHolding() => CurrentUltrahandable != null;

        public void TryStartInteraction()
        {
            Player.Motor.TurnTowardCameraDirection();
            Player.Animator.PlayUltrahandInteraction();

            if (IsHolding()) return;

            var hasUltrahandable = PlayerSettings.AbilityCaster.
                TryGetHittingComponent(out IUltrahandable ultrahandable);
            if (!hasUltrahandable) return;

            CurrentUltrahandable = ultrahandable;

            Player.Ultrahand.AttachHolder(CurrentUltrahandable.transform);

            CurrentUltrahandable.Interact();
            OnInteractionStarted?.Invoke(CurrentUltrahandable);
        }

        public void CancelInteraction()
        {
            Disable();

            if (!IsHolding()) return;

            CurrentUltrahandable.CancelInteraction();
            OnInteractionCanceled?.Invoke(CurrentUltrahandable);

            Player.Ultrahand.UnttachHolder(CurrentUltrahandable.transform);

            CurrentUltrahandable = null;
        }

        public void MoveDistally(float input)
        {
            var speed = input * distalSpeed * Time.deltaTime;
            var position = Player.Ultrahand.Holder.position;
            var nextPosition = position + Player.transform.forward * speed;
            var distance = GetDistalDistanceFromPlayer(nextPosition);
            var hasMaxDistance = distance > maxDistalDistance;
            var hasMinDistance = distance < minDistalDistance;
            var hasInvalidDistance = hasMaxDistance || hasMinDistance;

            if (hasInvalidDistance) return;

            Player.Ultrahand.Holder.position = nextPosition;
        }

        public void MoveVertically(float input)
        {
            var speed = input * verticalSpeed * Time.deltaTime;
            var position = Player.Ultrahand.Holder.position;
            var nextPosition = position + Vector3.up * speed;
            var distance = GetVerticalDistanceFromPlayer(nextPosition);
            var hasMaxDistance = distance > maxVerticalDistance;
            var isBellowPlayer = nextPosition.y < Player.transform.position.y;
            var hasInvalidDistance = hasMaxDistance || isBellowPlayer;

            if (hasInvalidDistance) return;

            Player.Ultrahand.Holder.position = nextPosition;
        }

        public void EnableRotation(bool enabled) =>
            Player.Ultrahand.EnableDirectionalIndicator(enabled);

        public void Rotate(Vector2 input)
        {
            var hasInput = Mathf.Abs(input.sqrMagnitude) > 0F;
            if (!hasInput) return;

            Player.Ultrahand.Rotate(
                input,
                CurrentUltrahandable.transform,
                rotateAngle,
                rotateTime
            );
            OnObjectRotated?.Invoke();
        }

        public void ResetRotation() =>
            Player.Ultrahand.ResetRotation(CurrentUltrahandable.transform, rotateTime);

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

        private float GetDistalDistanceFromPlayer(Vector3 position)
        {
            var playerPos = Player.transform.position;

            playerPos.y = 0f;
            position.y = 0f;

            return Vector3.Distance(playerPos, position);
        }

        private float GetVerticalDistanceFromPlayer(Vector3 position)
        {
            var verticalDelta = position.y - Player.transform.position.y;
            return Mathf.Abs(verticalDelta);
        }
    }
}