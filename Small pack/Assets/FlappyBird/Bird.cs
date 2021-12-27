using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Bird : MonoBehaviour
{
    public float flyPower = 250;
    [Space]
    Rigidbody2D rb;
    public delegate void FlyInput();
    public event FlyInput OnButtonPress;
    public static Bird instance;
    [SerializeField]
    Transform sprite;
    bool startedPlaying = false;
    [SerializeField]
    GameObject pressToPlayText;
    [SerializeField]
    AudioClip flap;
    AudioSource audioSource;
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        OnButtonPress += Fly;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) OnButtonPress.Invoke();
        if (Time.timeScale > 0)
        {
            float AngleRad = Mathf.Atan2(rb.velocity.y, 3);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            sprite.rotation = Quaternion.Euler(0, 0, AngleDeg);
        }
    }
    public void OnFly()
    {
        OnButtonPress.Invoke();
    }
    void Fly()
    {
        if (!startedPlaying)
        {
            startedPlaying = true;
            pressToPlayText.SetActive(false);
            rb.gravityScale = 1;
            StartCoroutine(BirdManager.instance.SpawnPipes());
        }
        if (Time.timeScale > 0)
        {
            audioSource.clip = flap;
            audioSource.Play();
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector3.up * flyPower);
        }
    }
}
