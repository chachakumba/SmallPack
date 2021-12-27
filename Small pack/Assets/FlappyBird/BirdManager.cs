using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class BirdManager : MonoBehaviour
{
    public int score = 0;
    public float timeToNewPipe = 5;
    public GameObject pipePrefab;
    public Transform pipeSpawn;
    public Transform backgroundSpawn;
    public float pipeSpread = 3;
    public float pipeSpeed = 50;
    public float backSpeed = 1;
    public static BirdManager instance;
    Queue<GameObject> createdPipes = new Queue<GameObject>();
    [Space]
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI maxScoreText;
    [SerializeField]
    GameObject losePanel;
    [SerializeField]
    TextMeshProUGUI loseScoreText;

    [SerializeField]
    AudioClip dieSound;
    AudioSource audioSource;

    public GameObject backPrefab;
    Queue<GameObject> createdBacks = new Queue<GameObject>();
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        instance = this;
        StartCoroutine(MoveBackground());
        Menu.LoadSave();
        if (BirdBackgroundSound.instance == null)
        {
            Debug.Log("AddingMusic");
            SceneManager.LoadSceneAsync("BirdMusic", LoadSceneMode.Additive);
        }
    }
    IEnumerator MoveBackground()
    {
        createdBacks.Enqueue(Instantiate(backPrefab, Vector3.zero, Quaternion.identity));
        createdBacks.Enqueue(Instantiate(backPrefab, Vector3.right * 18, Quaternion.identity));
        while (true)
        {
            if(createdBacks.Peek().transform.position.x <= -12)
            {
                GameObject newBack;
                newBack = Instantiate(backPrefab, backgroundSpawn);
                newBack.transform.position = new Vector3(createdBacks.Peek().transform.position.x + 36, 0, 0);
                createdBacks.Enqueue(newBack);
                Destroy(createdBacks.Dequeue());
            }
            foreach(GameObject back in createdBacks)
            {
                back.transform.position -= Vector3.right * Time.deltaTime * backSpeed;
            }
            yield return null;
        }
    }
    public void GameOver()
    {
        StartCoroutine(PlayDeathSound());
        Time.timeScale = 0;
        losePanel.SetActive(true);
        loseScoreText.text = "Score: " + score;
        if (score > Menu.save.birdMaxScore)
        {
            maxScoreText.text = "New best!";
            Menu.SaveBird(score);
        }
        else maxScoreText.text = "Max score: " + Menu.save.birdMaxScore;
    }
    IEnumerator PlayDeathSound()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        audioSource.clip = dieSound;
        audioSource.Play();
    }
    public void Restart()
    {
        StartCoroutine(WaitToRestart());
    }
    IEnumerator WaitToRestart()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        BirdBackgroundSound.instance.ReloadScene();
    }
    public void ToMainMenu()
    {
        StartCoroutine(WaitToReturnToMenu());
    }
    IEnumerator WaitToReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public IEnumerator SpawnPipes()
    {
        while (Time.timeScale > 0)
        {
            GameObject pipe = Instantiate(pipePrefab, pipeSpawn);
            pipe.transform.position = new Vector2(pipeSpawn.position.x, pipeSpawn.position.y + UnityEngine.Random.Range(-pipeSpread, pipeSpread));
            pipe.GetComponent<Rigidbody2D>().AddForce(Vector2.left * pipeSpeed);
            createdPipes.Enqueue(pipe);
            if (createdPipes.Peek().transform.position.x < -4) Destroy(createdPipes.Dequeue());
            yield return new WaitForSeconds(timeToNewPipe);
        }
    }
    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }
    public void OnButtonPress()
    {
        audioSource.clip = Menu.instance.buttonPress;
        audioSource.Play();
    }
    public void OnButtonAllow()
    {
        audioSource.clip = Menu.instance.buttonAllow;
        audioSource.Play();
    }
    public void OnButtonDeny()
    {
        audioSource.clip = Menu.instance.buttonDeny;
        audioSource.Play();
    }
}
