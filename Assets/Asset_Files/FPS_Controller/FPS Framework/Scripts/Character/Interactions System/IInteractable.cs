using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Interface for objects that can be interacted with in the FPS framework.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Gets the transform of the interactable object.
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// Handles the interaction logic when this object is interacted with.
        /// </summary>
        /// <param name="source">The interaction manager initiating the interaction.</param>
        void Interact(InteractionsManager source);

        /// <summary>
        /// Gets the name or description of the interaction.
        /// </summary>
        /// <returns>A string representing the interaction name or description.</returns>
        string GetInteractionName();

        /// <summary>포맷 플레이스홀더에 넘길 인자 배열 (없으면 null 또는 빈 배열)</summary>
        object[] GetInteractionArguments();
    }
}
