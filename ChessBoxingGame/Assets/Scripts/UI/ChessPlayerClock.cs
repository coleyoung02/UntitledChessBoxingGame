using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChessPlayerClock : Timer
{
    private bool ticking;

    private void Awake()
    {
        ticking = false;
    }

    public void toggle()
    {
        ticking = !ticking; 
    }

    public void setTicking(bool ticking)
    {
        this.ticking = ticking;
    }

    public void setTime(float time)
    {
        this.timeValue = time;
        DisplayTime(this.timeValue);
    }

    public float getTime()
    {
        return this.timeValue;
    }

    public override void Update()
    {
        if (ticking)
        {
            base.Update();
        }
    }

}