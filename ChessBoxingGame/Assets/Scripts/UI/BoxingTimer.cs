using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxingTimer : RoundCountdown
{
    [SerializeField] public BoxingRound round;
    protected override void endTimer()
    {
        round.endRound();
    }
}