using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;           // Subí un poco la velocidad (puedes bajarla si quieres)
    public float jumpForce = 8f;       // Fuerza del salto (ajusta según necesites)

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

        rb.freezeRotation = true;           // Evita que el jugador se caiga de lado
        anim.applyRootMotion = false;       // Muy importante para no hundirse
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
        CheckGrounded();        // Detectamos si está tocando el suelo
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Dirección basada en cámara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveZ + right * moveX;

        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);

        // Rotación hacia donde se mueve
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
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Resetea velocidad vertical
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Activar animación de salto
            if (anim != null)
                anim.SetBool("IsJumping", true);

            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        // Raycast para detectar el suelo de forma más fiable
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.3f);
    }

    void Animate()
    {
        if (anim == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float movement = new Vector2(moveX, moveZ).magnitude;

        anim.SetFloat("Speed", movement);

        // Control de la animación de salto
        if (isGrounded && anim.GetBool("IsJumping"))
        {
            anim.SetBool("IsJumping", false);   // Volvemos a Idle/Walk cuando toca el suelo
        }
    }
}