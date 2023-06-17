using System;
using UnityEngine;
using TheLegend.Players;

namespace TheLegend.Abilities
{
    public abstract class AbstractAbilitySettings : ScriptableObject
    {
        [field: SerializeField] public PlayerSettings PlayerSettings { get; private set; }

        public event Action<bool> OnToggled;

        internal void Initialize() { }
        internal void Toggle(bool enabled) => OnToggled?.Invoke(enabled);

        internal abstract void HandleHitChanged(RaycastHit hit);
    }
}