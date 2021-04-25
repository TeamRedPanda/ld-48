using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class PlayerInput : NetworkBehaviour
{
    [SerializeField] Rigidbody2D m_Rigidbody = default;

    [SerializeField] float m_Speed = 5f;
    [SerializeField] float m_JumpVelocity = 5f;
    [SerializeField] float m_FallMultiplier = 2.5f;
    [SerializeField] float m_LowJumpMultiplier = 2f;

    private InputControls m_InputControls;

    private bool m_IsJumping;
    private bool m_ShouldJump;

    [SerializeField] LayerMask m_GroundMask = default;
    private RaycastHit2D[] m_GroundRaycastHit = new RaycastHit2D[64];
    private bool m_IsGrounded = false;

    [SerializeField] LayerMask m_StairsMask = default;
    [SerializeField] float m_ClimbSpeed = 5f;
    [SerializeField] bool m_Climbing = false;

    void Update()
    {
        Vector2 position = (Vector2)transform.position;
        int hitCount = Physics2D.LinecastNonAlloc(position, position - Vector2.up * 0.1f, m_GroundRaycastHit, m_GroundMask);

        if (hitCount > 0) {
            m_IsGrounded = true;
            m_Climbing = false;
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || m_InputControls == null)
            return;

        var velocity = m_Rigidbody.velocity;

        var movementInput = m_InputControls.Player.Movement.ReadValue<Vector2>();

        ComputeHorizontalVelocity(ref velocity, movementInput.x);
        ComputeVerticalVelocity(ref velocity, movementInput.y);

        m_Rigidbody.velocity = velocity;
    }

    private void ComputeHorizontalVelocity(ref Vector2 velocity, float horizontalInput)
    {
        if (m_Climbing)
            return;

        velocity.x = horizontalInput * m_Speed;
    }

    private void ComputeVerticalVelocity(ref Vector2 velocity, float verticalInput)
    {
        // Climbing
        if (Mathf.Abs(verticalInput) > 0) {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, m_StairsMask);

            if (hit.collider != null) {
                if (!m_Climbing) {
                    var position = transform.position;
                    position.x = hit.transform.position.x;
                    transform.position = position;

                    velocity.x = 0;
                }

                m_Climbing = true;
            } else {
                m_Climbing = false;
            }
        }

        // Gravity
        if (velocity.y < 0 && !m_Climbing) {
            velocity.y += Physics2D.gravity.y * (m_FallMultiplier - 1) * Time.deltaTime;
        } else if (!m_IsJumping && !m_Climbing) {
            velocity.y += Physics2D.gravity.y * (m_LowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (m_Climbing) {
            m_Rigidbody.gravityScale = 0f;

            velocity.y = verticalInput * m_ClimbSpeed;
        } else {
            m_Rigidbody.gravityScale = 1f;
        }

        // Jump
        if (m_ShouldJump && (m_IsGrounded || m_Climbing)) {
            velocity.y = m_JumpVelocity;
            m_ShouldJump = false;
            m_Climbing = false;
        }
    }

    void HandleJumpAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            m_ShouldJump = true;
            m_IsJumping = true;
        }

        if (context.phase == InputActionPhase.Canceled) {
            m_IsJumping = false;
        }
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        m_InputControls = new InputControls();
        m_InputControls.Player.Jump.performed += HandleJumpAction;
        m_InputControls.Player.Jump.canceled += HandleJumpAction;
        m_InputControls.Player.Enable();
    }
}
