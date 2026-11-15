
using UnityEngine;
using System.Collections.Generic;

public class GeneradorNiveles : MonoBehaviour
{
    [Header("Jugador")]
    public Transform jugador;
    public MovimientoJugador movimiento;

    [Header("Prefabs de segmentos")]
    public GameObject[] segmentosFaciles;
    public GameObject[] segmentosMedios;
    public GameObject[] segmentosDificiles;

    [Header("Configuracion")]
    public float longitudSegmento = 20f;
    public int segmentosIniciales = 5;
    public float distanciaGeneracion = 60f;
    public float distanciaEliminacion = 80f;

    [Header("Inicio de generacion")]
    public float posicionInicialZ = 0f;

    private float siguienteZ;

    private List<GameObject> segmentosActivos =
        new List<GameObject>();

    void Start()
    {
        if (jugador == null || movimiento == null)
        {
            Debug.LogError(
                "Falta asignar el Jugador o Movimiento."
            );
            enabled = false;
            return;
        }

        if (longitudSegmento <= 0 || segmentosIniciales < 1)
        {
            Debug.LogError("Revisa la longitud y cantidad de segmentos.");
            enabled = false;
            return;
        }

        // Iniciar la generacion desde la posicion configurada
        siguienteZ = posicionInicialZ;

        // Crear los primeros segmentos
        for (int i = 0; i < segmentosIniciales; i++)
        {
            if (!GenerarSegmento())
                break;
        }
    }

    void Update()
    {
        if (jugador == null || movimiento == null)
            return;

        if (movimiento.JuegoTerminado)
            return;

        // Generar segmentos antes de llegar al final
        while (jugador.position.z + distanciaGeneracion >= siguienteZ)
        {
            if (!GenerarSegmento())
                break;
        }

        // Eliminar segmentos que quedaron atras
        for (int i = segmentosActivos.Count - 1; i >= 0; i--)
        {
            GameObject segmento = segmentosActivos[i];

            if (segmento == null)
            {
                segmentosActivos.RemoveAt(i);
                continue;
            }

            float finalSegmento =
                segmento.transform.position.z + longitudSegmento;

            if (jugador.position.z >
                finalSegmento + distanciaEliminacion)
            {
                Destroy(segmento);
                segmentosActivos.RemoveAt(i);
            }
        }
    }

    bool GenerarSegmento()
    {
        int puntos = movimiento.Puntuacion;

        GameObject[] opciones;

        // Elegir dificultad segun los puntos
        if (puntos < 20)
        {
            opciones = segmentosFaciles;
        }
        else if (puntos < 50)
        {
            opciones = segmentosMedios;
        }
        else
        {
            opciones = segmentosDificiles;
        }

        // Comprobar prefabs disponibles
        if (opciones == null || opciones.Length == 0)
        {
            Debug.LogError(
                "No hay prefabs asignados para esta dificultad."
            );
            return false;
        }

        // Elegir un prefab aleatorio
        GameObject prefab = opciones[
            Random.Range(0, opciones.Length)
        ];

        if (prefab == null)
        {
            Debug.LogError("Hay un elemento vacío en los prefabs.");
            return false;
        }

        // Crear el segmento en la siguiente posicion
        Vector3 posicion = new Vector3(
            0f,
            0f,
            siguienteZ
        );

        GameObject nuevoSegmento = Instantiate(
            prefab,
            posicion,
            Quaternion.identity
        );

        segmentosActivos.Add(nuevoSegmento);

        Debug.Log(
            "SEGMENTO GENERADO: " +
            nuevoSegmento.name +
            " | POSICION Z: " + siguienteZ +
            " | PUNTOS: " + puntos
        );

        // Preparar la posicion del siguiente segmento
        siguienteZ += longitudSegmento;

        return true;
    }
}
