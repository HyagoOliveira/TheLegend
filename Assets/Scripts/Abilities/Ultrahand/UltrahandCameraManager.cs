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
        [SerializeField] private CinemachineVirtualCamera interactionVC;
        [SerializeField] private CinemachineTargetGroup group;

        private Transform interactedTransform;

        private void Reset()
        {
            freeLook = GetComponentInChildren<CinemachineFreeLook>();
            interactionVC = GetComponentInChildren<CinemachineVirtualCamera>();
            group = GetComponentInChildren<CinemachineTargetGroup>();
        }

        private void Start()
        {
            freeLook.SetTarget(playerSettings.Player);
            group.AddMember(playerSettings.Player.CameraPivot, 1F, 2F);
        }

        private void OnEnable()
        {
            playerSettings.OnAbilityEnabled += HandleAbilityEnabled;
            playerSettings.Ultrahand.OnInteractionStarted += HandleInteractionStarted;
            playerSettings.Ultrahand.OnInteractionCanceled += HandleInteractionCanceled;
        }

        private void OnDisable()
        {
            playerSettings.OnAbilityEnabled -= HandleAbilityEnabled;
            playerSettings.Ultrahand.OnInteractionStarted -= HandleInteractionStarted;
            playerSettings.Ultrahand.OnInteractionCanceled -= HandleInteractionCanceled;
        }

        private void HandleAbilityEnabled(AbilityType type)
        {
            var isUltrahand = type == AbilityType.Ultrahand;
            if (!isUltrahand) return;

            playerCameraManager.CurrentCamera = freeLook;
        }

        private void HandleInteractionStarted(IUltrahandable ultrahandable)
        {
            playerCameraManager.CurrentCamera = interactionVC;
            group.AddMember(ultrahandable.transform, 1F, 0F);
            interactedTransform = ultrahandable.transform;
        }

        private void HandleInteractionCanceled(IUltrahandable _)
        {
            group.RemoveMember(interactedTransform);
            interactedTransform = null;
        }
    }
}