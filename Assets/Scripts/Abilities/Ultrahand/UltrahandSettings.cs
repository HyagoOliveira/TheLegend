using System;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    [CreateAssetMenu(fileName = "UltrahandSettings", menuName = "TheLegend/Abilities/Ultrahand Settings", order = 110)]
    public sealed class UltrahandSettings : AbstractAbilitySettings
    {
        [SerializeField, Min(0f)] private float moveSpeed = 10F;
        [SerializeField, Min(0f)] private float rotateSpeed = 60f;
        [SerializeField, Min(0f)] private float maxDistance = 10f;

        public event Action<bool> OnSelectionChanged;
        public event Action<IUltrahandable> OnInteractionStarted;
        public event Action<IUltrahandable> OnInteractionCanceled;

        public bool IsInteracting => CurrentUltrahandable != null;
        public float CurrentDistance { get; private set; }
        public IUltrahandable CurrentUltrahandable { get; private set; }

        internal void Update()
        {
            if (!IsInteracting) return;

            DrawLineBetweenPlayerAndObject();
            UpdateObjectAccordinglyToPlayer();
        }

        public void MoveLaterally(Vector2 input)
        {
            if (!IsInteracting) return;

            var direction = Player.Motor.GetInputDirectionRelativeToCamera(input);
            var velocity = moveSpeed * Time.deltaTime * direction;
            var nextPosition = CurrentUltrahandable.transform.position + velocity;
            var distance = Vector3.Distance(
                Player.transform.position,
                nextPosition
            );
            var hasMaxDistance = distance > maxDistance;
            if (hasMaxDistance) velocity = Vector3.zero;

            CurrentUltrahandable.Move(velocity);
        }

        public void MoveAround(Vector2 input)
        {
            if (!IsInteracting) return;

            var angle = rotateSpeed * Time.deltaTime;
            var direction = new Vector3(-input.y, input.x);
            var rotation = CurrentUltrahandable.transform.rotation;

            CurrentUltrahandable.transform.RotateAround(
                Player.transform.position,
                direction,
                angle
            );
            CurrentUltrahandable.transform.rotation = rotation;
        }

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
            CurrentDistance = Vector3.Distance(
                Player.transform.position,
                CurrentUltrahandable.transform.position
            );
        }

        internal void CancelInteraction()
        {
            if (!IsInteracting) return;

            Disable();
            CurrentUltrahandable.CancelInteraction();
            OnInteractionCanceled?.Invoke(CurrentUltrahandable);

            CurrentDistance = 0f;
            CurrentUltrahandable = null;
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

        private void UpdateObjectAccordinglyToPlayer()
        {
            var direction = (CurrentUltrahandable.transform.position - Player.transform.position).normalized;
            CurrentUltrahandable.transform.position = Player.transform.position + direction * CurrentDistance;
        }

        private void DrawLineBetweenPlayerAndObject()
        {
            Debug.DrawLine(
                Player.UltrahandTransform.position,
                CurrentUltrahandable.transform.position,
                Color.green
            );
        }
    }
}