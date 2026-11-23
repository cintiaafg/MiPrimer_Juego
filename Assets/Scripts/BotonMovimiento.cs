
using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMovimiento : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [Header("Dirección del movimiento")]
    [Range(-1f, 1f)]
    public float direccion = -1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        MovimientoJugador.mobileX = direccion;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DetenerMovimiento();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DetenerMovimiento();
    }

    private void DetenerMovimiento()
    {
        MovimientoJugador.mobileX = 0f;
    }

    private void OnDisable()
    {
        DetenerMovimiento();
    }
}
