using UnityEngine;

namespace TheLegend.Abilities
{
    /// <summary>
    /// Interface used on objects able to be used by Ultrahand ability.
    /// </summary>
    public interface IUltrahandable : IInteractable
    {
        void Move(Vector3 velocity);
    }
}