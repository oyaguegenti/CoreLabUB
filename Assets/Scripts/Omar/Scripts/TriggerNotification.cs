using UnityEngine;
using TMPro;

public class TriggerNotificacion : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelUI;
    public TextMeshProUGUI textoUI;

    [Header("Mensaje")]
    [TextArea]
    public string mensaje;

    [Header("Configuración")]
    public bool soloUnaVez = true;

    private int playerLayer;
    private bool yaActivado = false;
    private bool mensajeActivo = false;

    private void Start()
    {
        panelUI.SetActive(false);
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (soloUnaVez && yaActivado) return;

            yaActivado = true;

            // Mostrar UI
            panelUI.SetActive(true);
            textoUI.text = mensaje;
            mensajeActivo = true;
        }
    }

    private void Update()
    {
        if (!mensajeActivo) return;

        // Botón A (Oculus / Quest suele ser "joystick button 0")
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            CerrarMensaje();
        }
    }

    private void CerrarMensaje()
    {
        panelUI.SetActive(false);
        mensajeActivo = false;

        if (soloUnaVez)
        {
            gameObject.SetActive(false);
        }
    }
}