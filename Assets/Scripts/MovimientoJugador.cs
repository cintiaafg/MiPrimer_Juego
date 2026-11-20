
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadAvance = 5f;
    public float velocidadLateral = 5f;
    public float fuerzaSalto = 5f;

    [Header("Dificultad progresiva")]
    public float incrementoVelocidad = 0.5f;
    public int puntosPorNivel = 10;
    public float velocidadMaxima = 12f;

    [Header("Efectos visuales")]
    public ParticleSystem particulasSalto;
    public ParticleSystem particulasAterrizaje;

    // Controles móviles
    public static float mobileX;
    public static float mobileZ;

    private Rigidbody rb;
    private bool puedeSaltar;
    private bool juegoTerminado;

    private float posicionInicialZ;
    private float velocidadActual;

    public int Puntuacion { get; private set; }

    public bool JuegoTerminado => juegoTerminado;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        posicionInicialZ = transform.position.z;
        velocidadActual = velocidadAvance;

        mobileX = 0;
        mobileZ = 0;

        // Evitar que las partículas se reproduzcan al iniciar
        if (particulasSalto != null)
            particulasSalto.Stop();

        if (particulasAterrizaje != null)
            particulasAterrizaje.Stop();
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        Puntuacion = Mathf.Max(
            0,
            Mathf.FloorToInt(transform.position.z - posicionInicialZ)
        );

        if (puntosPorNivel > 0)
        {
            int nivel = Puntuacion / puntosPorNivel;

            velocidadActual = Mathf.Min(
                velocidadAvance + nivel * incrementoVelocidad,
                velocidadMaxima
            );
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Saltar();
        }
    }

    void FixedUpdate()
    {
        if (juegoTerminado || rb == null)
            return;

        float tecladoX = Input.GetAxisRaw("Horizontal");

        float movimientoX = tecladoX != 0
            ? tecladoX
            : mobileX;

        movimientoX = Mathf.Clamp(movimientoX, -1f, 1f);

        Vector3 nuevaVelocidad = new Vector3(
            movimientoX * velocidadLateral,
            rb.linearVelocity.y,
            velocidadActual
        );

        rb.linearVelocity = nuevaVelocidad;
    }

    public void Saltar()
    {
        if (puedeSaltar && !juegoTerminado && rb != null)
        {
            rb.AddForce(
                Vector3.up * fuerzaSalto,
                ForceMode.Impulse
            );

            // Reproducir partículas al saltar
            if (particulasSalto != null)
            {
                particulasSalto.Play();
            }

            puedeSaltar = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            puedeSaltar = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            puedeSaltar = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            "Colisión detectada con: " +
            collision.gameObject.name
        );

        // Detectar obstáculos
        if (collision.gameObject.CompareTag("obstaculo"))
        {
            Debug.Log("¡El jugador chocó con un obstáculo!");

            TerminarJuego();
            return;
        }

        // Detectar aterrizaje
        if (collision.gameObject.CompareTag("Suelo"))
        {
            if (particulasAterrizaje != null)
            {
                particulasAterrizaje.Play();
            }
        }
    }

    public void TerminarJuego()
    {
        if (juegoTerminado)
            return;

        juegoTerminado = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Detener los efectos visuales
        if (particulasSalto != null)
            particulasSalto.Stop();

        if (particulasAterrizaje != null)
            particulasAterrizaje.Stop();

        Debug.Log(
            "GAME OVER. Puntuación: " + Puntuacion
        );
    }

    public float ObtenerVelocidad()
    {
        return velocidadActual;
    }
}
