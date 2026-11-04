
using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    [Header("Objetivo de seguimiento")]
    public Transform objetivo;

    [Header("Configuración de cámara")]
    public Vector3 desplazamiento = new Vector3(0, 5, -7);

    public float suavizado = 5f;

    void LateUpdate()
    {
        if (objetivo == null)
            return;

        // Calcular la posición deseada
        Vector3 posicionDeseada =
            objetivo.position + desplazamiento;

        // Movimiento suave de la cámara
        transform.position = Vector3.Lerp(
            transform.position,
            posicionDeseada,
            suavizado * Time.deltaTime
        );

        // Mantener al personaje en el centro de la vista
        transform.LookAt(
            objetivo.position + Vector3.up * 1.2f
        );
    }
}
