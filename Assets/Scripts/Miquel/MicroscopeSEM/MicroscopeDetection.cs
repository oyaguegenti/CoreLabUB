using UnityEngine;

public class MicroscopeDetection : MonoBehaviour
{
    [SerializeField] private bool activateDrawing;
    [SerializeField] private GameObject startScanButton;

    [Header("DEBUG")]
    [SerializeField] private bool debugForceDetection = false;
    [SerializeField] private SubstanceType debugSubstanceType = SubstanceType.Pollen;
    [SerializeField] private GameObject debugSampleObject;

    private SubstanceType sampleHit = SubstanceType.NULL;
    private MinigameSEM minigameSEM;

    private void Awake()
    {
        if (startScanButton != null)
        {
            minigameSEM = startScanButton.transform.parent.GetComponent<MinigameSEM>();
            startScanButton.SetActive(false);
        }
    }

    public void Detect()
    {
        if (startScanButton == null)
        {
            Debug.LogWarning("StartScanButton is missing.");
            return;
        }

        if (minigameSEM == null)
        {
            Debug.LogWarning("MinigameSEM reference not found.");
            return;
        }

        // BLOQUEO: si ya hay un minijuego en curso, no volver a detectar
        if (minigameSEM.IsMinigameRunning())
        {
            Debug.Log("SEM minigame already running. Detection ignored.");
            return;
        }

        if (debugForceDetection)
        {
            ActivateMinigame(debugSubstanceType, debugSampleObject);
            return;
        }

        startScanButton.SetActive(false);
        sampleHit = SubstanceType.NULL;

        RaycastHit hit;
        Vector3 offset = new Vector3(0f, 0f, 0.18f);

        if (Physics.BoxCast(
            transform.position - offset,
            transform.lossyScale,
            transform.forward,
            out hit,
            transform.rotation,
            0.1f,
            LayerMask.GetMask("SubstanceInteractable")))
        {
            SubstanceSample sample = hit.transform.GetComponent<SubstanceSample>();

            if (sample == null)
            {
                sample = hit.transform.GetComponentInParent<SubstanceSample>();
            }

            if (sample == null)
            {
                Debug.Log("Detected object has no SubstanceSample.");
                return;
            }

            if (!sample.HasSubstance())
            {
                Debug.Log("Sample detected, but it has no substance.");
                return;
            }

            sampleHit = sample.GetSubstanceType();

            if (sampleHit == SubstanceType.NULL)
            {
                Debug.Log("Sample substance type is NULL.");
                return;
            }

            ActivateMinigame(sampleHit, sample.gameObject);
        }
        else
        {
            Debug.Log("No sample detected inside SEM.");
        }
    }

    private void ActivateMinigame(SubstanceType detectedSubstance, GameObject sampleObject)
    {
        startScanButton.SetActive(true);

        minigameSEM.SetSubstanceType(detectedSubstance);
        minigameSEM.SetSampleMicroscope(sampleObject);
        minigameSEM.SetMinigameRunning(true);

        Debug.Log("SEM ready. Detected substance: " + detectedSubstance);
    }

    private void OnDrawGizmos()
    {
        if (!activateDrawing) return;

        RaycastHit hit;
        Vector3 offset = new Vector3(0f, 0f, 0.18f);

        if (Physics.BoxCast(
            transform.position - offset,
            transform.lossyScale,
            transform.forward,
            out hit,
            transform.rotation,
            0.1f,
            LayerMask.GetMask("SubstanceInteractable")))
        {
            Gizmos.DrawWireCube(hit.transform.position, transform.lossyScale);
        }
    }
}