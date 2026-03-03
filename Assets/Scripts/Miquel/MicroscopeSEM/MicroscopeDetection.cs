using UnityEngine;

public class MicroscopeDetection : MonoBehaviour
{
    [SerializeField] private bool activateDrawing;
    [SerializeField] private GameObject startScanButton;

    // DEBUG
    [Header("DEBUG")]
    [SerializeField] private bool debugForceDetection = false;
    [SerializeField] private SubstanceType debugSubstanceType = SubstanceType.Pollen;
    [SerializeField] private GameObject debugSampleObject;

    private SubstanceType sampleHit;

    // Detects the interior of the SEM
    public void Detect()
    {
        // =============================
        // DEBUG FORCE DETECTION
        // =============================
        if (debugForceDetection)
        {
            startScanButton.SetActive(true);

            startScanButton.transform.parent
                .GetComponent<MinigameSEM>()
                .SetSubstanceType(debugSubstanceType);

            startScanButton.transform.parent
                .GetComponent<MinigameSEM>()
                .SetSampleMicroscope(debugSampleObject);

            return;
        }

        // =============================
        // NORMAL DETECTION
        // =============================
        RaycastHit hit;
        Vector3 offset = new Vector3(0, 0, 0.18f);

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

            if (sample != null)
            {
                if (sample.GetSubstanceType() == SubstanceType.Pollen)
                {
                    startScanButton.SetActive(true);

                    sampleHit = sample.GetSubstanceType();

                    startScanButton.transform.parent
                        .GetComponent<MinigameSEM>()
                        .SetSubstanceType(sampleHit);

                    startScanButton.transform.parent
                        .GetComponent<MinigameSEM>()
                        .SetSampleMicroscope(hit.transform.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!activateDrawing) return;

        RaycastHit hit;
        Vector3 offset = new Vector3(0, 0, 0.18f);

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
