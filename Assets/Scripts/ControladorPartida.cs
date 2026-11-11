
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPartida : MonoBehaviour
{
    public MovimientoJugador jugador;
    public GameObject panelGameOver;

    private bool partidaTerminada = false;

    void Start()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (jugador == null)
            return;

        if (jugador.JuegoTerminado && !partidaTerminada)
        {
            partidaTerminada = true;

            if (panelGameOver != null)
                panelGameOver.SetActive(true);
        }
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
