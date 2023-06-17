using UnityEngine;

namespace TheLegend.Players
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMotor motor;

        private static readonly int isStrafingId = Animator.StringToHash("IsStrafing");
        private static readonly int isStandingId = Animator.StringToHash("IsStanding");
        private static readonly int absInputMagnitudeId = Animator.StringToHash("AbsInputMagnitude");
        private static readonly int verticalMoveInputId = Animator.StringToHash("VerticalMoveInput");
        private static readonly int horizontalMoveInputId = Animator.StringToHash("HorizontalMoveInput");

        private void Reset()
        {
            animator = GetComponent<Animator>();
            motor = GetComponent<PlayerMotor>();
        }

        private void Update()
        {
            SetMoveInput(motor.MoveInput);
            SetAbsInputMagnitude(motor.AbsInputMagnitude, motor.IsSprinting);
        }

        private void OnEnable() => motor.OnLocomotionChanged += SetLocomotion;
        private void OnDisable() => motor.OnLocomotionChanged -= SetLocomotion;

        public void SetLocomotion(LocomotionType locomotion)
        {
            var isStanding = locomotion == LocomotionType.Stand;
            var isStrafeing = locomotion == LocomotionType.Strafe;

            SetIsStanding(isStanding);
            SetIsStrafing(isStrafeing);
        }

        public void SetIsStrafing(bool enabled) => animator.SetBool(isStrafingId, enabled);
        public void SetIsStanding(bool enabled) => animator.SetBool(isStandingId, enabled);

        public void SetMoveInput(Vector2 input)
        {
            animator.SetFloat(verticalMoveInputId, input.y);
            animator.SetFloat(horizontalMoveInputId, input.x);
        }

        public void SetAbsInputMagnitude(float input, bool isSprinting)
        {
            const float sprintInput = 2F;
            if (isSprinting) input = sprintInput;
            animator.SetFloat(absInputMagnitudeId, input);
        }

        public void PlayUltrahandInteraction() { }
    }
}
