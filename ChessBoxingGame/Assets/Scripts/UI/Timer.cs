using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timeValue = 90f;

    [Header("ClockUI References")]
    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform pos3;
    [SerializeField] private Transform pos4;

    //Update is called once per frame
    void Update()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
            endTimer();
            this.enabled = false;
        }

        DisplayTime(timeValue);
    }

    protected virtual void endTimer()
    {
        Debug.Log("Over");
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        changeClockUI(string.Format("{0:00}:{1:00}", minutes, seconds));
    }

    void changeClockUI(string timer)
    {
        //prob a better way to do this but leaving for now

        if (pos1.GetChild(int.Parse(timer[0].ToString())).gameObject.activeSelf == false)
        {
            //If not, call changeValue
            changeValue(int.Parse(timer[0].ToString()), pos1);
        }

        if (pos2.GetChild(int.Parse(timer[1].ToString())).gameObject.activeSelf == false)
        {
            changeValue(int.Parse(timer[1].ToString()), pos2);
        }

        if (pos3.GetChild(int.Parse(timer[3].ToString())).gameObject.activeSelf == false)
        {
            changeValue(int.Parse(timer[3].ToString()), pos3);
        }

        if (pos4.GetChild(int.Parse(timer[4].ToString())).gameObject.activeSelf == false)
        {
            changeValue(int.Parse(timer[4].ToString()), pos4);
        }
    }

    void changeValue(int val, Transform pos)
    {
        //Disable everything
        foreach (Transform child in pos)
        {
            child.gameObject.SetActive(false);
        }

        //Turn on desired value
        pos.GetChild(val).gameObject.SetActive(true);
    }
}