using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 6f;

    // Jump King charge jump values
    private float minJumpPower = 5f;
    private float maxJumpPower = 22f;
    private float chargeRate = 20f;
    private float currentCharge = 0f;
    private bool isCharging = false;

    public float wallCheckDistance = 0.1f;
    private bool isTouchingWall;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        // Horizontal input
        horizontal = Input.GetAxisRaw("Horizontal");

        // Charge jump logic
        HandleChargeJump();

        // Flip sprite
        Flip();

        // Wall detection (NO physics here)
        isTouchingWall =
            Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, groundLayer) ||
            Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, groundLayer);
    }

    private void FixedUpdate()
    {
        // Normal movement only when grounded and not charging
        if (!isCharging && IsGrounded())
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }

        
        if (isTouchingWall && Mathf.Abs(rb.linearVelocity.x) > 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void HandleChargeJump()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isCharging = true;
            currentCharge = minJumpPower;

            
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minJumpPower, maxJumpPower);
        }

        // Release jump
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            // Apply jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentCharge);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            bool hitLeftSide = contact.point.x <= collision.collider.bounds.min.x + 0.05f;
            bool hitRightSide = contact.point.x >= collision.collider.bounds.max.x - 0.05f;

            if (hitLeftSide || hitRightSide)
            {
                float pushDirection = hitLeftSide ? -1f : 1f;
                float pushForce = 4f; // tune this value

                // Bounce-back (still works perfectly)
                rb.linearVelocity = new Vector2(pushDirection * pushForce, rb.linearVelocity.y);
            }
        }
    }
}
