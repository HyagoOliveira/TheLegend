using System;
using TheLegend.Abilities;
using UnityEngine;

namespace TheLegend.Players
{
    /// <summary>
    /// Controls a Character physics using a Kinematic <see cref="Rigidbody"/> and a <see cref="CapsuleCollider"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private Rigidbody body;
        [SerializeField] private CapsuleCollider collider;
        [SerializeField] private LocomotionType initialLocomotion;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float walkSpeed = 2f;
        [SerializeField, Min(0f)] private float runSpeed = 6f;
        [SerializeField, Min(0f)] private float sprintSpeed = 8f;
        [SerializeField, Min(0f)] private float moveAcceleration = 12f;

        [Header("Inputs")]
        [SerializeField, Range(0f, 1F)] private float walkInputThreshold = 0.25F;

        public event Action<LocomotionType> OnLocomotionChanged;

        public bool IsSprinting { get; private set; }
        public bool IsMoveInputting { get; private set; }

        public float Speed { get; private set; }
        public float AbsInputMagnitude { get; private set; }

        public Vector2 MoveInput { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 VelocityPerSecond { get; private set; }

        public LocomotionType Locomotion
        {
            get => locomotion;
            private set
            {
                locomotion = value;
                OnLocomotionChanged?.Invoke(locomotion);
            }
        }

        private Vector3 moveDirection;
        private Transform currentCamera;
        private LocomotionType locomotion;

        private void Reset()
        {
            body = GetComponent<Rigidbody>();
            collider = GetComponent<CapsuleCollider>();

            SetupRigidbody();
        }

        private void Awake() => currentCamera = Camera.main.transform;
        private void Start() => Locomotion = initialLocomotion;

        private void FixedUpdate()
        {
            UpdateMovement();
            UpdateRotation();
        }

        private void OnEnable()
        {
            settings.OnAbilityEnabled += HandleAbilityEnabled;
            settings.OnAbilityDisabled += HandleAbilityDisabled;
        }

        private void OnDisable()
        {
            settings.OnAbilityEnabled -= HandleAbilityEnabled;
            settings.OnAbilityDisabled -= HandleAbilityDisabled;
        }

        public bool IsMoving() => Mathf.Abs(Velocity.sqrMagnitude) > 0F;

        public bool CanSprint() => Locomotion == LocomotionType.Free;

        public void Move(Vector2 input)
        {
            MoveInput = input;
            AbsInputMagnitude = Mathf.Clamp01(Mathf.Abs(MoveInput.sqrMagnitude));
            IsMoveInputting = AbsInputMagnitude > 0F;
        }

        public void Stop() => Move(Vector2.zero);

        public void StartSprint() => IsSprinting = CanSprint();
        public void CancelSprint() => IsSprinting = false;

        public void StartFreeLocomotion() => Locomotion = LocomotionType.Free;
        public void StartStandLocomotion() => Locomotion = LocomotionType.Stand;
        public void StartStrafeLocomotion() => Locomotion = LocomotionType.Strafe;

        public void TurnTowardCameraDirection()
        {
            var lookDirection = body.position + GetCameraForwardDirection();
            transform.LookAt(lookDirection);
        }

        public Vector3 GetInputDirectionRelativeToCamera(Vector2 input)
        {
            var right = currentCamera.right;
            right.y = 0f;
            var forward = Vector3.Cross(right, Vector3.up);
            return (right * input.x + forward * input.y).normalized;
        }

        private void UpdateMovement()
        {
            UpdateCurrentSpeed();
            UpdateMoveDirection();

            var isMovingIntoCollision = IsMoveInputting && IsForwardCollision();

            Velocity = isMovingIntoCollision ? Vector3.zero : Speed * moveDirection;
            VelocityPerSecond = Velocity * Time.deltaTime;

            var position = body.position + VelocityPerSecond;
            transform.position = position;
        }

        private void UpdateRotation()
        {
            var lookDirection = body.position + GetLookDirection();
            transform.LookAt(lookDirection);
        }

        private void UpdateCurrentSpeed()
        {
            if (!IsMoveInputting)
            {
                Speed = 0f;
                return;
            }

            var targetSpeed = IsSprinting ? sprintSpeed : GetNormalSpeed();
            var moveSpeed = moveAcceleration * Time.deltaTime;

            Speed = Mathf.MoveTowards(Speed, targetSpeed, moveSpeed);
            Speed = RoundIntoThreeDecimalPlaces(Speed);
        }

        private void UpdateMoveDirection()
        {
            moveDirection = IsMoveInputting ?
                GetMoveInputDirectionRelativeToCamera() :
                Vector3.zero;
        }

        private void SetupRigidbody()
        {
            body.isKinematic = true;
            body.constraints =
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }

        private bool IsForwardCollision()
        {
            var origin = body.position + Vector3.up;
            return Physics.Raycast(origin, transform.forward, collider.radius);
        }

        private bool IsWalkingInput() => AbsInputMagnitude < walkInputThreshold;

        private Vector3 GetMoveInputDirectionRelativeToCamera() =>
            GetInputDirectionRelativeToCamera(MoveInput);

        private Vector3 GetLookDirection() => Locomotion switch
        {
            LocomotionType.Free => moveDirection,
            LocomotionType.Stand => moveDirection,
            LocomotionType.Strafe => GetCameraForwardDirection(),
            _ => moveDirection
        };

        private Vector3 GetCameraForwardDirection()
        {
            var direction = currentCamera.forward;
            direction.y = 0f;
            return direction;
        }

        private float GetNormalSpeed() => IsWalkingInput() ? walkSpeed : runSpeed;

        private void HandleAbilityEnabled(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Ultrahand:
                    StartStandLocomotion();
                    break;
            }
        }

        private void HandleAbilityDisabled(AbilityType _) => StartFreeLocomotion();

        private static float RoundIntoThreeDecimalPlaces(float value) =>
            Mathf.Round(value * 1000f) / 1000f;
    }
}