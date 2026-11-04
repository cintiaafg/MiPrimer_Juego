using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    private Rigidbody rb;
    private float horizontal;
    private float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No se encontró Rigidbody en Jugador");
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 movimiento = new Vector3(
            horizontal,
            0f,
            vertical
        ).normalized;

        rb.linearVelocity = new Vector3(
            movimiento.x * velocidad,
            rb.linearVelocity.y,
            movimiento.z * velocidad
        );
    }
}