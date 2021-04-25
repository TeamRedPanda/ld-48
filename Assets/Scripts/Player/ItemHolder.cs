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

    [Command]
    public void DropItem()
    {
        if (ItemIndex == -1 || ItemIndex > m_ItemList.Prefabs.Count)
            return;

        Vector3 position = m_ItemPivot.transform.position;
        Quaternion rotation = m_ItemPivot.transform.rotation;
        var go = Instantiate(m_ItemList.Prefabs[ItemIndex], position, rotation);
        go.GetComponent<Rigidbody2D>().simulated = true;
        go.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;

        go.AddComponent<PickableItem>().ItemId = ItemIndex;

        NetworkServer.Spawn(go);

        ItemIndex = -1;
    }

    private void OnItemChanged(int oldIndex, int newIndex)
    {
        foreach (Transform child in m_ItemPivot.transform) {
            Destroy(child.gameObject);
        }

        if (newIndex > m_ItemList.Prefabs.Count || newIndex == -1)
            return;

        Instantiate(m_ItemList.Prefabs[newIndex], m_ItemPivot, false);
    }
}
