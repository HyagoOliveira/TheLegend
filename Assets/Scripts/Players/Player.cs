using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.Players
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(Ultrahand))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerAnimator))]
    public sealed class Player : MonoBehaviour
    {
        [field: SerializeField] public PlayerSettings Settings { get; private set; }
        [field: SerializeField] public PlayerMotor Motor { get; private set; }
        [field: SerializeField] public PlayerAnimator Animator { get; private set; }
        [field: SerializeField] public Transform CameraPivot { get; private set; }

        [field: Header("Abilities")]
        [field: SerializeField] public Ultrahand Ultrahand { get; private set; }

        private void Reset()
        {
            Motor = GetComponent<PlayerMotor>();
            Animator = GetComponent<PlayerAnimator>();
            CameraPivot = transform.Find(nameof(CameraPivot));
            Ultrahand = GetComponent<Ultrahand>();
        }

        private void Start() => Settings.Initialize(this);

        private void OnEnable()
        {
            Settings.ToggleMovement(true);

            Settings.Ultrahand.OnToggled += HandleUltrahandToggled;
        }

        private void OnDisable()
        {
            Settings.ToggleMovement(false);

            Settings.Ultrahand.OnToggled -= HandleUltrahandToggled;
        }

        private void HandleUltrahandToggled(bool enabled)
        {
            if (enabled) Settings.EnableAbility(AbilityType.Ultrahand);
            else Settings.DisableAbility(AbilityType.Ultrahand);
        }
    }
}