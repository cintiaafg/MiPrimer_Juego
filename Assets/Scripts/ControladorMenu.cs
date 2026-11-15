
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("EscenaPrincipal");
    }

    public void Salir()
    {
        Debug.Log("Cerrando videojuego");

        Application.Quit();
    }
}
