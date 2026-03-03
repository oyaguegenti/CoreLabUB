using UnityEngine;

public class SubstanceObject : RaycastTarget
{
    public override void OnRaycastEnter(GameObject emitter)
    {
        // Plays rubbing SFX
        if (emitter.GetComponent<Stick>().GetState() == StickState.GetSample)
            emitter.GetComponent<Stick>().GetHeadAudioSource().UnPause();
    }

    public override void OnRaycastExit(GameObject emitter)
    {
        // Stops rubbing SFX
        if (emitter.GetComponent<Stick>().GetState() == StickState.GetSample)
            emitter.GetComponent<Stick>().GetHeadAudioSource().Pause();
    }
}
