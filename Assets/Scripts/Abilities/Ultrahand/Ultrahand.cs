using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>, IInteractable
    {
        private IUltrahandable lastUltrahandable;

        public bool IsInteracting => lastUltrahandable != null;

        public override void Interact()
        {
            Player.Motor.TurnTowardCameraDirection();
            Player.Animator.PlayUltrahandInteraction();

            var hasUltrahandable = AbilityCaster.TryGetHittingComponent(out IUltrahandable ultrahandable);
            if (!hasUltrahandable) return;

            ultrahandable.Interact();
            settings.StartInteraction(ultrahandable);

            lastUltrahandable = ultrahandable;
        }

        public void CancelInteraction()
        {
            if (IsInteracting)
            {
                lastUltrahandable.CancelInteraction();
                settings.CancelInteraction(lastUltrahandable);
            }

            lastUltrahandable = null;
        }

        public void MoveLaterally(Vector2 input)
        {
            if (!IsInteracting) return;

            var velocity = settings.Speed * Time.deltaTime * input;
            lastUltrahandable.Move(velocity);
        }

        public void MoveLongitudially(Vector2 input)
        {
            if (!IsInteracting) return;

        }
    }
}