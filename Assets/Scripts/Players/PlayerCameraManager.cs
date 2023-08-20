using UnityEngine;
using Cinemachine;
using TheLegend.Abilities;

namespace TheLegend.Players
{
    [DisallowMultipleComponent]
    public sealed class PlayerCameraManager : MonoBehaviour
    {
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private CinemachineFreeLook freeLook;

        public CinemachineVirtualCameraBase CurrentCamera
        {
            get => currentCamera;
            internal set
            {
                currentCamera.Replace(value);
                currentCamera = value;
            }
        }

        private CinemachineVirtualCameraBase currentCamera;

        private void Awake() => currentCamera = freeLook;
        private void Start() => freeLook.SetTarget(settings.Player);

        private void OnEnable()
        {
            settings.OnInitialized += HandleInitialized;
            settings.OnAbilityDisabled += HandleAbilityDisabled;
        }

        private void OnDisable()
        {
            settings.OnInitialized -= HandleInitialized;
            settings.OnAbilityDisabled -= HandleAbilityDisabled;
        }

        private void HandleInitialized() => StartFreeLook();
        private void HandleAbilityDisabled(AbilityType _) => StartFreeLook();

        private void StartFreeLook() => CurrentCamera = freeLook;
    }
}