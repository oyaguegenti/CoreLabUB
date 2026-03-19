using System.Collections.Generic;
using UnityEngine;

public class WorkAreaZone : MonoBehaviour
{
    [Header("Valid Objects (root objects)")]
    [SerializeField] private List<GameObject> validObjects = new List<GameObject>();

    [Header("Visual")]
    [SerializeField] private GameObject visualZone;

    [Header("Behaviour")]
    [SerializeField] private bool disableZoneAfterSuccess = true;

    [Header("Optional Next Step")]
    [SerializeField] private GameObject objectToActivateAfterSuccess;

    private Collider zoneCollider;
    private bool isCompleted = false;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCompleted)
        {
            return;
        }

        GameObject rootObject = other.transform.root.gameObject;

        for (int i = 0; i < validObjects.Count; i++)
        {
            if (validObjects[i] == rootObject)
            {
                CompleteZone();
                return;
            }
        }
    }

    private void CompleteZone()
    {
        isCompleted = true;

        // Ocultar visual rojo
        if (visualZone != null)
        {
            visualZone.SetActive(false);
        }

        // Desactivar collider si se quiere
        if (disableZoneAfterSuccess && zoneCollider != null)
        {
            zoneCollider.enabled = false;
        }

        // Activar siguiente objeto (opcional)
        if (objectToActivateAfterSuccess != null)
        {
            objectToActivateAfterSuccess.SetActive(true);
        }
    }
}