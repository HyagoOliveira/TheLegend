using UnityEngine;
using TheLegend.Abilities;
using static UnityEngine.InputSystem.InputAction;

namespace TheLegend.Players
{
    public partial class PlayerInputHandler : MonoBehaviour
    {
        private void BindUltrahandActions()
        {
            actions.Ultrahand.Cancel.started += HandleCancelStarted;
            actions.Ultrahand.Rotate.started += HandleRotateStarted;
            actions.Ultrahand.Rotate.canceled += HandleRotateCanceled;
            //actions.Ultrahand.Unstick.performed += HandleUnstickPerformed;
            actions.Ultrahand.RotateAxis.started += HandleRotateAxisStarted;
            actions.Ultrahand.ResetRotation.started += HandleResetRotationStarted;
        }

        private void UnBindUltrahandActions()
        {
            actions.Ultrahand.Cancel.started -= HandleCancelStarted;
            actions.Ultrahand.Rotate.started -= HandleRotateStarted;
            actions.Ultrahand.Rotate.canceled -= HandleRotateCanceled;
            //actions.Ultrahand.Unstick.performed -= HandleUnstickPerformed;
            actions.Ultrahand.RotateAxis.started -= HandleRotateAxisStarted;
            actions.Ultrahand.ResetRotation.started -= HandleResetRotationStarted;
        }

        private void HandleRotateStarted(CallbackContext _) => UltrahandSettings.ShowDirectionalIndicator();
        private void HandleRotateCanceled(CallbackContext _) => UltrahandSettings.HideDirectionalIndicator();

        //private void HandleUnstickPerformed(CallbackContext context) { }

        private void HandleRotateAxisStarted(CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            UltrahandSettings.Rotate(input);
        }

        private void HandleResetRotationStarted(CallbackContext _) => UltrahandSettings.ResetRotation();

        private void HandleUltrahandToggled(bool enabled)
        {
            if (enabled) actions.AbilitySearch.Enable();
            else actions.AbilitySearch.Disable();
        }

        private void HandleUltrahandInteractionStarted(IUltrahandable _)
        {
            actions.AbilitySearch.Disable();
            actions.Ultrahand.Enable();
            actions.Ultrahand.RotateAxis.Disable();
            actions.Ultrahand.MoveDistally.Enable();

            OnUpdate += UpdateMoveDistallyInput;
            OnUpdate += UpdateMoveVerticallyInput;
        }

        private void HandleUltrahandInteractionCanceled(IUltrahandable _)
        {
            actions.AbilitySearch.Disable();
            actions.Ultrahand.Disable();

            OnUpdate -= UpdateMoveDistallyInput;
            OnUpdate -= UpdateMoveVerticallyInput;
        }

        private void HandleUltrahandDirectionalIndicatorToggled(bool enabled)
        {
            if (enabled)
            {
                actions.Ultrahand.RotateAxis.Enable();
                actions.Ultrahand.MoveDistally.Disable();
            }
            else
            {
                actions.Ultrahand.RotateAxis.Disable();
                actions.Ultrahand.MoveDistally.Enable();
            }
        }

        private void UpdateMoveDistallyInput()
        {
            var input = actions.Ultrahand.MoveDistally.ReadValue<float>();
            UltrahandSettings.MoveDistally(input);
        }

        private void UpdateMoveVerticallyInput()
        {
            var input = actions.Ultrahand.MoveVertically.ReadValue<float>();
            UltrahandSettings.MoveVertically(input);
        }
    }
}