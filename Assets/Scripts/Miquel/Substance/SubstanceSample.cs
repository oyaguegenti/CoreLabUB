using UnityEngine;

public class SubstanceSample : RaycastTarget
{
    [Header("Sample Visual Objects")]
    [SerializeField] private Renderer[] sampleRenderers;

    [Header("Material To Apply (Inspector)")]
    [SerializeField] private Material substanceMaterial;

    [SerializeField] private bool hasSubstance = false;
    [SerializeField] private SubstanceType substanceType;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnRaycastEnter(GameObject emitter)
    {
        TryApplyFromEmitter(emitter);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER DETECTED: " + other.name);
        TryApplyFromEmitter(other.gameObject);
    }

    private void TryApplyFromEmitter(GameObject emitter)
    {
        Debug.Log("Emitter detected: " + emitter.name);

        Stick stick = emitter.GetComponent<Stick>();
        if (stick == null)
        {
            Debug.Log("No Stick component found");
            return;
        }

        Debug.Log("Stick state: " + stick.GetState());

        if (hasSubstance)
        {
            Debug.Log("Already has substance");
            return;
        }

        if (stick.GetState() == StickState.GetSample)
        {
            Debug.Log("Stick has no sample");
            return;
        }

        ApplySubstance();
    }

    private void ApplySubstance()
    {
        Debug.Log("Applying substance...");

        if (sampleRenderers == null || sampleRenderers.Length == 0)
        {
            Debug.LogWarning("No renderers assigned.");
            return;
        }

        if (substanceMaterial == null)
        {
            Debug.LogWarning("No material assigned.");
            return;
        }

        for (int i = 0; i < sampleRenderers.Length; i++)
        {
            if (sampleRenderers[i] != null)
            {
                sampleRenderers[i].sharedMaterial = substanceMaterial;
            }
        }

        hasSubstance = true;
    }

    public SubstanceType GetSubstanceType()
    {
        return substanceType;
    }

    public bool HasSubstance()
    {
        return hasSubstance;
    }
}
