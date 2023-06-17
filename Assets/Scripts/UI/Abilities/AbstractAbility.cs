using UnityEngine;
using TheLegend.Abilities;

namespace TheLegend.UI
{
    public abstract class AbstractAbility<T> : MonoBehaviour where T : AbstractAbilitySettings
    {
        [SerializeField] protected T settings;

        protected virtual void OnEnable() => settings.OnToggled += HandleToggled;
        protected virtual void OnDisable() => settings.OnToggled -= HandleToggled;

        protected abstract void HandleToggled(bool enabled);
    }
}