using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mirror;
using UnityEngine;

public class PickableItem : NetworkBehaviour, IInteractable
{
    public int ItemId;

    public InteractionType BeginInteraction(GameObject gameObject)
    {
        Debug.Log("yo fuck you");
        GiveItemToPlayer(gameObject);

        return InteractionType.None;
    }

    [Command(requiresAuthority = false)]
    void GiveItemToPlayer(GameObject player)
    {
        var holder = player.GetComponent<ItemHolder>();

        holder.ItemIndex = ItemId;

        Destroy(gameObject);
    }

    public void EndInteraction()
    {
    }
}

