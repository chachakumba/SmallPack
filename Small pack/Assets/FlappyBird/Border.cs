using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField]
    AudioClip bump;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Bird>() != null)
        {
            audioSource.clip = bump;
            audioSource.Play();
            BirdManager.instance.GameOver();
        }
    }
}
