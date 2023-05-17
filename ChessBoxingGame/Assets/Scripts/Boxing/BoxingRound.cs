using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingRound : MonoBehaviour
{
    bool KO;
    [SerializeField] private TimerCountdown timer;
    [SerializeField] private PlayerFighter player;
    [SerializeField] private EnemyFighter enemy;
    [SerializeField] private int roundNumber;

    void Win()
    {

    }

    void Lose()
    {

    }
    public void endRound()
    {
        
    }

    private void Update()
    {
        if(KO)
        {
            Win();
        }
        else
        {
            Lose();
        }
    }
}
