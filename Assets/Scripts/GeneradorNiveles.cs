
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

    private float siguienteZ = 0f;
    private List<GameObject> segmentosActivos =
        new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < segmentosIniciales; i++)
        {
            GenerarSegmento();
        }
    }

    void Update()
    {
        if (jugador == null || movimiento == null)
            return;

        // Generar nuevos segmentos por delante
        if (jugador.position.z + distanciaGeneracion > siguienteZ)
        {
            GenerarSegmento();
        }

        // Eliminar segmentos que quedaron atras
        for (int i = segmentosActivos.Count - 1; i >= 0; i--)
        {
            GameObject segmento = segmentosActivos[i];

            if (jugador.position.z >
                segmento.transform.position.z +
                longitudSegmento + distanciaEliminacion)
            {
                Destroy(segmento);
                segmentosActivos.RemoveAt(i);
            }
        }
    }

    void GenerarSegmento()
    {
        int puntos = movimiento.Puntuacion;

        GameObject[] opciones;

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

        if (opciones == null || opciones.Length == 0)
            return;

        int indice = Random.Range(0, opciones.Length);

        GameObject nuevoSegmento = Instantiate(
            opciones[indice],
            new Vector3(0, 0, siguienteZ),
            Quaternion.identity
        );

        segmentosActivos.Add(nuevoSegmento);

        siguienteZ += longitudSegmento;
    }
}
