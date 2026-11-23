
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadAvance = 5f;
    public float velocidadLateral = 5f;
    public float fuerzaSalto = 5f;
    public float limiteLateral = 3f;

    [Header("Dificultad progresiva")]
    public float incrementoVelocidad = 0.5f;
    public int puntosPorNivel = 10;
    public float velocidadMaxima = 12f;

    [Header("Efectos")]
    public ParticleSystem particulasSalto;
    public ParticleSystem particulasAterrizaje;
    public TrailRenderer estela;

    [Header("Detección del suelo")]
    public float radioDeteccionSuelo = 0.2f;
    public float distanciaDesdePies = 0.08f;

    [Header("Estado del juego")]
    public bool JuegoTerminado => juegoTerminado;
    public int Puntuacion { get; private set; }

    // Controles táctiles:
    // -1 = izquierda
    //  0 = quieto
    //  1 = derecha
    public static float mobileX;

    private Rigidbody rb;
    private Collider colliderJugador;

    private bool puedeSaltar;
    private bool juegoTerminado;
    private bool estabaEnSuelo;

    private float posicionInicialZ;
    private float velocidadActual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        colliderJugador = GetComponent<Collider>();

        posicionInicialZ = transform.position.z;
        velocidadActual = velocidadAvance;

        mobileX = 0f;
        Puntuacion = 0;

        juegoTerminado = false;

        // Permitir el primer salto.
        puedeSaltar = true;

        // Detectar si el personaje comienza sobre el suelo.
        estabaEnSuelo = EstaEnSuelo();

        if (estabaEnSuelo)
            puedeSaltar = true;

        if (estela != null)
            estela.emitting = true;
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        LeerTeclado();

        ActualizarPuntuacion();
        ActualizarDificultad();

        DetectarAterrizaje();
    }

    void FixedUpdate()
    {
        if (juegoTerminado)
            return;

        float movimientoX = mobileX;

        // Controles de teclado para PC.
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed ||
                Keyboard.current.aKey.isPressed)
            {
                movimientoX = -1f;
            }
            else if (Keyboard.current.rightArrowKey.isPressed ||
                     Keyboard.current.dKey.isPressed)
            {
                movimientoX = 1f;
            }
        }

        Vector3 velocidad = rb.linearVelocity;

        // Movimiento lateral.
        velocidad.x = movimientoX * velocidadLateral;

        // Avance automático.
        velocidad.z = velocidadActual;

        rb.linearVelocity = velocidad;

        LimitarMovimiento();
    }

    // Leer el teclado.
    void LeerTeclado()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Saltar();
        }
    }

    // Saltar desde teclado o botón táctil.
    public void Saltar()
    {
        if (juegoTerminado || !puedeSaltar)
            return;

        puedeSaltar = false;

        // Eliminar velocidad vertical anterior.
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        // Aplicar fuerza de salto.
        rb.AddForce(
            Vector3.up * fuerzaSalto,
            ForceMode.Impulse
        );

        if (particulasSalto != null)
            particulasSalto.Play();
    }

    // Detectar si el jugador está sobre una superficie.
    bool EstaEnSuelo()
    {
        if (colliderJugador == null)
            return false;

        Vector3 puntoPies = new Vector3(
            colliderJugador.bounds.center.x,
            colliderJugador.bounds.min.y + distanciaDesdePies,
            colliderJugador.bounds.center.z
        );

        Collider[] colisiones = Physics.OverlapSphere(
            puntoPies,
            radioDeteccionSuelo
        );

        foreach (Collider colision in colisiones)
        {
            if (colision == colliderJugador)
                continue;

            // Ignorar los colliders hijos del jugador.
            if (colision.transform.IsChildOf(transform))
                continue;

            if (colision.isTrigger)
                continue;

            return true;
        }

        return false;
    }

    // Detectar aterrizaje y habilitar otro salto.
    void DetectarAterrizaje()
    {
        bool enSuelo = EstaEnSuelo();

        if (enSuelo)
        {
            puedeSaltar = true;

            // Ejecutar partículas al aterrizar.
            if (!estabaEnSuelo &&
                particulasAterrizaje != null)
            {
                particulasAterrizaje.Play();
            }
        }

        estabaEnSuelo = enSuelo;
    }

    // Limitar el movimiento lateral.
    void LimitarMovimiento()
    {
        Vector3 posicion = rb.position;

        posicion.x = Mathf.Clamp(
            posicion.x,
            -limiteLateral,
            limiteLateral
        );

        rb.position = posicion;
    }

    // Actualizar puntuación según la distancia recorrida.
    void ActualizarPuntuacion()
    {
        Puntuacion = Mathf.Max(
            0,
            Mathf.FloorToInt(
                transform.position.z - posicionInicialZ
            )
        );
    }

    // Aumentar velocidad progresivamente.
    void ActualizarDificultad()
    {
        int nivel = 0;

        if (puntosPorNivel > 0)
            nivel = Puntuacion / puntosPorNivel;

        velocidadActual = Mathf.Min(
            velocidadAvance +
            (nivel * incrementoVelocidad),
            velocidadMaxima
        );
    }

    // Detectar colisiones con obstáculos.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstaculo"))
        {
            TerminarJuego();
        }
    }

    // Finalizar partida.
    public void TerminarJuego()
    {
        if (juegoTerminado)
            return;

        juegoTerminado = true;
        mobileX = 0f;

        rb.linearVelocity = Vector3.zero;

        if (estela != null)
            estela.emitting = false;
    }

    // Limpiar el control táctil al desactivar el jugador.
    private void OnDisable()
    {
        mobileX = 0f;
    }

    // Mostrar visualmente la detección del suelo en Scene.
    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();

        if (col == null)
            return;

        Vector3 puntoPies = new Vector3(
            col.bounds.center.x,
            col.bounds.min.y + distanciaDesdePies,
            col.bounds.center.z
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            puntoPies,
            radioDeteccionSuelo
        );
    }
}
