using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadAvance = 5f;
    public float velocidadLateral = 5f;
    public float fuerzaSalto = 7f;

    [Header("Limites de la pista")]
    public float limiteIzquierdo = -3.5f;
    public float limiteDerecho = 3.5f;

    [Header("Deteccion del suelo")]
    public LayerMask capaSuelo = ~0;
    public float distanciaDeteccionSuelo = 0.15f;

    [Header("Dificultad progresiva")]
    public float incrementoVelocidad = 0.5f;
    public int puntosPorNivel = 10;
    public float velocidadMaxima = 12f;

    [Header("Efectos visuales")]
    public ParticleSystem particulasSalto;
    public ParticleSystem particulasAterrizaje;

    [Header("Configuracion de estela")]
    public TrailRenderer estela;
    public float grosorEstela = 0.035f;
    public float duracionEstela = 0.20f;

    // Controles moviles
    public static float mobileX;
    public static float mobileZ;

    private Rigidbody rb;
    private CapsuleCollider capsule;

    private bool puedeSaltar;
    private bool estabaEnSuelo;
    private bool juegoTerminado;

    private float posicionInicialZ;
    private float velocidadActual;

    public int Puntuacion { get; private set; }
    public bool JuegoTerminado => juegoTerminado;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        posicionInicialZ = transform.position.z;
        velocidadActual = velocidadAvance;

        mobileX = 0;
        mobileZ = 0;

        // Configuracion de fisica
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.constraints =
            RigidbodyConstraints.FreezeRotation;

        // Detener particulas al iniciar
        if (particulasSalto != null)
            particulasSalto.Stop();

        if (particulasAterrizaje != null)
            particulasAterrizaje.Stop();

        // Configurar estela
        if (estela != null)
        {
            estela.widthMultiplier = grosorEstela;

            estela.widthCurve = new AnimationCurve(
                new Keyframe(0f, 0.5f),
                new Keyframe(1f, 0f)
            );

            estela.time = duracionEstela;
            estela.minVertexDistance = 0.1f;

            estela.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.Off;

            estela.receiveShadows = false;
            estela.Clear();
        }
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        // Actualizar puntuacion
        Puntuacion = Mathf.Max(
            0,
            Mathf.FloorToInt(
                transform.position.z - posicionInicialZ
            )
        );

        // Dificultad progresiva
        if (puntosPorNivel > 0)
        {
            int nivel = Puntuacion / puntosPorNivel;

            velocidadActual = Mathf.Min(
                velocidadAvance +
                nivel * incrementoVelocidad,
                velocidadMaxima
            );
        }

        // Salto mediante teclado
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Saltar();
        }
    }

    void FixedUpdate()
    {
        if (juegoTerminado || rb == null || rb.isKinematic)
            return;

        // Comprobar si el jugador esta sobre el suelo
        ComprobarSuelo();

        // Movimiento lateral
        float tecladoX = Input.GetAxisRaw("Horizontal");

        float movimientoX = tecladoX != 0
            ? tecladoX
            : mobileX;

        movimientoX = Mathf.Clamp(movimientoX, -1f, 1f);

        // Mantener avance automatico y velocidad vertical
        Vector3 nuevaVelocidad = new Vector3(
            movimientoX * velocidadLateral,
            rb.linearVelocity.y,
            velocidadActual
        );

        // Limites laterales
        Vector3 posicion = rb.position;

        if (posicion.x <= limiteIzquierdo)
        {
            posicion.x = limiteIzquierdo;

            if (nuevaVelocidad.x < 0)
                nuevaVelocidad.x = 0;
        }

        if (posicion.x >= limiteDerecho)
        {
            posicion.x = limiteDerecho;

            if (nuevaVelocidad.x > 0)
                nuevaVelocidad.x = 0;
        }

        rb.position = posicion;
        rb.linearVelocity = nuevaVelocidad;
    }

    // Comprobar si el jugador esta tocando el suelo
    private void ComprobarSuelo()
    {
        Bounds limites = capsule.bounds;

        float radio = Mathf.Max(
            0.05f,
            Mathf.Min(limites.extents.x, limites.extents.z) * 0.8f
        );

        Vector3 origen = new Vector3(
            limites.center.x,
            limites.min.y + radio + 0.03f,
            limites.center.z
        );

        bool enSuelo = Physics.SphereCast(
            origen,
            radio,
            Vector3.down,
            out RaycastHit impacto,
            distanciaDeteccionSuelo + 0.03f,
            capaSuelo,
            QueryTriggerInteraction.Ignore
        );

        // No considerar los obstaculos como suelo
        if (enSuelo &&
            TieneTagEnJerarquia(impacto.collider.transform, "obstaculo"))
        {
            enSuelo = false;
        }

        puedeSaltar = enSuelo;

        // Detectar aterrizaje
        if (enSuelo && !estabaEnSuelo)
        {
            if (particulasAterrizaje != null)
                particulasAterrizaje.Play();
        }

        estabaEnSuelo = enSuelo;
    }

    // Ejecutar salto
    public void Saltar()
    {
        if (!puedeSaltar ||
            juegoTerminado ||
            rb == null ||
            rb.isKinematic)
        {
            return;
        }

        puedeSaltar = false;
        estabaEnSuelo = false;

        // Reiniciar velocidad vertical
        Vector3 velocidad = rb.linearVelocity;
        velocidad.y = 0;
        rb.linearVelocity = velocidad;

        // Aplicar impulso
        rb.AddForce(
            Vector3.up * fuerzaSalto,
            ForceMode.Impulse
        );

        if (particulasSalto != null)
            particulasSalto.Play();

        Debug.Log("¡Jengibre esta saltando!");
    }

    // Comprobar Tag en el objeto o sus padres
    private bool TieneTagEnJerarquia(
        Transform objeto,
        string tag
    )
    {
        while (objeto != null)
        {
            if (objeto.CompareTag(tag))
                return true;

            objeto = objeto.parent;
        }

        return false;
    }

    // Detectar colisiones
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            "Colision detectada con: " +
            collision.collider.name
        );

        // Detectar obstaculos
        if (TieneTagEnJerarquia(
            collision.collider.transform,
            "obstaculo"))
        {
            Debug.Log("¡Jengibre choco con un obstaculo!");

            TerminarJuego();
        }
    }

    // Terminar partida
    public void TerminarJuego()
    {
        if (juegoTerminado)
            return;

        juegoTerminado = true;
        puedeSaltar = false;
        estabaEnSuelo = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (particulasSalto != null)
            particulasSalto.Stop();

        if (particulasAterrizaje != null)
            particulasAterrizaje.Stop();

        if (estela != null)
            estela.Clear();

        Debug.Log(
            "GAME OVER. Puntuacion: " + Puntuacion
        );
    }

    public float ObtenerVelocidad()
    {
        return velocidadActual;
    }
}