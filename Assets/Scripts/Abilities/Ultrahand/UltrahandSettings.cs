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

        public bool IsShowingDirectionalIndicator
        {
            get => Player.Ultrahand.IsShowingDirectionalIndicator;
            private set
            {
                Player.Ultrahand.IsShowingDirectionalIndicator = value;
                OnDirectionalIndicatorToggled?.Invoke(value);
            }
        }

        public event Action OnObjectRotated;
        public event Action<bool> OnSelectionChanged;
        public event Action<bool> OnDirectionalIndicatorToggled;
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

            if (hasUltrahandable) StartInteraction(ultrahandable);
        }

        public void CancelInteraction()
        {
            IsShowingDirectionalIndicator = false;

            if (!IsHolding()) return;

            CurrentUltrahandable.CancelInteraction();
            OnInteractionCanceled?.Invoke(CurrentUltrahandable);

            Player.Ultrahand.UnttachHolder(CurrentUltrahandable.transform);

            CurrentUltrahandable = null;
        }

        public void MoveDistally(float input)
        {
            if (!HasInput(input)) return;

            var speed = distalSpeed * Time.deltaTime;
            var direction = input * Player.transform.forward;
            var isInvalidMovement = !CurrentUltrahandable.CanMove(direction, speed);

            if (isInvalidMovement) return;

            var velocity = speed * direction;
            var position = Player.Ultrahand.Holder.position;
            var nextPosition = position + velocity;
            var distance = GetDistalDistanceFromPlayer(nextPosition);
            var hasMaxDistance = distance > maxDistalDistance;
            var hasMinDistance = distance < minDistalDistance;
            var hasInvalidDistance = hasMaxDistance || hasMinDistance;

            if (hasInvalidDistance) return;

            Player.Ultrahand.Holder.position = nextPosition;
        }

        public void MoveVertically(float input)
        {
            if (!HasInput(input)) return;

            var direction = input * Vector3.up;
            var speed = verticalSpeed * Time.deltaTime;
            var isInvalidMovement = !CurrentUltrahandable.CanMove(direction, speed);

            if (isInvalidMovement) return;

            var velocity = speed * direction;
            var position = Player.Ultrahand.Holder.position;
            var nextPosition = position + velocity;
            var distance = GetVerticalDistanceFromPlayer(nextPosition);
            var hasMaxDistance = distance > maxVerticalDistance;
            var isBellowPlayer = nextPosition.y < Player.transform.position.y;
            var hasInvalidDistance = hasMaxDistance || isBellowPlayer;

            if (hasInvalidDistance) return;

            Player.Ultrahand.Holder.position = nextPosition;
        }

        public void Rotate(Vector2 input)
        {
            var hasInput = Mathf.Abs(input.sqrMagnitude) > 0F;
            if (!hasInput) return;

            UltrahandRotator.Rotate(
                input,
                CurrentUltrahandable.transform,
                rotateAngle,
                rotateTime
            );
            OnObjectRotated?.Invoke();
        }

        public void ResetRotation() =>
            UltrahandRotator.Reset(
                CurrentUltrahandable.transform,
                rotateTime
            );

        public void ShowDirectionalIndicator() => IsShowingDirectionalIndicator = true;
        public void HideDirectionalIndicator() => IsShowingDirectionalIndicator = false;

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

        private void StartInteraction(IUltrahandable ultrahandable)
        {
            CurrentUltrahandable = ultrahandable;

            Player.Ultrahand.AttachHolder(CurrentUltrahandable.transform);

            UltrahandRotator.Rotate(
               CurrentUltrahandable.transform,
               rotateAngle,
               rotateTime
            );

            CurrentUltrahandable.Interact();
            OnInteractionStarted?.Invoke(CurrentUltrahandable);
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

        private static bool HasInput(float input) => Mathf.Abs(input) > 0F;
    }
}