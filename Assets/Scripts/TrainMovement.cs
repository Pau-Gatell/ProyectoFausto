using UnityEngine;
using DG.Tweening; // Es vital que tengas DOTween instalado

public class TrainMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    [Tooltip("Arrastra aquí tus objetos Waypoint 1 y Waypoint 2")]
    [SerializeField] private Transform[] waypoints;

    [Header("Ajustes de Movimiento")]
    [SerializeField] private float speed = 5f;       // Velocidad del tren
    [SerializeField] private float stationWait = 2f; // Tiempo de parada en segundos

    void Start()
    {
        // Comprobación de seguridad para evitar errores en la consola
        if (waypoints != null && waypoints.Length >= 2)
        {
            IniciarTrayecto();
        }
        else
        {
            Debug.LogWarning("Por favor, asigna al menos 2 Waypoints en el Inspector.");
        }
    }

    void IniciarTrayecto()
    {
        // Creamos una secuencia de DOTween
        Sequence trainSequence = DOTween.Sequence();

        foreach (Transform targetPoint in waypoints)
        {
            // 1. Calculamos la duración basada en la distancia para velocidad constante
            float distance = Vector3.Distance(transform.position, targetPoint.position);
            float duration = distance / speed;


            // 3. Movimiento: Se desplaza de forma lineal hasta el punto
            trainSequence.Append(transform.DOMove(targetPoint.position, duration).SetEase(Ease.Linear));

            // 4. Pausa: El tren espera en el Waypoint (simulando una estación)
            trainSequence.AppendInterval(stationWait);
        }

        // El loop en modo Yoyo hace que vaya del 1 al 2 y regrese del 2 al 1 infinitamente
        trainSequence.SetLoops(-1, LoopType.Yoyo);
    }
}