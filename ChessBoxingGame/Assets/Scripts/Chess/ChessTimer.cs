using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessTimer : MonoBehaviour
{
    [SerializeField] private float timeRemained = 30f;
    void Update()
    {
        if (timeRemained > 0)
        {
            timeRemained -= Time.deltaTime;
        }
        else
        {
            timeRemained = 0;
            SceneManager.LoadScene("BoxingScene");
        }
    }
}

