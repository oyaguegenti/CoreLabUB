using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum InteractableType { NULL, MagneticCard, Stick }
public class RaycastInteractable : MonoBehaviour
{
    protected InteractableType interactableType = InteractableType.NULL;
    
    protected bool canDrag = true;
    protected bool isDragging = true;

    protected virtual void Awake()
    {
        gameObject.layer = 9; // RaycastInteractable

        // Events Triggers
        #region XR GRAB INTERACTABLE EVENTS
        GetComponent<XRGrabInteractable>().hoverEntered.AddListener(_ =>
        {
            HoverEnter(_.interactorObject.transform.gameObject);
        });

        GetComponent<XRGrabInteractable>().hoverExited.AddListener(_ =>
        {
            HoverExit(_.interactorObject.transform.gameObject);
        });

        GetComponent<XRGrabInteractable>().selectEntered.AddListener(_ =>
        {
            StopAllCoroutines();

            SelectEnter(_.interactorObject.transform.gameObject);

            StartCoroutine(Grab());
        });

        GetComponent<XRGrabInteractable>().selectExited.AddListener(_ =>
        {
            StopAllCoroutines();

            SelectExit(_.interactorObject.transform.gameObject);
        });
        #endregion
    }

    public virtual void Activate(GameObject interactor) // BOTH Triggers Down
    {
        ChangeHandState(interactor, HandState.INTERACT);
    }

    public virtual void Deactivate(GameObject interactor) // BOTH Triggers Up
    {
        ChangeHandState(interactor, HandState.IDLE);
    }

    public virtual void HoverEnter(GameObject interactor)
    {
        ChangeHandState(interactor, HandState.POINTING);
    }

    public virtual void HoverExit(GameObject interactor)
    {
        ChangeHandState(interactor, HandState.IDLE);
    }

    public virtual void SelectEnter(GameObject interactor) // Hand Trigger Down
    {
        if (canDrag) { isDragging = true; }
        Debug.Log(interactor.name);
        interactor.transform.parent.GetChild(4).gameObject.SetActive(false);
        interactor.GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    public virtual IEnumerator Grab()
    {
        yield return null;
    }

    public virtual void SelectExit(GameObject interactor)
    {
        if (canDrag) { isDragging = false; }

        interactor.transform.parent.GetChild(4).gameObject.SetActive(true);
        interactor.GetComponent<XRInteractorLineVisual>().enabled = true;
    }

    public void ChangeHandState(GameObject interactor, HandState state)
    {
        try
        {
            interactor.transform.parent.GetComponent<Hand>().ChangeHandState(state);
        }
        catch
        {
            //Debug.Log(interactor.name);
        }
    }

    public InteractableType GetInteractableType()
    {
        return interactableType;
    }
}
