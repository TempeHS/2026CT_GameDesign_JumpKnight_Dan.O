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

    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        HandleChargeJump();
        Flip();
    }

    private void FixedUpdate()
    {
        // No movement while charging or airborne (Jump King rule)
        if (!isCharging && IsGrounded())
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    private void HandleChargeJump()
    {
        // Start charging only if grounded
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isCharging = true;
            currentCharge = minJumpPower;

            // Stop sliding when charging begins
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // Increase charge while holding
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, minJumpPower, maxJumpPower);
        }

        // Release jump
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            isCharging = false;

            // Launch upward with charged force
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
            // Wall hit detection
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                float pushDirection = -Mathf.Sign(contact.normal.x);
                float pushForce = 3f;

                // Bounce back
                rb.linearVelocity = new Vector2(pushDirection * pushForce, rb.linearVelocity.y);
            }
        }
    }
}