using System.Collections;
using UnityEngine;

public enum StickState { GetSample, PutSample }

public class Stick : RaycastInteractable
{
    private bool enableDetection = true;
    private bool hasHit = false;

    private StickState stickState = StickState.GetSample;

    private RaycastTarget previousTarget;
    private BaseSubstance substance;

    private Vector3 headPosition = new Vector3(0, 0, 0.02f);
    [SerializeField] private float rayDistance = 0.25f;

    [SerializeField] protected LayerMask substanceLayer;

    private AudioSource headAudio;

    protected override void Awake()
    {
        base.Awake();
        interactableType = InteractableType.Stick;

        headAudio = transform.GetChild(1).gameObject.GetComponent<AudioSource>();

        headAudio.Play();
        headAudio.Pause();

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
            yield return null;
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

        if (!hasHit) return;

        if (previousTarget != null)
        {
            previousTarget.OnRaycastExit(gameObject);
        }

        headAudio.Pause();
        hasHit = false;
    }

    public void ChangeHead(Material material)
    {
        Debug.Log("Changing head material");
        transform.GetChild(0).GetComponent<Renderer>().material = material;
    }

    public void SetSubstance(BaseSubstance substanceFound)
    {
        Debug.Log("Substance set: " + substanceFound.name);
        substance = substanceFound;
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
        if (substance == null)
        {
            Debug.LogWarning("Substance is NULL when requesting material");
            return null;
        }

        return substance.GetSubstanceMaterial();
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

        if (!hasHit) return;

        if (previousTarget != null)
        {
            previousTarget.OnRaycastExit(gameObject);
        }

        headAudio.Pause();
        hasHit = false;
    }
}