using UnityEngine;

namespace TheLegend.Abilities
{
    [DisallowMultipleComponent]
    public sealed class Ultrahand : AbstractAbility<UltrahandSettings>
    {
        private void Update() => settings.Update();
    }
}