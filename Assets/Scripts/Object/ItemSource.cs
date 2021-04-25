using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mirror;
using UnityEngine;

public class ItemSource : NetworkBehaviour, IInteractable
{
    [SerializeField] private InteractionType m_InteractionType = default;

    [SerializeField] private EquipableItemList m_ItemList = default;
    [SerializeField] private GameObject m_Item = default;

    public InteractionType BeginInteraction(GameObject gameObject)
    {
        GiveItemToPlayer(gameObject);

        return m_InteractionType;
    }

    [Command(requiresAuthority = false)]
    void GiveItemToPlayer(GameObject player)
    {
        var holder = player.GetComponent<ItemHolder>();

        holder.ItemIndex = m_ItemList.FindIndex(m_Item);
    }

    public void EndInteraction()
    {
    }
}
