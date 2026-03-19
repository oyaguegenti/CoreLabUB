using System.Collections;
using UnityEngine;

public enum StickState { GetSample, PutSample }

public class Stick : RaycastInteractable
{
    [Header("Detection")]
    [SerializeField] private bool enableDetection = true;
    [SerializeField] private float rayDistance = 0.25f;
    [SerializeField] protected LayerMask substanceLayer;

    [Header("References")]
    [SerializeField] private Renderer stickHeadRenderer;
    [SerializeField] private AudioSource headAudio;

    [Header("Ray Origin Offset")]
    [SerializeField] private Vector3 headPosition = new Vector3(0f, 0f, 0.02f);

    private bool hasHit = false;
    private StickState stickState = StickState.GetSample;

    private RaycastTarget previousTarget;
    private BaseSubstance substance;

    private SubstanceType currentSubstanceType = SubstanceType.NULL;
    private Material currentSubstanceMaterial;

    protected override void Awake()
    {
        base.Awake();
        interactableType = InteractableType.Stick;

        if (stickHeadRenderer == null)
        {
            Debug.LogWarning("[Stick] Stick head Renderer is not assigned in Inspector.");
        }

        if (headAudio != null)
        {
            headAudio.playOnAwake = false;
            if (headAudio.isPlaying)
            {
                headAudio.Stop();
            }
        }

        Debug.Log("Stick Awake");
    }

    public override void SelectEnter(GameObject hand)
    {
        base.SelectEnter(hand);
        Debug.Log("Stick grabbed by: " + hand.name);
    }

    public override IEnumerator Grab()
    {
        Debug.Log("Grab Coroutine STARTED");

        if (!enableDetection)
        {
            Debug.Log("Detection disabled");
            yield break;
        }

        while (isDragging)
        {
            Debug.Log("Raycasting...");

            Ray ray = new Ray(transform.position + headPosition, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance, substanceLayer))
            {
                Debug.Log("Raycast HIT: " + hit.transform.name);

                BaseSubstance substanceHit = hit.transform.GetComponentInChildren<BaseSubstance>();

                if (substanceHit == null)
                {
                    Debug.LogWarning("Hit object has NO BaseSubstance component");
                    yield return null;
                    continue;
                }

                RaycastTarget targetHit = substanceHit;

                if (previousTarget == null)
                {
                    Debug.Log("Initial target detected");
                    previousTarget = targetHit;
                }

                if (previousTarget.GetId() != targetHit.GetId())
                {
                    Debug.Log("Target changed");
                    previousTarget.OnRaycastExit(gameObject);
                    previousTarget = targetHit;
                }

                Debug.Log("Calling OnRaycastEnter on " + targetHit.name);
                targetHit.OnRaycastEnter(gameObject);

                hasHit = true;
            }
            else
            {
                Debug.Log("Raycast missed");

                if (previousTarget != null)
                {
                    Debug.Log("Calling OnRaycastExit on previous target");
                    previousTarget.OnRaycastExit(gameObject);
                    previousTarget = null;
                }
            }

            yield return null;
        }

        Debug.Log("Grab Coroutine ENDED");
    }

    public override void SelectExit(GameObject hand)
    {
        base.SelectExit(hand);

        Debug.Log("Stick released");

        if (!hasHit)
        {
            return;
        }

        if (previousTarget != null)
        {
            previousTarget.OnRaycastExit(gameObject);
        }

        if (headAudio != null)
        {
            headAudio.Pause();
        }

        hasHit = false;
    }

    public void ChangeHead(Material material)
    {
        Debug.Log("Changing head material");

        if (stickHeadRenderer == null)
        {
            Debug.LogWarning("[Stick] Cannot change head material because stickHeadRenderer is null.");
            return;
        }

        if (material == null)
        {
            Debug.LogWarning("[Stick] Cannot change head material because provided material is null.");
            return;
        }

        stickHeadRenderer.material = material;
    }

    public void SetSubstance(BaseSubstance substanceFound)
    {
        if (substanceFound == null)
        {
            Debug.LogWarning("Trying to set NULL substance on stick.");
            return;
        }

        Debug.Log("Substance set: " + substanceFound.name);

        substance = substanceFound;
        currentSubstanceType = substanceFound.GetSubstanceType();
        currentSubstanceMaterial = substanceFound.GetSubstanceMaterial();

        stickState = StickState.PutSample;
    }

    public BaseSubstance GetSubstance()
    {
        return substance;
    }

    public AudioSource GetHeadAudioSource()
    {
        return headAudio;
    }

    public StickState GetState()
    {
        return stickState;
    }

    public Material GetSubstanceMaterial()
    {
        return currentSubstanceMaterial;
    }

    public SubstanceType GetSubstanceType()
    {
        return currentSubstanceType;
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position + headPosition, transform.forward);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(ray);
    }

    public void OnDestroy()
    {
        Debug.Log("Stick destroyed");

        if (!hasHit)
        {
            return;
        }

        if (previousTarget != null)
        {
            previousTarget.OnRaycastExit(gameObject);
        }

        if (headAudio != null)
        {
            headAudio.Pause();
        }

        hasHit = false;
    }
}