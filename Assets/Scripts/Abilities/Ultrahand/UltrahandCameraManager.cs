using Cinemachine;
using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class UltrahandCameraManager : MonoBehaviour
    {
        [SerializeField] private UltrahandSettings settings;
        [SerializeField] private CinemachineFreeLook freeLook;

        private void OnEnable()
        {
            settings.OnToggled += HandleToggled;
            settings.OnInteractionCanceled += HandleInteractionCanceled;
        }

        private void OnDisable()
        {
            settings.OnToggled -= HandleToggled;
            settings.OnInteractionCanceled -= HandleInteractionCanceled;
        }

        private void HandleToggled(bool enabled) => freeLook.gameObject.SetActive(enabled);

        private void HandleInteractionCanceled(IUltrahandable _) => HandleToggled(false);
    }
}