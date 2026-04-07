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

    private int playerLayer;

    private void Start()
    {
        panelUI.SetActive(false);
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            panelUI.SetActive(true);
            textoUI.text = mensaje;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            panelUI.SetActive(false);
        }
    }
}