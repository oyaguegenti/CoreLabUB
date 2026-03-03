using UnityEngine;

public class DoorLab : MonoBehaviour
{
    [SerializeField] private Animator leftDoorAnimator;
    [SerializeField] private Animator rightDoorAnimator;

    private void Awake()
    {
        // After Animation, a Animation Event is triggered calling "toggleSingleCardReaderRaycast" CardReader event

        leftDoorAnimator.SetBool("isLeftDoor", true);
        rightDoorAnimator.SetBool("isLeftDoor", false);
    }

    public void SetDoor(bool state)
    {
        leftDoorAnimator.SetBool("StartDoorAnimation", state);
        rightDoorAnimator.SetBool("StartDoorAnimation", state);
    }
}
