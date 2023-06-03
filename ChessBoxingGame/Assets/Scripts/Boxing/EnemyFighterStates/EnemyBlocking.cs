using System;
using System.Collections;
using UnityEngine;

public class EnemyBlocking : State
{
    public EnemyBlocking(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_BLOCKING;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startBlocking());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
    }

    public IEnumerator startBlocking()
    {
        float blocking_time = UnityEngine.Random.Range(0.0f, 1.0f)*(Constants.Enemy.BLOCKING_TIME_MAX - Constants.Enemy.BLOCKING_TIME_MIN) + Constants.Enemy.BLOCKING_TIME_MIN;
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

    public void goHeavyPunching()
    {
        nextState = new EnemyHeavyPunchingFst(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

}