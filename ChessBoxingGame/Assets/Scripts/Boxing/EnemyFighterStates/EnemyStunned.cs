using System;
using System.Collections;
using UnityEngine;

public class EnemyStunned : State
{
    private Coroutine coroutine;
    public EnemyStunned(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_STUNNED;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startStunned());
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
    
    public IEnumerator startStunned()
    {
        yield return new WaitForSeconds(Constants.Enemy.STUN_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyIdle(anim, player, (EnemyFighter)fighter);
            stage = EVENT.EXIT;
        }
        coroutine = null;
    }
    
    public override void goKO()
    {
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
}