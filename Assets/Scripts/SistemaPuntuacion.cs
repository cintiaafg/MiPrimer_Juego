
using UnityEngine;
using TMPro;

public class SistemaPuntuacion : MonoBehaviour
{
    [Header("Referencias")]
    public MovimientoJugador jugador;

    public TMP_Text textoPuntuacion;
    public TMP_Text textoRecord;
    public TMP_Text textoGameOver;
    public TMP_Text textoNivel;

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

        // Actualizar nivel
        ActualizarNivel(puntuacionActual);

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


void ActualizarNivel(int puntos)
{
    if (textoNivel == null)
    {
        Debug.LogError("No está asignado TextoNivel en GameManager");
        return;
    }

    if (puntos < 80)
    {
        textoNivel.text = "NIVEL 1 - FACIL";
    }
    else if (puntos < 50)
    {
        textoNivel.text = "NIVEL 2 - MEDIO";
    }
    else
    {
        textoNivel.text = "NIVEL 3 - DIFICIL";
    }

    Debug.Log("Puntos: " + puntos +
              " | Nivel: " + textoNivel.text);
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
