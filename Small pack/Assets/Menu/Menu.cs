using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Audio;
public class Menu : MonoBehaviour
{
    public static Menu instance;
    public static SaveData save;
    [SerializeField]
    AudioMixer masterVol;
    [SerializeField]
    AudioSource mainMenuMusicSource;
    [SerializeField]
    AudioSource mainMenuSoundsSource;
    [SerializeField]
    AudioClip mainMenuMusic;
    public AudioClip buttonPress;
    public AudioClip buttonDeny;
    public AudioClip buttonAllow;
    private void Awake()
    {
        instance = this;
        LoadSave();
        masterVol.SetFloat("masterVol", Mathf.Log10(save.masterVolume) * 20);
        masterVol.SetFloat("musicVol", Mathf.Log10(save.musicVolume) * 20);
        masterVol.SetFloat("soundsVol", Mathf.Log10(save.soundsVolume) * 20);
        mainMenuMusicSource.clip = mainMenuMusic;
        mainMenuMusicSource.Play();
    }
    public void OnButtonPress()
    {
        mainMenuSoundsSource.clip = buttonPress;
        mainMenuSoundsSource.Play();
    }
    public void OnButtonAllow()
    {
        mainMenuSoundsSource.clip = buttonAllow;
        mainMenuSoundsSource.Play();
    }
    public void OnButtonDeny()
    {
        mainMenuSoundsSource.clip = buttonDeny;
        mainMenuSoundsSource.Play();
    }
    public static void SaveBird(int score)
    {
        if (save == null)
        {
            LoadSave();
        }
        if (save.birdMaxScore < score)
        {
            save.birdMaxScore = score;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
            bf.Serialize(file, save);
            file.Close();
            Debug.Log("Bird saved");
        }
    }
    public static void SaveTicTacToe(TicTacToeResult res)
    {
        if(save == null)
        {
            LoadSave();
        }
        if (res == TicTacToeResult.win)
        {
            save.ticTacToeWins++;
            save.ticTacToeTotalPlayed++;
        }
        if (res == TicTacToeResult.lose)
        {
            save.ticTacToeLoses++;
            save.ticTacToeTotalPlayed++;
        }
        if(res == TicTacToeResult.tie)
        {
            save.ticTacToeTotalPlayed++;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("TiTacToe saved");
    }
    public static void SaveTetris(int score)
    {
        if (save == null)
        {
            LoadSave();
        }
        if (score > save.tetrisMaxScore)
        {
            save.tetrisMaxScore = score;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Tetris saved");
    }
    public static void SaveClicker(DateTime lastPlayed, List<int> quantityOfMiners, int maxUpgrade, double amountOfMoney)
    {
        if (save == null)
        {
            LoadSave();
        }
        save.maxUpgrade = maxUpgrade;
        save.quantityOfMiners = quantityOfMiners;
        save.lastClickerDate = lastPlayed;
        save.clickerMoney = amountOfMoney;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        //Debug.Log("Clicker saved");
    }
    public static void LoadSave()
    {
        if (
        File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            save = (SaveData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
        }
        else
        {
            Debug.LogError("There is no save data!");
            save = new SaveData();
        }
    }
    public static void EraseData()
    {
        if (File.Exists(Application.persistentDataPath
          + "/save.dat"))
        {
            File.Delete(Application.persistentDataPath
              + "/save.dat");
            save = new SaveData();
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }
    //Play buttons
    #region
    public void PlayFlappyBird()
    {
        StartCoroutine(WaitToPlayFlappyBird());
    }
    IEnumerator WaitToPlayFlappyBird()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Flappy Bird");
    }
    public void PlayTicTacToe()
    {
        StartCoroutine(WaitToPlayTicTacToe());
    }
    IEnumerator WaitToPlayTicTacToe()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("TicTacToe");
    }
    public void PlayTetris()
    {
        StartCoroutine(WaitToPlayTetris());
    }
    IEnumerator WaitToPlayTetris()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Tetris");
    }
    public void PlayClicker()
    {
        StartCoroutine(WaitToPlayClicker());
    }
    IEnumerator WaitToPlayClicker()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Clicker");
    }
    #endregion
    public void SetVolumeMaster(float value)
    {
        if (value <= 0) value = 0.0001f;
        masterVol.SetFloat("masterVol", Mathf.Log10(value) * 20);


        if (save == null)
        {
            LoadSave();
        }
        save.masterVolume = value;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Sounds saved");
    }
    public void SetVolumeMusic(float value)
    {
        if (value <= 0) value = 0.0001f;
        masterVol.SetFloat("musicVol", Mathf.Log10(value) * 20);


        if (save == null)
        {
            LoadSave();
        }
        save.musicVolume = value;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Sounds saved");
    }
    public void SetVolumeSounds(float value)
    {
        if (value <= 0) value = 0.0001f;
        masterVol.SetFloat("soundsVol", Mathf.Log10(value) * 20);


        if (save == null)
        {
            LoadSave();
        }
        save.soundsVolume = value;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Sounds saved");
    }
    public void Exit()
    {
        StartCoroutine(WaitToExit());
    }
    IEnumerator WaitToExit()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
}
[Serializable]
public class SaveData
{
    [Header("Flappy bird save")]
    public int birdMaxScore = 0;
    [Header("TicTacToe save")]
    public int ticTacToeWins = 0;
    public int ticTacToeLoses = 0;
    public int ticTacToeTotalPlayed = 0;
    [Header("Tetris save")]
    public int tetrisMaxScore = 0;
    [Header("Clicker save")]
    public DateTime lastClickerDate = DateTime.MinValue;
    public List<int> quantityOfMiners = new List<int>();
    public int maxUpgrade = 0;
    public double clickerMoney = 0;
    [Header("Settings save")]
    public float masterVolume = 1;
    public float soundsVolume = 1;
    public float musicVolume = 1;
}
