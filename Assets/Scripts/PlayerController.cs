using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 9f;        // Un poco más alta para probar

    private Rigidbody rb;
    private Animator anim;
    private Transform cameraTransform;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        rb.freezeRotation = true;
        anim.applyRootMotion = false;
    }

    void Update()
    {
        Animate();
        CheckJump();
    }

    void FixedUpdate()
    {
        if (cameraTransform == null) return;

        Move();
        CheckGrounded();
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveZ + right * moveX;

        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
        }
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Activamos la animación
            if (anim != null)
            {
                anim.SetBool("IsJumping", true);
                Debug.Log("✅ Animación de salto ACTIVADA (IsJumping = true)");
            }
            else
            {
                Debug.LogError("❌ No se encontró el Animator");
            }

            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        // Raycast más fiable y tolerante
        Vector3 origin = transform.position + Vector3.up * 0.1f;   // Empieza un poco por encima de los pies
        float distance = 0.6f;                                     // Distancia del raycast

        isGrounded = Physics.Raycast(origin, Vector3.down, distance);

        // Debug visual (para que veas la línea)
        Debug.DrawRay(origin, Vector3.down * distance, isGrounded ? Color.green : Color.red);

        // Reset IsJumping cuando aterriza
        if (isGrounded && anim != null)
        {
            anim.SetBool("IsJumping", false);
        }
    }

    void Animate()
    {
        if (anim == null) return;

        float movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
        anim.SetFloat("Speed", movement);

        // Reset de IsJumping cuando toca el suelo
        if (isGrounded)
        {
            if (anim.GetBool("IsJumping") == true)
            {
                anim.SetBool("IsJumping", false);
                Debug.Log("✅ IsJumping puesto a false (aterrizaje)");
            }
        }
    }
}