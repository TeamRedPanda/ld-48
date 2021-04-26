using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : NetworkBehaviour, IInteractable
{

    [SyncVar(hook = nameof(OnCannonBallChanged))]
    private bool m_HasCannonBall = false;

    [SyncVar(hook = nameof(OnGunpowderChanged))]
    private bool m_HasGunpowder = false;

    [SerializeField] private EquipableItemList m_EquipableItems = default;
    [SerializeField] private GameObject m_CannonBall = default;
    [SerializeField] private GameObject m_Gunpowder = default;

    [SerializeField] private Image m_CannonBallImage = default;
    [SerializeField] private Image m_GunpowderImage = default;

    public InteractionType BeginInteraction(GameObject gameObject)
    {
        var itemHolder = gameObject.GetComponent<ItemHolder>();

        if (itemHolder.ItemIndex == ItemHolder.Empty) {
            CmdTryFiringCannon();
        } else {
            CmdTryFillingCannon(gameObject);
        }

        return InteractionType.None;
    }

    public void EndInteraction()
    {
    }

    private void OnCannonBallChanged(bool _, bool newState)
    {
        var color = m_CannonBallImage.color;

        color.a = newState ? 1f : 0.2f;

        m_CannonBallImage.color = color;
    }

    private void OnGunpowderChanged(bool _, bool newState)
    {
        var color = m_GunpowderImage.color;

        color.a = newState ? 1f : 0.2f;

        m_GunpowderImage.color = color;
    }

    void Awake()
    {
        OnCannonBallChanged(false, false);
        OnGunpowderChanged(false, false);
    }

    [Command(requiresAuthority = false)]
    private void CmdTryFiringCannon()
    {
        if (!m_HasCannonBall || !m_HasGunpowder)
            return;

        // Do stuff
        m_HasCannonBall = false;
        m_HasGunpowder = false;
    }

    [Command(requiresAuthority = false)]
    private void CmdTryFillingCannon(GameObject player)
    {
        var itemHolder = player.GetComponent<ItemHolder>();

        var cannonBallIndex = m_EquipableItems.FindIndex(m_CannonBall);
        var gunpowderIndex = m_EquipableItems.FindIndex(m_Gunpowder);

        if (itemHolder.ItemIndex == cannonBallIndex && !m_HasCannonBall) {
            itemHolder.ItemIndex = ItemHolder.Empty;
            m_HasCannonBall = true;
        }

        if (itemHolder.ItemIndex == gunpowderIndex && !m_HasGunpowder) {
            itemHolder.ItemIndex = ItemHolder.Empty;
            m_HasGunpowder = true;
        }
    }
}

