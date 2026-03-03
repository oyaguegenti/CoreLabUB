using UnityEngine;

public enum HandState { IDLE = 0, POINTING = 1, GRABBING = 2, INTERACT = 3 }
public class Hand : MonoBehaviour
{
    private HandState currentHandState;
    [SerializeField] private Animator handAnimation;
    [SerializeField] private string handName;

    // This Changes the hands' animations. From Idle To Pointing, and from Pointing to Interact or Grabbing
    public virtual void ChangeHandState(HandState newHandState)
    {
        // If the hand state is the same as the previous hand, return
        if (currentHandState == newHandState) { return; }

        switch (newHandState)
        {
            case HandState.IDLE:
            {
                handAnimation.SetBool(handName + "IndexPoint", false);
                handAnimation.SetBool(handName + "IndexInteract", false);
                handAnimation.SetBool(handName + "HandGrab", false);
                break;
            }
            case HandState.POINTING:
            {
                handAnimation.SetBool(handName + "IndexPoint", true);
                handAnimation.SetBool(handName + "IndexInteract", false);
                handAnimation.SetBool(handName + "HandGrab", false);
                break;
            }
            case HandState.GRABBING:
            {

                handAnimation.SetBool(handName + "IndexPoint", true);
                handAnimation.SetBool(handName + "IndexInteract", false);
                handAnimation.SetBool(handName + "HandGrab", true);
                break;
            }
            case HandState.INTERACT:
            {
                handAnimation.SetBool(handName + "IndexPoint", true);
                handAnimation.SetBool(handName + "IndexInteract", true);
                handAnimation.SetBool(handName + "HandGrab", false);
                break;
            }
        }
        currentHandState = newHandState;
    }
}
