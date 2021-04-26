using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : NetworkBehaviour, IInteractable
{
    enum State
    {
        Empty,
        Filled
    }

    [SyncVar]
    private State m_State = State.Empty;

    [SyncVar]
    private float m_MeltProgress = 0f;

    [SerializeField] private float m_MeltSpeed = 0.25f;

    [SerializeField] private EquipableItemList m_EquipableItems = default;
    [SerializeField] private GameObject m_Ore = default;
    [SerializeField] private GameObject m_Bar = default;

    [SerializeField] private Image m_UIBar = default;

    public InteractionType BeginInteraction(GameObject gameObject)
    {
        var itemHolder = gameObject.GetComponent<ItemHolder>();

        if (itemHolder.ItemIndex != ItemHolder.Empty && m_State == State.Filled)
            return InteractionType.None;

        if (itemHolder.ItemIndex == ItemHolder.Empty && m_State == State.Empty)
            return InteractionType.None;

        if (itemHolder.ItemIndex != ItemHolder.Empty && m_State == State.Empty) {
            CmdGetItemFromPlayer(gameObject);
        }

        if (itemHolder.ItemIndex == ItemHolder.Empty && m_State == State.Filled) {
            CmdGiveItemToPlayer(gameObject);
        }

        return InteractionType.None;
    }

    public void EndInteraction()
    {
    }

    [Command(requiresAuthority = false)]
    private void CmdGetItemFromPlayer(GameObject player)
    {
        var itemHolder = player.GetComponent<ItemHolder>();

        if (itemHolder.ItemIndex != m_EquipableItems.FindIndex(m_Ore))
            return;

        if (m_State == State.Filled)
            return;

        itemHolder.ItemIndex = ItemHolder.Empty;
        m_State = State.Filled;
        m_MeltProgress = 0f;
    }

    [Command(requiresAuthority = false)]
    private void CmdGiveItemToPlayer(GameObject player)
    {
        var itemHolder = player.GetComponent<ItemHolder>();

        if (m_State == State.Filled && m_MeltProgress >= 1f && itemHolder.ItemIndex == ItemHolder.Empty) {
            itemHolder.ItemIndex = m_EquipableItems.FindIndex(m_Bar);
            m_State = State.Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && m_MeltProgress <= 1f && m_State == State.Filled) {
            m_MeltProgress = Mathf.Min(m_MeltProgress + m_MeltSpeed * Time.deltaTime, 1f);
        }

        m_UIBar.transform.parent.gameObject.SetActive(m_State == State.Filled);

        m_UIBar.fillAmount = m_MeltProgress;
    }
}
