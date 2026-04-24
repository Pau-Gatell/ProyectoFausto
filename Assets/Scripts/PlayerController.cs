using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 11f;
    public float swimSpeed = 4f;
    public Transform skatePosition;

    [Header("Skate Settings")]
    public GameObject skate;
    public float skateSpeed = 12f;
    private bool isNearSkate = false;
    private bool isOnSkate = false;

    [Header("Jump Settings")]
    public float jumpForce = 9f;

    [Header("Dance Settings")]
    public bool canDance = true;
    public GameObject danceMenuUI;

    private bool isDanceMenuOpen = false;

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
        // 👉 SKATE INPUT
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isNearSkate && !isOnSkate)
                MountSkate();
            else if (isOnSkate)
                DismountSkate();
        }

        if (!isDancing && !isSwimming)
        {
            Animate();
            CheckJump();
            CheckDanceInput();
        }
        else if (isSwimming)
        {
            SwimMovement();
            CheckDanceInput();
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
        }
        else
        {
            Move();
            CheckGrounded();
        }
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

        // 👉 CAMBIO DE VELOCIDAD SI ESTÁ EN SKATE
        float currentSpeed = isOnSkate ? skateSpeed : (isRunning ? runSpeed : walkSpeed);

        rb.linearVelocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
        }
    }

    // ====================== SKATE ======================
    void MountSkate()
    {
        isOnSkate = true;

        skate.transform.SetParent(skatePosition);
        skate.transform.localPosition = Vector3.zero;
        skate.transform.localRotation = Quaternion.identity;

        if (anim != null)
            anim.SetBool("IsSkating", true);
    }

    void DismountSkate()
    {
        isOnSkate = false;

        skate.transform.SetParent(null);
        skate.transform.position = transform.position + transform.forward * 1.5f;

        if (anim != null)
            anim.SetBool("IsSkating", false);
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

    // ====================== DETECCIÓN ======================
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
            EnterWater();

        if (other.gameObject == skate)
            isNearSkate = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
            ExitWater();

        if (other.gameObject == skate)
            isNearSkate = false;
    }

    void EnterWater()
    {
        isSwimming = true;
        anim.SetBool("IsSwimming", true);
        rb.useGravity = false;
    }

    void ExitWater()
    {
        isSwimming = false;
        anim.SetBool("IsSwimming", false);
        rb.useGravity = true;
    }

    // ====================== BAILE MENÚ ======================
    void CheckDanceInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isDanceMenuOpen)
                CloseDanceMenu();
            else if (canDance && !isDancing)
                OpenDanceMenu();
        }

        if (isDanceMenuOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseDanceMenu();
    }

    void OpenDanceMenu()
    {
        if (danceMenuUI != null)
        {
            danceMenuUI.SetActive(true);
            isDanceMenuOpen = true;

            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void CloseDanceMenu()
    {
        if (danceMenuUI != null)
            danceMenuUI.SetActive(false);

        isDanceMenuOpen = false;

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 👉 LLAMADO DESDE UI
    public void PlayDance(int danceNumber)
    {
        CloseDanceMenu();

        if (isDancing) return;

        isDancing = true;
        rb.linearVelocity = Vector3.zero;

        anim.SetInteger("DanceIndex", danceNumber);
    }

    public void StopDance()
    {
        isDancing = false;
        anim.SetInteger("DanceIndex", 0);
    }
}