using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : NetworkBehaviour, IInteractable
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

    private int m_CraftingItem = -1;

    [SerializeField] private EquipableItemList m_EquipableItems = default;
    [SerializeField] private GameObject m_Bar = default;
    [SerializeField] private GameObject m_Charcoal = default;
    [SerializeField] private GameObject m_CannonBall = default;
    [SerializeField] private GameObject m_Gunpowder = default;

    private bool m_Crafting = false;

    [SerializeField] private Image m_UIBar = default;

    public InteractionType BeginInteraction(GameObject gameObject)
    {
        var itemHolder = gameObject.GetComponent<ItemHolder>();

        if (itemHolder.ItemIndex != ItemHolder.Empty && m_State == State.Filled)
            return InteractionType.None;

        if (itemHolder.ItemIndex == ItemHolder.Empty && m_State == State.Empty)
            return InteractionType.None;

        if (itemHolder.ItemIndex != ItemHolder.Empty && m_State == State.Empty) {
            CmdStartCraft(gameObject);
            return InteractionType.None;
        }

        if (itemHolder.ItemIndex == ItemHolder.Empty && m_State == State.Filled) {
            CmdCraft(gameObject);
            return InteractionType.StopMovement;
        }

        return InteractionType.None;
    }

    public void EndInteraction()
    {
        CmdStopCrafting();
    }

    [Command(requiresAuthority = false)]
    private void CmdStartCraft(GameObject player)
    {
        var itemHolder = player.GetComponent<ItemHolder>();

        if (m_State == State.Filled)
            return;

        if (itemHolder.ItemIndex != m_EquipableItems.FindIndex(m_Bar) && itemHolder.ItemIndex != m_EquipableItems.FindIndex(m_Charcoal))
            return;

        if (itemHolder.ItemIndex == m_EquipableItems.FindIndex(m_Bar)) {
            m_CraftingItem = m_EquipableItems.FindIndex(m_CannonBall);
        } else if (itemHolder.ItemIndex == m_EquipableItems.FindIndex(m_Charcoal)) {
            m_CraftingItem = m_EquipableItems.FindIndex(m_Gunpowder);
        }

        itemHolder.ItemIndex = ItemHolder.Empty;
        m_State = State.Filled;
        m_MeltProgress = 0f;
    }

    [Command(requiresAuthority = false)]
    private void CmdStopCrafting()
    {
        m_Crafting = false;
    }

    [Command(requiresAuthority = false)]
    private void CmdCraft(GameObject player)
    {
        if (m_MeltProgress < 1f) {
            m_Crafting = true;

            return;
        }

        if (m_State == State.Empty)
            return;

        var heldItem = player.GetComponent<ItemHolder>();
        heldItem.ItemIndex = m_CraftingItem;
        m_State = State.Empty;
        m_CraftingItem = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer && m_MeltProgress <= 1f && m_State == State.Filled && m_Crafting) {
            m_MeltProgress = Mathf.Min(m_MeltProgress + m_MeltSpeed * Time.deltaTime, 1f);
        }

        m_UIBar.transform.parent.gameObject.SetActive(m_State == State.Filled);

        m_UIBar.fillAmount = m_MeltProgress;
    }
}
