using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                // Arrastra aquí tu Player (el padre)

    [Header("Configuración")]
    public float distance = 6.5f;           // Distancia detrás del jugador
    public float height = 3.5f;             // Altura de la cámara
    public float smoothSpeed = 12f;         // Suavizado del seguimiento (más alto = más rápido)

    [Header("Rotación con ratón")]
    public float rotationSpeed = 140f;      // Velocidad de rotación
    public float minPitch = -35f;           // Ángulo mínimo hacia abajo
    public float maxPitch = 65f;            // Ángulo máximo hacia arriba

    private float yaw = 0f;                 // Rotación horizontal
    private float pitch = 25f;              // Rotación vertical

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Asigna el Player al campo 'Target' de la cámara");
            return;
        }

        // Posición inicial detrás del jugador
        yaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Rotación con el ratón
        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Calculamos la posición deseada
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        // Seguimiento suave
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // La cámara siempre mira al jugador (un poco por encima del centro)
        transform.LookAt(target.position + Vector3.up * 1.6f);
    }
}