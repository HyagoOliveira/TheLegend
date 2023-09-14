using System;
using TheLegend.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;
using static TheLegend.Players.PlayerInputActions;

namespace TheLegend.Players
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(Player))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Player player;

        private UltrahandSettings UltrahandSettings => player.Settings.Ultrahand;

        private event Action OnUpdate;

        private NavigationActions navigation;
        private UltrahandActions ultrahand;

        private void Reset() => player = GetComponent<Player>();

        private void Awake()
        {
            var input = new PlayerInputActions();

            ultrahand = input.Ultrahand;
            navigation = input.Navigation;
        }

        private void Update() => OnUpdate?.Invoke();

        private void OnEnable()
        {
            ultrahand.Enable();
            navigation.Enable();

            player.Settings.OnMovementToggled += HandleMovementToggled;
            player.Settings.OnAbilityEnabled += HandleAbilityEnabled;
            player.Settings.OnAbilityDisabled += HandleAbilityDisabled;
            player.Settings.Ultrahand.OnInteractionStarted += HandleUltrahandInteractionStarted;
            player.Settings.Ultrahand.OnInteractionCanceled += HandleUltrahandInteractionCanceled;
        }

        private void OnDisable()
        {
            navigation.Disable();
            ultrahand.Disable();

            player.Settings.OnMovementToggled -= HandleMovementToggled;
            player.Settings.OnAbilityEnabled -= HandleAbilityEnabled;
            player.Settings.OnAbilityDisabled -= HandleAbilityDisabled;
            player.Settings.Ultrahand.OnInteractionStarted -= HandleUltrahandInteractionStarted;
            player.Settings.Ultrahand.OnInteractionCanceled -= HandleUltrahandInteractionCanceled;
        }

        private void UpdateNavigationActions()
        {
            var moveAxis = navigation.Move.ReadValue<Vector2>();
            var ultrahandButton = navigation.Ultrahand.WasPressedThisFrame();

            player.Motor.Move(moveAxis);
            if (ultrahandButton) player.Ultrahand.Toggle();

            UpdateButtonActions(
                navigation.Sprint,
                player.Motor.StartSprint,
                player.Motor.CancelSprint
            );
            UpdateButtonActions(
                navigation.Strafe,
                player.Motor.StartStrafeLocomotion,
                player.Motor.StartFreeLocomotion
            );
        }

        private void UpdateUltrahandSearchingActions()
        {
            var interactButton = ultrahand.Interact.WasPressedThisFrame();
            var cancelButton = ultrahand.Cancel.WasPressedThisFrame();

            if (interactButton) UltrahandSettings.TryStartInteraction();
            else if (cancelButton) UltrahandSettings.CancelInteraction();
        }

        private void UpdateUltrahandActions()
        {
            var isHoldingRotateButton = ultrahand.Rotate.IsPressed();
            var cancelButton = ultrahand.Cancel.WasPressedThisFrame();
            var moveVertically = ultrahand.MoveVertically.ReadValue<float>();
            var resetRotationButton = ultrahand.ResetRotation.WasPressedThisFrame();

            if (cancelButton) UltrahandSettings.CancelInteraction();
            else if (resetRotationButton) UltrahandSettings.ResetRotation();

            UltrahandSettings.EnableRotation(isHoldingRotateButton);
            UltrahandSettings.MoveVertically(moveVertically);

            if (isHoldingRotateButton)
            {
                if (ultrahand.RotateAxis.WasPerformedThisFrame())
                {
                    var rotation = ultrahand.RotateAxis.ReadValue<Vector2>();
                    UltrahandSettings.Rotate(rotation);
                }
            }
            else
            {
                var moveDistally = ultrahand.MoveDistally.ReadValue<float>();
                UltrahandSettings.MoveDistally(moveDistally);
            }
        }

        private void HandleMovementToggled(bool enabled)
        {
            if (enabled) OnUpdate += UpdateNavigationActions;
            else OnUpdate -= UpdateNavigationActions;
        }

        private void HandleAbilityEnabled(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Ultrahand:
                    OnUpdate += UpdateUltrahandSearchingActions;
                    break;
            }
        }

        private void HandleAbilityDisabled(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Ultrahand:
                    OnUpdate -= UpdateUltrahandSearchingActions;
                    break;
            }
        }

        private void HandleUltrahandInteractionStarted(IUltrahandable _) =>
            OnUpdate += UpdateUltrahandActions;

        private void HandleUltrahandInteractionCanceled(IUltrahandable _) =>
            OnUpdate -= UpdateUltrahandActions;

        private static void UpdateButtonActions(
            InputAction button,
            Action pressedCallback,
            Action releasedCallback
        )
        {
            if (button.WasPressedThisFrame()) pressedCallback();
            else if (button.WasReleasedThisFrame()) releasedCallback();
        }
    }
}
