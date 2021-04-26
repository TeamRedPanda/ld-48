using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EyeController : NetworkBehaviour
{
    [SerializeField] Transform m_EyeTransform = default;

    Rigidbody2D m_Rigidbody;

    [SyncVar]
    float facing = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_EyeTransform.localScale = new Vector3(facing, 1f, 1f);

        if (!isLocalPlayer)
            return;

        if (m_Rigidbody.velocity.x < 0) {
            facing = -1f;
        } else if (m_Rigidbody.velocity.x > 0) {
            facing = 1f;
        }
    }
}
