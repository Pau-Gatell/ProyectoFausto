using UnityEngine;

public class GlideController : MonoBehaviour
{
    [Header("Glide Settings")]
    public float glideStartHeight = 3f;
    public float glideFallSpeed = -2f;

    private Rigidbody rb;
    private Animator anim;

    private bool isGrounded;
    private bool isGliding;
    private float jumpStartY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CheckGround();

        if (isGrounded)
        {
            isGliding = false;
            anim.SetBool("IsGliding", false);

            jumpStartY = transform.position.y;
            return;
        }

        float fallDistance = jumpStartY - transform.position.y;

        if (!isGliding &&
            rb.linearVelocity.y < 0 &&
            fallDistance > glideStartHeight)
        {
            StartGlide();
        }

        if (isGliding)
        {
            Vector3 velocity = rb.linearVelocity;

            if (velocity.y < glideFallSpeed)
            {
                velocity.y = glideFallSpeed;
                rb.linearVelocity = velocity;
            }
        }
    }

    void StartGlide()
    {
        isGliding = true;

        if (anim != null)
            anim.SetBool("IsGliding", true);
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.6f
        );
    }
}