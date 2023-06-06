using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessPlayerClock : Timer
{
    private bool ticking;
    [SerializeField] Image piece;
    [SerializeField] Image outline;
    [SerializeField] bool isPlayer;

    private void Awake()
    {
        ticking = false;
    }

    protected override void endTimer()
    {
        if (isPlayer)
        {
            Resources.FindObjectsOfTypeAll<GameManagerClass>()[0].setWinner(GameManagerClass.Winner.ENEMY);
        }
        else
        {
            Resources.FindObjectsOfTypeAll<GameManagerClass>()[0].setWinner(GameManagerClass.Winner.PLAYER);
        }
        SceneManager.LoadScene("EndScreen");
    }
    public void toggle()
    {
        ticking = !ticking; 
    }

    public void setTicking(bool ticking)
    {
        this.ticking = ticking;
        if (ticking)
        {
            Color tempColor = piece.color;
            tempColor.a = 1f;
            this.piece.color = tempColor;
        }
        else 
        {
            Color tempColor = piece.color;
            tempColor.a = .3f;
            this.piece.color = tempColor;
        }
        this.outline.gameObject.SetActive(ticking);
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

    public override void FixedUpdate()
    {
        if (ticking)
        {
            base.FixedUpdate();
        }
    }

}