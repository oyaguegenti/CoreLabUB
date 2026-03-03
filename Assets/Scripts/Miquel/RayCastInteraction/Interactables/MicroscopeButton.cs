using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MicroscopeButton : RaycastInteractable
{
    [SerializeField] private DoorController microscopeDoor;
    [SerializeField] private MicroscopeDetection microscopeDetection;

    protected override void Awake()
    {
        gameObject.layer = 9; // RaycastInteractable

        GetComponent<XRSimpleInteractable>().activated.AddListener(_ =>
        {
            Activate(_.interactorObject.transform.gameObject);
        });

        GetComponent<XRSimpleInteractable>().deactivated.AddListener(_ =>
        {
            Deactivate(_.interactorObject.transform.gameObject);
        });
    }

    // Opens / Closes SEM Door
    public override void Activate(GameObject interactor)
    {
        microscopeDoor.ToggleDoor();

        if (GetDoorOpen() == false)
        {
            microscopeDetection.Detect();
        }
    }

    public bool GetDoorOpen()
    { return microscopeDoor.GetSemIsOpen(); }
}
