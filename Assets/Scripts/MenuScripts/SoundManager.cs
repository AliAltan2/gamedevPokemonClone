using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Source https://www.youtube.com/watch?v=BKCsH8mQ-lM and https://www.youtube.com/watch?v=yWCHaTwVblk
public class SoundManager : MonoBehaviour
{
[SerializeField] Slider volumeSlider;

    void Start() {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.7f);
            Load();
        }else
        {
            Load();
        }
    }
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }
}
