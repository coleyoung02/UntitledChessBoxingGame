using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChessRoundTimer : RoundCountdown
{
    [SerializeField] public ChessUI round;
    protected override void endTimer()
    {
        StartCoroutine(bellWait(round.endRound));
        round.Disable();
    }
}