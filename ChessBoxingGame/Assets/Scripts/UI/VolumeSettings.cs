using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("SFXVolume"))
            LoadVolume();
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value / 82;
        if (volume == 0)
        {
            myMixer.SetFloat("Music", -90f);
        }
        else
        {
            myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value / 82;
        if (volume == 0)
        {
            myMixer.SetFloat("SFX", -90f);
        }
        else
        {
            myMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume") * 82;
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume") * 82;

        SetMusicVolume();
        SetSFXVolume();
    }
}
