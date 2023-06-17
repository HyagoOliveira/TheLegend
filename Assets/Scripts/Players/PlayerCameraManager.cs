using UnityEngine;
using Cinemachine;

namespace TheLegend.Players
{
    [DisallowMultipleComponent]
    public sealed class PlayerCameraManager : MonoBehaviour
    {
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private CinemachineFreeLook freeLook;

        private void Start() => SetTarget(freeLook, settings.Player);
        private void OnEnable() => settings.Player.Motor.OnLocomotionChanged += HandleLocomotionChanged;
        private void OnDisable() => settings.Player.Motor.OnLocomotionChanged -= HandleLocomotionChanged;

        private void HandleLocomotionChanged(LocomotionType type)
        {
            var isFreeLocomotion = type == LocomotionType.Free;
            freeLook.gameObject.SetActive(isFreeLocomotion);
        }

        private static void SetTarget(CinemachineFreeLook virtualCamera, Player player)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.CameraPivot;
        }

        private static void Replace(CinemachineFreeLook from, CinemachineFreeLook to)
        {
            if (from == to)
            {
                to.gameObject.SetActive(true);
                return;
            }

            to.transform.SetPositionAndRotation(
                from.transform.position,
                from.transform.rotation
            );

            to.gameObject.SetActive(true);
            from.gameObject.SetActive(false);
        }
    }
}