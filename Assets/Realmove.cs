using UnityEngine;

public class JumpKingMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump King Charge Jump")]
    public float minJumpForce = 5f;
    public float maxJumpForce = 22f;
    public float chargeSpeed = 20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float chargeAmount = 0f;
    private bool isCharging = false;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGround();

        // Horizontal movement ONLY when grounded
        if (isGrounded && !isCharging)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }

        HandleJumpCharge();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleJumpCharge()
    {
        // Start charging when jump is held on ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            chargeAmount = minJumpForce;
        }

        // Increase charge while holding
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            chargeAmount += chargeSpeed * Time.deltaTime;
            chargeAmount = Mathf.Clamp(chargeAmount, minJumpForce, maxJumpForce);
        }

        // Release jump
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            // Apply jump force
            rb.velocity = new Vector2(rb.velocity.x, chargeAmount);

            // No control in air
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}

