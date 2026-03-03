using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CardReader : MonoBehaviour
{
    // Event that notifies this when palyer is dragging a magnetic card or doors are open
    public static UnityEvent<bool> toggleCardReaderRaycast = new();
    public static UnityEvent<CardPerms, bool> toggleSingleCardReaderRaycast = new();

    // Magnetic card being dragged -> true, Magnetic card not being dragged -> false
    private bool raycastsActive = false;

    // Raycast Interactable layermask
    [SerializeField] private LayerMask interactableLayerMask;

    // Perms to open the door
    [SerializeField] private CardPerms labPerm;

    // Door
    [SerializeField] private DoorLab door;

    // Raycasts variables
    private Vector3 raycastOffsets;
    private Ray rayTop;
    private Ray rayBot;
    private float rayDistance = 0.05f;

    private int numBlinks = 10;

    private void Awake()
    {
        // This activates or deactivates the raycasts for all CardReaders. This is a performance optimization
        toggleCardReaderRaycast.AddListener((bool state) => 
        {
            raycastsActive = state;
        });
        // This does the same as the previous one but to a single CardReader
        toggleSingleCardReaderRaycast.AddListener((CardPerms cardPerms, bool state) =>
        {
            if (cardPerms != labPerm || raycastsActive)
            { return; }

            raycastsActive = state;
        });

        // Adds or substracts 1/4 of this gameObject size to the position to space out the raycast for better precision
        raycastOffsets = new Vector3(0,transform.localScale.y/4, 0);

        rayTop = new Ray(transform.position + raycastOffsets, transform.forward);
        rayBot = new Ray(transform.position - raycastOffsets, transform.forward);
    }
    private void OnDrawGizmos()
    {
        rayTop = new Ray(transform.position + raycastOffsets, transform.forward);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(rayTop);
    }
    private void Update()
    {
        if (!raycastsActive) { return; }

        RaycastHit hitTop;
        RaycastHit hitBot;

        // Both raycasts must collide with the card to continue
        if (!Physics.Raycast(rayTop, out hitTop, rayDistance , interactableLayerMask))
        { return; }

        if (!Physics.Raycast(rayBot, out hitBot, rayDistance, interactableLayerMask))
        { return; }

        GameObject detectedObject = hitTop.transform.gameObject;

        // Check if the Interactable Object is a card
        if (detectedObject.GetComponent<RaycastInteractable>().GetInteractableType() == InteractableType.MagneticCard)
        {
            // Check if the card has this lab permissions
            if (detectedObject.GetComponent<MagneticCard>().CheckPerm(labPerm))
            {
                // Play Confirmation Sound, Card Reader Panel Blinks Green Light

                StartCoroutine(BlinkConfirmation());

                AudioManager.Instance.PlaySoundAt("CardReaderConfirmation", transform.position);

                door.SetDoor(true);
            }
            else
            {
                // Play Errop Sound, Card Reader Panel Blinks Red Light

                StartCoroutine(BlinkError());

                AudioManager.Instance.PlaySoundAt("CardReaderError", transform.position);
                
            }

            raycastsActive = false;
        }
    }

    private IEnumerator BlinkConfirmation()
    {
        int counter = 0;
        while (counter <= numBlinks)
        {
            ChangeEmissionColor(Color.green);
            yield return new WaitForSeconds(0.05f);
            ChangeEmissionColor(Color.white);
            yield return new WaitForSeconds(0.05f);
            counter++;
        }
        yield return null;
    }
    
    private IEnumerator BlinkError()
    {
        int counter = 0;
        while (counter <= numBlinks)
        {
            ChangeEmissionColor(Color.red);
            yield return new WaitForSeconds(0.08f);
            ChangeEmissionColor(Color.white);
            yield return new WaitForSeconds(0.08f);
            counter++;
        }
        yield return null;

        raycastsActive = true;
    }

    // This function changes the icon's color on the CardReader
    private void ChangeEmissionColor(Color color)
    {
        transform.parent.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
    }
}
