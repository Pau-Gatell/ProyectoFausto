using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 11f;

    [Header("Jump Settings")]
    public float jumpForce = 9f;

    [Header("Dance Settings")]
    public bool canDance = true;

    private Rigidbody rb;
    private Animator anim;
    private Transform cameraTransform;
    private bool isGrounded;
    private bool isDancing = false;

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
        if (!isDancing)
        {
            Animate();
            CheckJump();
            CheckDanceInput();
        }
        else
        {
            // Mientras baila solo permitimos cancelar
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape))
            {
                StopDance();
            }
        }
    }

    void FixedUpdate()
    {
        if (cameraTransform == null || isDancing) return;

        Move();
        CheckGrounded();
    }

    // ====================== MOVIMIENTO ======================
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

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        rb.linearVelocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
        }
    }

    // ====================== SALTO ======================
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (anim != null)
            {
                anim.SetBool("IsJumping", true);
            }

            isGrounded = false;
        }
    }

    void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float distance = 0.6f;
        isGrounded = Physics.Raycast(origin, Vector3.down, distance);

        Debug.DrawRay(origin, Vector3.down * distance, isGrounded ? Color.green : Color.red);

        if (isGrounded && anim != null)
        {
            anim.SetBool("IsJumping", false);
        }
    }

    // ====================== ANIMACIONES ======================
    void Animate()
    {
        if (anim == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float movement = new Vector2(moveX, moveZ).magnitude;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        anim.SetFloat("Speed", movement * (isRunning ? runSpeed : walkSpeed));
        anim.SetBool("IsRunning", isRunning && movement > 0.01f);
    }

    // ====================== BAILE ======================
    void CheckDanceInput()
    {
        if (Input.GetKeyDown(KeyCode.B) && canDance && !isDancing)
        {
            OpenDanceMenu();
        }
    }

    void OpenDanceMenu()
    {
        Debug.Log("Menú de baile abierto");
        // De momento activamos el primer baile directamente
        PlaySpecificDance(1);
    }

    public void PlaySpecificDance(int danceIndex)
    {
        if (isDancing) return;

        isDancing = true;
        rb.linearVelocity = Vector3.zero;        // Paramos completamente el movimiento

        // Activamos el bool en el Animator
        anim.SetBool("IsDancing", true);

        // Opcional: si quieres usar triggers para diferentes bailes
        switch (danceIndex)
        {
            case 1:
                // anim.SetTrigger("Dance1");   // comentado porque ahora usamos bool
                break;
            case 2:
                // anim.SetTrigger("Dance2");
                break;
        }
    }

    public void StopDance()
    {
        isDancing = false;
        anim.SetBool("IsDancing", false);     // ← Esto hace que vuelva al movimiento
    }
}