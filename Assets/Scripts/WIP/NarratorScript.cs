using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorScript : MonoBehaviour
{
    public List<AudioClip> narratorCLips;
    private AudioSource audioSource;

    private int current_narration;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }

    public void ChangeClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public List<AudioClip> GetNarratorClips()
    {
        return narratorCLips;
    }

    public void PlayNextNarration()
    {
        if (current_narration < narratorCLips.Count - 1)
            current_narration++;

        ChangeClip(narratorCLips[current_narration]);
        Play();
    }
}