using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachableObject : RaycastInteractable
{
    private Transform originPos;
    private GameObject attachTransform;

    private bool lockAttached = false;

    protected bool isAttatch = false;

    private void Start()
    {
        attachTransform = transform.GetChild(0).gameObject;
        originPos = attachTransform.transform;
    }

    public override void HoverEnter(GameObject interactor)
    {
        base.HoverEnter(interactor);
    }

    public override void HoverExit(GameObject interactor)
    {
        base.HoverExit(interactor);
    }

    public virtual void OnAttach(Transform attachPosition)
    {
        lockAttached = true;

        // Changes MovementType to seem the card is attached
        GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        GetComponent<XRGrabInteractable>().smoothPosition = false;

        isAttatch = true;
        originPos = attachPosition;

        ReturnToPreviousPosition();

        lockAttached = false;
    }

    public virtual void OnDisattach()
    {
        // Changes MovementType to simulate physics
        GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.VelocityTracking;
        GetComponent<XRGrabInteractable>().smoothPosition = true;

        isAttatch = false;
    }

    public override void SelectExit(GameObject interactor)
    {
        if (lockAttached == false) { return; }

        ReturnToPreviousPosition();
    }

    protected void ReturnToPreviousPosition()
    {
        transform.position = originPos.position;
    }
}
