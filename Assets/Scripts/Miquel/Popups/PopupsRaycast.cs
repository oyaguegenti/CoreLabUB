using UnityEngine;

public class PopupsRaycast : MonoBehaviour
{
    [SerializeField] protected int popupId;

    protected Ray ray;

    [SerializeField] protected float rayDistance;

    [SerializeField] protected LayerMask interactableLayerMask;

    [SerializeField] bool destroyOnTrigger = true;

    private void Start()
    {
        ray = new Ray(transform.position, transform.forward);
    }

    private void OnDrawGizmos()
    {
        ray = new Ray(transform.position, transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, transform.forward * rayDistance);
    }

    protected void DestroyOnTrigger()
    {
        if (destroyOnTrigger)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
