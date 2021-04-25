using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class ItemHolder : NetworkBehaviour
{
    [SerializeField] private Transform m_ItemPivot = default;

    [SerializeField] private EquipableItemList m_ItemList = default;

    [SyncVar(hook = nameof(OnItemChanged))]
    public int ItemIndex = -1;

    private void OnItemChanged(int oldIndex, int newIndex)
    {
        foreach (Transform child in m_ItemPivot.transform) {
            Destroy(child.gameObject);
        }

        if (newIndex > m_ItemList.Prefabs.Count)
            return;

        Instantiate(m_ItemList.Prefabs[newIndex], m_ItemPivot, false);
    }
}
