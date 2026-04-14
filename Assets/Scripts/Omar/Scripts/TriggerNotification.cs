using UnityEngine;
using TMPro;
using System.Collections;

public class TriggerNotificacion : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelUI;
    public TextMeshProUGUI textoUI;

    [Header("Mensaje")]
    [TextArea]
    public string mensaje;

    [Header("Configuración")]
    public float duracion = 5f; // tiempo visible
    public bool soloUnaVez = true; // si se desactiva para siempre

    private int playerLayer;
    private bool yaActivado = false;
    private Coroutine rutinaActual;

    private void Start()
    {
        panelUI.SetActive(false);
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            // Si es solo una vez y ya se activó → no hacer nada
            if (soloUnaVez && yaActivado) return;

            yaActivado = true;

            // Activar UI
            panelUI.SetActive(true);
            textoUI.text = mensaje;

            // Reiniciar coroutine si ya había una
            if (rutinaActual != null)
                StopCoroutine(rutinaActual);

            rutinaActual = StartCoroutine(DesactivarTrasTiempo());
        }
    }

    private IEnumerator DesactivarTrasTiempo()
    {
        yield return new WaitForSeconds(duracion);

        panelUI.SetActive(false);

        // Si es solo una vez, desactivamos el trigger completamente
        if (soloUnaVez)
        {
            gameObject.SetActive(false);
        }
    }
}