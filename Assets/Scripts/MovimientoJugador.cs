
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
    public bool JuegoTerminado => juegoTerminado;
    // Controles móviles
    public static float mobileX;
    public static float mobileZ;

    private Rigidbody rb;
    private bool puedeSaltar;
    private bool juegoTerminado;

    private float posicionInicialZ;
    private float velocidadActual;

    public int Puntuacion { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        posicionInicialZ = transform.position.z;
        velocidadActual = velocidadAvance;

        mobileX = 0;
        mobileZ = 0;
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        // Actualizar puntuación según la distancia
        Puntuacion = Mathf.Max(
            0,
            Mathf.FloorToInt(
                transform.position.z - posicionInicialZ
            )
        );

        // Aumentar dificultad cada 10 puntos
        int nivel = Puntuacion / puntosPorNivel;

        velocidadActual = Mathf.Min(
            velocidadAvance + nivel * incrementoVelocidad,
            velocidadMaxima
        );

        // Salto mediante teclado
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
        if (puedeSaltar && !juegoTerminado)
        {
            rb.AddForce(
                Vector3.up * fuerzaSalto,
                ForceMode.Impulse
            );

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

    public void TerminarJuego()
    {
        juegoTerminado = true;

        rb.linearVelocity = Vector3.zero;

        Debug.Log("Juego terminado. Puntuación: " + Puntuacion);
    }

    public float ObtenerVelocidad()
    {
        return velocidadActual;
    }
    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Obstaculo"))
    {
        TerminarJuego();
    }
}
}
