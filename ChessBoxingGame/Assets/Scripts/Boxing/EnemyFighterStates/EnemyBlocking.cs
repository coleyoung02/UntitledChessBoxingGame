using System;
using System.Collections;
using UnityEngine;

public class EnemyBlocking : State
{
    private static readonly int GoBlocking = Animator.StringToHash("GoBlocking");

    public EnemyBlocking(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_BLOCKING;
    }

    public override void enter()
    {
        //fighter.StartCoroutine(startBlocking());
        base.enter();
        anim.SetBool(GoBlocking, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoBlocking, false);
    }

    public IEnumerator startBlocking()
    {
        float blocking_time = (UnityEngine.Random.Range(Constants.Enemy.BLOCKING_TIME_MIN, Constants.Enemy.BLOCKING_TIME_MAX) + UnityEngine.Random.Range(Constants.Enemy.BLOCKING_TIME_MIN, Constants.Enemy.BLOCKING_TIME_MAX)) / 2f;
        yield return new WaitForSeconds(blocking_time);
        if (fighter.currentState == this)
        {
            //should pick a random of idle, heavy, or light punch
            nextState = new EnemyIdle(anim, player, (EnemyFighter)fighter);
            stage = EVENT.EXIT;
        }
    }

    public void goLightPunching()
    {
        nextState = new EnemyLightPunching(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public void goPreCombo()
    {
        nextState = new EnemyPreCombo(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    public void goHeavyPunching(int numPunches = 1)
    {
        nextState = new EnemyHeavyPunchingFst(anim, player, (EnemyFighter)fighter, numPunches);
        stage = EVENT.EXIT;
    }

    public void goIdle()
    {
        nextState = new EnemyIdle(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

}