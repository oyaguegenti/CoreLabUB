using UnityEngine;

public class CustomAudioSource : MonoBehaviour
{
    // Clip to be played
    protected AudioClip audioClip;

    public virtual void PlayClip(AudioClip clip)
    {
        audioClip = clip;
        GetComponent<AudioSource>().clip = audioClip;

        GetComponent<AudioSource>().Play();
    }
}
