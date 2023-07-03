using System;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    public abstract class AbstractAbilitySettings : ScriptableObject
    {
        [field: SerializeField] public PlayerSettings PlayerSettings { get; private set; }

        public Player Player => PlayerSettings.Player;

        public event Action<bool> OnToggled;

        internal virtual void Initialize() { }
        internal virtual void Toggle(bool enabled) => OnToggled?.Invoke(enabled);

        internal abstract void HandleHitChanged(RaycastHit hit);

        protected void Disable() => OnToggled?.Invoke(false);
    }
}