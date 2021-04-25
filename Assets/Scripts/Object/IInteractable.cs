using UnityEngine;

public interface IInteractable
{
    InteractionType BeginInteraction(GameObject gameObject);
    void EndInteraction();
}

public enum InteractionType
{
    None,
    StopMovement
}