using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    AudioSource source;
    public void Play(AudioClip clip)
    {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(gameObject, 1);
    }
}
