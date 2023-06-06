using System;
using System.Collections;
using UnityEngine;

public class EnemyFakeIdle : State
{
    private Coroutine coroutine;
    public EnemyFakeIdle(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_FAKEIDLE;
        coroutine = null;
    }

    public override void enter()
    {
        coroutine = fighter.StartCoroutine(startFakeIdle());
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

    public IEnumerator startFakeIdle()
    {
        yield return new WaitForSeconds(Constants.Enemy.FAKE_IDLE_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyBlocking(anim, player, (EnemyFighter)fighter);
            stage = EVENT.EXIT;
        }
    }
    public void goStunned()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new EnemyStunned(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }

}