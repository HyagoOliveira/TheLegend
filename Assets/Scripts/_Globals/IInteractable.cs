namespace TheLegend
{
    public interface IInteractable
    {
        bool IsInteracting { get; }

        void Interact();
        void CancelInteraction();
    }
}