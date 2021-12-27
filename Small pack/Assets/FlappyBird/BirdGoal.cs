using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGoal : MonoBehaviour
{
    [SerializeField]
    AudioClip getScoreSound;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Bird>() != null)
        {
            audioSource.clip = getScoreSound;
            audioSource.Play();
            BirdManager.instance.AddScore();
        }
    }
}
