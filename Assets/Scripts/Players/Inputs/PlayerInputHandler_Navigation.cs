using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace TheLegend.Players
{
    public partial class PlayerInputHandler : MonoBehaviour
    {
        private void BindNavigationActions()
        {
            actions.Navigation.Sprint.started += HandleSpritStarted;
            actions.Navigation.Sprint.canceled += HandleSpritCanceled;
            actions.Navigation.Ultrahand.started += HandleUltrahandStarted;
        }

        private void UnBindNavigationActions()
        {
            actions.Navigation.Sprint.started -= HandleSpritStarted;
            actions.Navigation.Sprint.canceled -= HandleSpritCanceled;
            actions.Navigation.Ultrahand.started -= HandleUltrahandStarted;
        }

        private void HandleSpritStarted(CallbackContext _) => player.Motor.StartSprint();
        private void HandleSpritCanceled(CallbackContext _) => player.Motor.CancelSprint();
        private void HandleUltrahandStarted(CallbackContext _) => player.Ultrahand.Toggle();

        private void HandleMovementToggled(bool enabled)
        {
            if (enabled)
            {
                actions.Navigation.Enable();
                OnUpdate += UpdateMoveInput;
            }
            else
            {
                actions.Navigation.Disable();
                OnUpdate -= UpdateMoveInput;
            }
        }

        private void UpdateMoveInput()
        {
            var input = actions.Navigation.Move.ReadValue<Vector2>();
            player.Motor.Move(input);
        }
    }
}