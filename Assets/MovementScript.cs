using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    public float combo;
    public float multiplier = 1;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TextMeshProUGUI combo_text;
    [SerializeField] private TextMeshProUGUI multiplier_text;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform pogoEnd;
    [SerializeField] private SliderJoint2D pogoJoint;

    private Vector3 startPos;

    private float jumpForce;
    private float timeInComboRange;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // UI
        combo_text.text = "COMBO: " + combo.ToString();
        multiplier_text.text = "x " + string.Format("{0:0.0}", multiplier);

        multiplier = Mathf.Clamp(1 + combo / 10f, 1f, 2f);

        // Systems
        PogoJump();
        RotatePlayer();
        Respawn();

        // Normal movement
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    // -----------------------------
    // POGO SYSTEM
    // -----------------------------
    private void PogoJump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            JointTranslationLimits2D limits = pogoJoint.limits;
            JointMotor2D motor = pogoJoint.motor;

            limits.min = 0.88f;
            limits.max = 1.3f - 0.042f * jumpForce;

            pogoJoint.limits = limits;
            pogoJoint.motor = motor;

            jumpForce = Mathf.Clamp(jumpForce + Time.deltaTime * 15f, 0f, 10f);
        }
        else if (jumpForce > 0f)
        {
            JointTranslationLimits2D limits = pogoJoint.limits;
            JointMotor2D motor = pogoJoint.motor;

            limits.max = 1.3f;
            pogoJoint.limits = limits;

            motor.motorSpeed = jumpForce * multiplier;
            pogoJoint.motor = motor;

            StartCoroutine(PogoReset());
            jumpForce = 0f;

            if (IsInComboRange())
                combo += 1;
            else
                combo = 0;
        }

        // Combo timer
        if (IsInComboRange())
            timeInComboRange += Time.deltaTime;
        else
            timeInComboRange = 0f;

        if (IsInComboRange() && timeInComboRange > 0.25f)
            combo = 0;
    }

    private IEnumerator PogoReset()
    {
        yield return new WaitForSeconds(0.2f);

        JointMotor2D motor = pogoJoint.motor;
        JointTranslationLimits2D limits = pogoJoint.limits;

        motor.motorSpeed = 0f;
        limits.min = 1.26f;

        pogoJoint.motor = motor;
        pogoJoint.limits = limits;
    }

    private bool IsInComboRange()
    {
        return Physics2D.CircleCast(pogoEnd.position, 0.3f, Vector2.zero, 0.3f, groundLayer);
    }

    // -----------------------------
    // ROTATION
    // -----------------------------
    private void RotatePlayer()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = cam.ScreenToWorldPoint(mousePosition);

        Vector2 direction = (mousePosition - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float angleDifference = Mathf.DeltaAngle(rb.rotation, targetAngle);
        float rotationSpeed = Mathf.Lerp(0.1f, 40f, Mathf.Abs(angleDifference) / 180f);

        float rotateAmount = targetAngle - rb.rotation;
        rotateAmount = Mathf.Repeat(rotateAmount + 180f, 360f) - 180f;

        float rotateDirection = Mathf.Sign(rotateAmount);
        float torque = rotateDirection * rotationSpeed;

        rb.AddTorque(torque);
    }

    // -----------------------------
    // MOVEMENT HELPERS
    // -----------------------------
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

    // -----------------------------
    // RESPAWN
    // -----------------------------
    private void Respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            transform.rotation = Quaternion.identity;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            startPos = transform.position;
        }
    }
}
