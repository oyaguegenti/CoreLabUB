using UnityEngine;

public class SFXAudioSource : CustomAudioSource
{
    // Timer to delete this GameObject
    private float count = 0;

    private void Update()
    {
        if (audioClip == null)
        { return; }

        DestroyWhenClipFinishes();
    }

    private void DestroyWhenClipFinishes()
    {
        count += Time.deltaTime;
        if (count < audioClip.length)
        { return; }

        Destroy(gameObject);
    }
}
