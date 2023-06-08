using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] AudioMixer myMixer;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            myMixer.SetFloat("Music", scale(PlayerPrefs.GetFloat("musicVolume")));
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            myMixer.SetFloat("SFX", scale(PlayerPrefs.GetFloat("SFXVolume")));
        }
    }

    private static float scale(float volume)
    {
        return Mathf.Log10(volume) * 20;
    }


    public void onStart()
    {
        GameManagerClass gameManager = Resources.FindObjectsOfTypeAll<GameManagerClass>()[0];
        gameManager.ResetGame();
        SceneManager.LoadScene("ChessScene");
    }


}
