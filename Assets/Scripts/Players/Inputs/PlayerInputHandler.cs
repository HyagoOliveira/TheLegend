using System;
using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.Players
{
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(Player))]
    public partial class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Player player;

        private UltrahandSettings UltrahandSettings => player.Settings.Ultrahand;

        private PlayerInputActions actions;
        private event Action OnUpdate;

        private void Reset() => player = GetComponent<Player>();
        private void Awake() => actions = new PlayerInputActions();
        private void Update() => OnUpdate?.Invoke();

        private void OnEnable()
        {
            BindNavigationActions();
            BindAbilitySearchActions();
            BindUltrahandActions();

            player.Settings.OnMovementToggled += HandleMovementToggled;
            player.Settings.Ultrahand.OnToggled += HandleUltrahandToggled;
            player.Settings.Ultrahand.OnInteractionStarted += HandleUltrahandInteractionStarted;
            player.Settings.Ultrahand.OnInteractionCanceled += HandleUltrahandInteractionCanceled;
            player.Settings.Ultrahand.OnDirectionalIndicatorToggled += HandleUltrahandDirectionalIndicatorToggled;
        }

        private void OnDisable()
        {
            UnBindNavigationActions();
            UnBindAbilitySearchActions();
            UnBindUltrahandActions();

            player.Settings.OnMovementToggled -= HandleMovementToggled;
            player.Settings.Ultrahand.OnToggled -= HandleUltrahandToggled;
            player.Settings.Ultrahand.OnInteractionStarted -= HandleUltrahandInteractionStarted;
            player.Settings.Ultrahand.OnInteractionCanceled -= HandleUltrahandInteractionCanceled;
            player.Settings.Ultrahand.OnDirectionalIndicatorToggled -= HandleUltrahandDirectionalIndicatorToggled;
        }
    }
}
