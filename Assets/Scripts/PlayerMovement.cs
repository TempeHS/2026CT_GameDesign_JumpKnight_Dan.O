using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 6f;

   
    private float minJumpPower = 5f;
    private float maxJumpPower = 22f;
    private float chargeRate = 20f;
    private float currentCharge = 0f;
    private bool isCharging = false;

    public float wallCheckDistance = 0.1f;
    private bool isTouchingWall;
    private bool isFacingRight = true;

    // Ice
    public bool onIce = false;
    public float iceFriction = 0.5f;   // how fast you slow down
    public float iceAcceleration = 12f;
    private bool justLandedOnIce = false;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        HandleChargeJump();
        Flip();

        // Wall detection
        isTouchingWall =
            Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, groundLayer) ||
            Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, groundLayer);
    }

    private void FixedUpdate()
    {
        float targetSpeed = horizontal * speed;

        if (!isCharging)
        {
            if (onIce)
            {
                // accelerate based on input
                float vx = rb.linearVelocity.x + horizontal * iceAcceleration * Time.fixedDeltaTime;

                // apply friction toward 0 when no input or changing direction
                vx = Mathf.MoveTowards(vx, 0f, iceFriction * Time.fixedDeltaTime);

                rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
            }
            else
            {
                // normal ground movement
                if (IsGrounded())
                {
                    rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
                }
            }
        }

        // wall-stick only when NOT on ice
        if (!onIce && isTouchingWall && Mathf.Abs(rb.linearVelocity.x) > 0)
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

            // Freeze horizontal movement
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minJumpPower, maxJumpPower);
        }

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
        
        onIce = collision.collider.CompareTag("Ice");

        
        foreach (ContactPoint2D contact in collision.contacts)
        {
            bool hitLeftSide = contact.point.x <= collision.collider.bounds.min.x + 0.05f;
            bool hitRightSide = contact.point.x >= collision.collider.bounds.max.x - 0.05f;

            if (hitLeftSide || hitRightSide)
            {
                float pushDirection = hitLeftSide ? -1f : 1f;
                float pushForce = 4f;

                rb.linearVelocity = new Vector2(pushDirection * pushForce, rb.linearVelocity.y);
            }
        }

        if (collision.collider.CompareTag("Ice"))
        {
            onIce = true;
            justLandedOnIce = true;   
        }
        else
        {
            onIce = false;
        }
    }
}