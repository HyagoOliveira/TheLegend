using UnityEngine;
using TheLegend.Players;
using ActionCode.Physics;

namespace TheLegend.Abilities
{
    public abstract class AbstractAbility<T> : MonoBehaviour where T : AbstractAbilitySettings
    {
        [SerializeField] protected T settings;

        public bool IsEnabled
        {
            get => enabled;
            private set
            {
                enabled = value;
                settings.Toggle(enabled);
            }
        }

        public Player Player => settings.PlayerSettings.Player;
        public AbstractCaster AbilityCaster => Player.Settings.AbilityCaster;

        protected virtual void Reset() => enabled = false;
        protected virtual void Awake() => settings.Initialize();

        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;
    }
}