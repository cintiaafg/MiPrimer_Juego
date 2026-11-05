
using UnityEngine;
using TMPro;

public class SistemaPuntuacion : MonoBehaviour
{
    [Header("Referencias")]
    public MovimientoJugador jugador;

    public TMP_Text textoPuntuacion;
    public TMP_Text textoRecord;
    public TMP_Text textoGameOver;

    private int mejorPuntuacion;

    private const string CLAVE_RECORD =
        "Jengibre_MejorPuntuacion";

    void Start()
    {
        // Recuperar el récord guardado
        mejorPuntuacion =
            PlayerPrefs.GetInt(CLAVE_RECORD, 0);

        // Ocultar mensaje inicial
        if (textoGameOver != null)
        {
            textoGameOver.gameObject.SetActive(false);
        }

        ActualizarRecord();
    }

    void Update()
    {
        if (jugador == null)
            return;

        int puntuacionActual = jugador.Puntuacion;

        // Actualizar puntuación
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text =
                "PUNTOS: " + puntuacionActual;
        }

        // Guardar un nuevo récord
        if (puntuacionActual > mejorPuntuacion)
        {
            mejorPuntuacion = puntuacionActual;

            PlayerPrefs.SetInt(
                CLAVE_RECORD,
                mejorPuntuacion
            );

            PlayerPrefs.Save();

            ActualizarRecord();
        }

        // Mostrar Game Over
        if (jugador.JuegoTerminado)
        {
            if (textoGameOver != null)
            {
                textoGameOver.gameObject.SetActive(true);

                textoGameOver.text =
                    "¡PARTIDA TERMINADA!\n" +
                    "Puntuación: " + puntuacionActual;
            }
        }
    }

    void ActualizarRecord()
    {
        if (textoRecord != null)
        {
            textoRecord.text =
                "RÉCORD: " + mejorPuntuacion;
        }
    }
}
