
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

    [Header("Botones de Game Over")]
    public GameObject botonReiniciar;
    public GameObject botonMenu;

    private int mejorPuntuacion;
    private bool partidaFinalizada = false;

    private const string CLAVE_RECORD =
        "Jengibre_MejorPuntuacion";

    void Start()
    {
        Time.timeScale = 1f;

        // Recuperar el récord guardado.
        mejorPuntuacion =
            PlayerPrefs.GetInt(CLAVE_RECORD, 0);

        // Ocultar mensaje de Game Over.
        if (textoGameOver != null)
            textoGameOver.gameObject.SetActive(false);

        // Ocultar botones al iniciar.
        if (botonReiniciar != null)
            botonReiniciar.SetActive(false);

        if (botonMenu != null)
            botonMenu.SetActive(false);

        ActualizarRecord();
    }

    void Update()
    {
        if (jugador == null)
            return;

        int puntuacionActual = jugador.Puntuacion;

        // Actualizar puntuación.
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text =
                "PUNTOS: " + puntuacionActual;
        }

        // Actualizar nivel.
        ActualizarNivel(puntuacionActual);

        // Guardar nuevo récord.
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

        // Detectar final de partida.
        if (jugador.JuegoTerminado && !partidaFinalizada)
        {
            partidaFinalizada = true;

            // Mostrar Game Over.
            if (textoGameOver != null)
            {
                textoGameOver.gameObject.SetActive(true);

                textoGameOver.text =
                    "¡PARTIDA TERMINADA!\n" +
                    "Puntuación: " + puntuacionActual;
            }

            // Mostrar botón Reiniciar.
            if (botonReiniciar != null)
                botonReiniciar.SetActive(true);

            // Mostrar botón Menú Principal.
            if (botonMenu != null)
                botonMenu.SetActive(true);
        }
    }

    // Actualizar nivel.
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

    // Actualizar récord.
    void ActualizarRecord()
    {
        if (textoRecord != null)
        {
            textoRecord.text =
                "RÉCORD: " + mejorPuntuacion;
        }
    }

    // Reiniciar partida.
    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Regresar al menú principal.
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MenuPrincipal");
    }
}
