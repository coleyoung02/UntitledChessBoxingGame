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
        string playerText = Talk.text;
        //string[] playerTextArray = playerText.Split(' ');
        
        //for(int i = 0; i < playerTextArray.Length; i++)
        //{
            if (playerText.Contains(keyword1) || playerText.Contains(keyword2))
            {
                WordDetected();
                Debug.Log("trash talk detected");
            }
        //}
        
    }

    public void WordDetected()
    {
        //DamageEnemy();
    }
}



//case sensitive
//use an array maybe
