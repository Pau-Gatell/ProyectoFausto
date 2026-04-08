using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;           // ← Arrastra aquí tu Player

    [Header("Configuración")]
    public float distance = 6f;        // Distancia detrás del jugador
    public float height = 3f;          // Altura de la cámara
    public float smoothSpeed = 10f;    // Qué tan suave sigue (más alto = más rápido)

    [Header("Rotación con ratón")]
    public float rotationSpeed = 120f; // Velocidad de rotación con el ratón
    public float minVerticalAngle = -30f;  // Ángulo mínimo hacia abajo
    public float maxVerticalAngle = 60f;   // Ángulo máximo hacia arriba

    private float currentYaw = 0f;     // Rotación horizontal
    private float currentPitch = 20f;  // Rotación vertical (inclinación)

    void Start()
    {
        if (target == null)
            Debug.LogError("¡Asigna el Player al campo 'Target' de la cámara!");
    }

    void LateUpdate()   // LateUpdate es importante para cámaras
    {
        // Rotación con el ratón
        currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // Calculamos la posición deseada
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        // Movemos la cámara de forma suave
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // La cámara siempre mira al jugador (un poco por encima de los pies)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}