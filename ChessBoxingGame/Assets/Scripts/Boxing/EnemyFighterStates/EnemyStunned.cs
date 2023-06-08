using System;
using System.Collections;
using UnityEngine;

public class EnemyStunned : State
{
    private Coroutine coroutine;
    private static readonly int GoStunned = Animator.StringToHash("GoStunned");

    public EnemyStunned(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_STUNNED;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startStunned());
        base.enter();
        anim.SetBool(GoStunned, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoStunned, false);
    }
    
    public IEnumerator startStunned()
    {
        yield return new WaitForSeconds(Constants.Enemy.STUN_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyBlocking(anim, player, (EnemyFighter)fighter);
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