using System;
using TheLegend.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;
using static TheLegend.Players.PlayerInputActions;

namespace TheLegend.Players
{
    /// <summary>
    /// Component responsible to receive the inputs and forward them into <see cref="PlayerMotor_Character"/>.
    /// </summary>
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
        }

        private void OnDisable()
        {
            navigation.Disable();
            ultrahand.Disable();

            player.Settings.OnMovementToggled -= HandleMovementToggled;
            player.Settings.OnAbilityEnabled -= HandleAbilityEnabled;
            player.Settings.OnAbilityDisabled -= HandleAbilityDisabled;
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
            /*UpdateButtonActions(
                navigation.Strafe,
                player.Motor.StartStrafeLocomotion,
                player.Motor.StartFreeLocomotion
            );*/
        }

        private void UpdateUltrahandActions()
        {
            var aroundAxis = ultrahand.MoveAround.ReadValue<Vector2>();
            var laterallyAxis = ultrahand.MoveLaterally.ReadValue<Vector2>();
            var interactButton = ultrahand.Interact.WasPressedThisFrame();
            var cancelButton = ultrahand.Cancel.WasPressedThisFrame();

            //UltrahandSettings.MoveAround(aroundAxis);
            //UltrahandSettings.MoveLaterally(laterallyAxis);

            if (interactButton) UltrahandSettings.TryStartInteraction();
            else if (cancelButton) UltrahandSettings.CancelInteraction();
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
                    OnUpdate += UpdateUltrahandActions;
                    break;
            }
        }

        private void HandleAbilityDisabled(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Ultrahand:
                    OnUpdate -= UpdateUltrahandActions;
                    break;
            }
        }

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
