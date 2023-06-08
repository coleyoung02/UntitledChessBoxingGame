using System;
using System.Collections;
using UnityEngine;

public class EnemyFakeIdle : State
{
    private Coroutine coroutine;
    private static readonly int GoIdle = Animator.StringToHash("GoIdle");

    public EnemyFakeIdle(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_FAKEIDLE;
        coroutine = null;
    }

    public override void enter()
    {
        coroutine = fighter.StartCoroutine(startFakeIdle());
        base.enter();
        anim.SetBool(GoIdle, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoIdle, false);
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