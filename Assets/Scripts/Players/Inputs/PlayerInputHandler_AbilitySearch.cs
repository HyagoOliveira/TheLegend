using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace TheLegend.Players
{
    public partial class PlayerInputHandler : MonoBehaviour
    {
        private void BindAbilitySearchActions()
        {
            actions.AbilitySearch.Perform.started += HandlePerformStarted;
            actions.AbilitySearch.Cancel.started += HandleCancelStarted;
        }

        private void UnBindAbilitySearchActions()
        {
            actions.AbilitySearch.Perform.started -= HandlePerformStarted;
            actions.AbilitySearch.Cancel.started -= HandleCancelStarted;
        }

        private void HandlePerformStarted(CallbackContext _) => player.Settings.Ultrahand.TryStartInteraction();
        private void HandleCancelStarted(CallbackContext _) => player.Ultrahand.Disable();
    }
}