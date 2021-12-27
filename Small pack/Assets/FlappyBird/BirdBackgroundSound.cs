using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BirdBackgroundSound : MonoBehaviour
{
    public static BirdBackgroundSound instance;
    [SerializeField]
    AudioClip backgroundMusic;
    AudioSource audioSource;
    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }
    public void ReloadScene()
    {
        SceneManager.UnloadSceneAsync("Flappy Bird");
        SceneManager.LoadSceneAsync("Flappy Bird", LoadSceneMode.Additive);
    }
    private void OnDestroy()
    {
        instance = null;
    }
}
