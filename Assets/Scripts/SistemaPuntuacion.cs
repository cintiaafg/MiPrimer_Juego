using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SistemaPuntuacion : MonoBehaviour
{
    [Header("Referencias")]
    public MovimientoJugador jugador;

    public TMP_Text textoPuntuacion;
    public TMP_Text textoRecord;
    public TMP_Text textoGameOver;
    public TMP_Text textoNivel;

    [Header("Boton de reinicio")]
    public GameObject botonReiniciar;

    private int mejorPuntuacion;
    private bool partidaFinalizada = false;

    private const string CLAVE_RECORD =
        "Jengibre_MejorPuntuacion";

    void Start()
    {
        // Recuperar el récord guardado
        mejorPuntuacion =
            PlayerPrefs.GetInt(CLAVE_RECORD, 0);

        // Ocultar mensaje de Game Over
        if (textoGameOver != null)
        {
            textoGameOver.gameObject.SetActive(false);
        }

        // Ocultar botón de reinicio
        if (botonReiniciar != null)
        {
            botonReiniciar.SetActive(false);
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

        // Detectar el final de la partida
        if (jugador.JuegoTerminado && !partidaFinalizada)
        {
            partidaFinalizada = true;

            // Mostrar mensaje de Game Over
            if (textoGameOver != null)
            {
                textoGameOver.gameObject.SetActive(true);

                textoGameOver.text =
                    "¡PARTIDA TERMINADA!\n" +
                    "Puntuación: " + puntuacionActual;
            }

            // Mostrar botón de reinicio
            if (botonReiniciar != null)
            {
                botonReiniciar.SetActive(true);
            }
        }
    }

    // Actualizar el nivel según la puntuación
    void ActualizarNivel(int puntos)
    {
        if (textoNivel == null)
            return;

        if (puntos < 100)
        {
            textoNivel.text = "NIVEL 1 - FACIL";
        }
        else if (puntos < 200)
        {
            textoNivel.text = "NIVEL 2 - MEDIO";
        }
        else
        {
            textoNivel.text = "NIVEL 3 - DIFICIL";
        }
    }

    // Actualizar el récord
    void ActualizarRecord()
    {
        if (textoRecord != null)
        {
            textoRecord.text =
                "RÉCORD: " + mejorPuntuacion;
        }
    }

    // Reiniciar la escena actual
    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}