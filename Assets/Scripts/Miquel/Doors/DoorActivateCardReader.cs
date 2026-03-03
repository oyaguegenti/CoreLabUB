using UnityEngine;

public class DoorActivateCardReader : MonoBehaviour
{
    // This function is called when the door animation ends
    // Even if there is no reference in code, its used, check door animations' events
    public void ActivateCardReader()
    {
        CardReader.toggleCardReaderRaycast.Invoke(true);
        transform.parent.GetComponent<DoorLab>().SetDoor(false);
    }
}
