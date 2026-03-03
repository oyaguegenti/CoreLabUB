using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachableInteractor : MonoBehaviour
{
    private bool isOccupied;
    private AttachableObject attachableObject;

    private void Awake()
    {
        GetComponent<XRSocketInteractor>().selectEntered.AddListener(_ =>
        {
            if (!isOccupied) // MAYBE DELETE
            {
                _.interactableObject.transform.GetComponent<AttachableObject>().OnAttach(_.interactorObject.transform.GetChild(0));
                isOccupied = true;
            }
        });

        GetComponent<XRSocketInteractor>().selectExited.AddListener(_ =>
        {
            if (isOccupied)
            {
                _.interactableObject.transform.GetComponent<AttachableObject>().OnDisattach();
                isOccupied = false;
            }
        });
    }
}
