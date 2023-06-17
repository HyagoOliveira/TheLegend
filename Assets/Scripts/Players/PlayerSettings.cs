using System;
using UnityEngine;
using ActionCode.Physics;
using TheLegend.Abilities;

namespace TheLegend.Players
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "TheLegend/Player Settings", order = 110)]
    public sealed class PlayerSettings : ScriptableObject
    {
        [field: SerializeField] public UltrahandSettings Ultrahand { get; private set; }

        public event Action OnInitialized;
        public event Action<bool> OnMovementToggled;
        public event Action<AbilityType> OnAbilityEnabled;
        public event Action<AbilityType> OnAbilityDisabled;

        public Player Player { get; private set; }
        public AbstractCaster AbilityCaster { get; internal set; }

        internal void Initialize(Player player)
        {
            Player = player;
            OnInitialized?.Invoke();
        }

        internal void ToggleMovement(bool enabled) => OnMovementToggled?.Invoke(enabled);
        internal void EnableAbility(AbilityType type) => OnAbilityEnabled?.Invoke(type);
        internal void DisableAbility(AbilityType type) => OnAbilityDisabled?.Invoke(type);
    }
}