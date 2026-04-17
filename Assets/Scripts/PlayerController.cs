using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 11f;
    public float swimSpeed = 4f;           // Velocidad al nadar

    [Header("Jump Settings")]
    public float jumpForce = 9f;

    [Header("Dance Settings")]
    public bool canDance = true;

    private Rigidbody rb;
    private Animator anim;
    private Transform cameraTransform;
    private bool isGrounded;
    private bool isDancing = false;
    private bool isSwimming = false;

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
        if (!isDancing && !isSwimming)
        {
            Animate();
            CheckJump();
            CheckDanceInput();
        }
        else if (isSwimming)
        {
            SwimMovement();
            CheckDanceInput();        // Opcional: permitir bailar dentro del agua
        }
        else if (isDancing)
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Escape))
                StopDance();
        }
    }

    void FixedUpdate()
    {
        if (isDancing) return;

        if (isSwimming)
        {
            // Movimiento de natación ya se maneja en Update
        }
        else
        {
            Move();
            CheckGrounded();
        }
    }

    // ====================== MOVIMIENTO NORMAL ======================
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
                anim.SetBool("IsJumping", true);

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
            anim.SetBool("IsJumping", false);
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

    // ====================== NATACIÓN ======================
    void SwimMovement()
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

        rb.linearVelocity = new Vector3(move.x * swimSpeed, rb.linearVelocity.y * 0.95f, move.z * swimSpeed);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }

        float movement = new Vector2(moveX, moveZ).magnitude;
        anim.SetFloat("Speed", movement * swimSpeed);
    }

    // ====================== DETECCIÓN DE AGUA (CON TAG) ======================
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            EnterWater();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ExitWater();
        }
    }

    void EnterWater()
    {
        isSwimming = true;
        anim.SetBool("IsSwimming", true);
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * 0.4f, 2f, rb.linearVelocity.z * 0.4f); // pequeño impulso hacia arriba
    }

    void ExitWater()
    {
        isSwimming = false;
        anim.SetBool("IsSwimming", false);
        rb.useGravity = true;
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
        PlaySpecificDance(1);        // Cambia el número según el baile que quieras
    }

    public void PlaySpecificDance(int danceIndex)
    {
        if (isDancing) return;

        isDancing = true;
        rb.linearVelocity = Vector3.zero;
        anim.SetBool("IsDancing", true);
    }

    public void StopDance()
    {
        isDancing = false;
        anim.SetBool("IsDancing", false);
    }
}