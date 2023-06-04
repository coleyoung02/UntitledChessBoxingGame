using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashTalk : MonoBehaviour
{
    public InputField Talk;
    string keyword1 = "?";
    string keyword2 = "fu";

    public void Button()
    {
        if(string.Equals(Talk.text, keyword1) || string.Equals(Talk.text, keyword2))
        {
            WordDetected();
            Debug.Log("trash talk detected");
        }
    }

    public void WordDetected()
    {
        //DamageEnemy();
    }
}
