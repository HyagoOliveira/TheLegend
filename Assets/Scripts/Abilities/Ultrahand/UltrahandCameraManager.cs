using Cinemachine;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class UltrahandCameraManager : MonoBehaviour
    {
        [SerializeField] private PlayerSettings playerSettings;
        [SerializeField] private PlayerCameraManager playerCameraManager;
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private CinemachineFreeLook interactionLook;

        private void Start()
        {
            freeLook.SetTarget(playerSettings.Player);
            interactionLook.SetTarget(playerSettings.Player);
        }

        private void OnEnable()
        {
            playerSettings.OnAbilityEnabled += HandleAbilityEnabled;
            playerSettings.Ultrahand.OnInteractionStarted += HandleInteractionStarted;
        }

        private void OnDisable()
        {
            playerSettings.OnAbilityEnabled -= HandleAbilityEnabled;
            playerSettings.Ultrahand.OnInteractionStarted -= HandleInteractionStarted;
        }

        private void HandleAbilityEnabled(AbilityType type)
        {
            var isUltrahand = type == AbilityType.Ultrahand;
            if (!isUltrahand) return;

            playerCameraManager.CurrentCamera = freeLook;
        }

        private void HandleInteractionStarted(IUltrahandable _)
        {
            playerCameraManager.CurrentCamera = interactionLook;
        }
    }
}