using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI masterVol;
    [SerializeField]
    Slider masterSlider;
    [SerializeField]
    TextMeshProUGUI musicVol;
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    TextMeshProUGUI soundsVol;
    [SerializeField]
    Slider soundsSlider;
    [SerializeField]
    GameObject mainPanel;
    [SerializeField]
    GameObject settingsPanel;
    private void Start()
    {
        masterSlider.value = Menu.save.masterVolume;
        masterVol.text = "Master volume: " + Mathf.RoundToInt(Menu.save.masterVolume * 100);
        musicSlider.value = Menu.save.musicVolume;
        musicVol.text = "Music volume: " + Mathf.RoundToInt(Menu.save.musicVolume * 100);
        soundsSlider.value = Menu.save.soundsVolume;
        soundsVol.text = "Sounds volume: " + Mathf.RoundToInt(Menu.save.soundsVolume * 100);
    }
    public void SetVolumeMaster()
    {
        float value = masterSlider.value;
        Menu.instance.SetVolumeMaster(value);
        masterVol.text = "Master volume: " + Mathf.RoundToInt(value * 100);
    }
    public void SetVolumeMusic()
    {
        float value = musicSlider.value;
        Menu.instance.SetVolumeMusic(value);
        musicVol.text = "Music volume: " + Mathf.RoundToInt(value * 100);
    }
    public void SetVolumeSounds()
    {
        float value = soundsSlider.value;
        Menu.instance.SetVolumeSounds(value);
        soundsVol.text = "Sounds volume: " + Mathf.RoundToInt(value * 100);
    }
    public void UpdateTexts()
    {
        masterSlider.value = Menu.save.masterVolume;
        masterVol.text = "Master volume: " + Mathf.RoundToInt(Menu.save.masterVolume * 100);
        musicSlider.value = Menu.save.musicVolume;
        soundsVol.text = "Sounds volume: " + Mathf.RoundToInt(Menu.save.musicVolume * 100);
        soundsSlider.value = Menu.save.soundsVolume;
        musicVol.text = "Music volume: " + Mathf.RoundToInt(Menu.save.soundsVolume * 100);
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
