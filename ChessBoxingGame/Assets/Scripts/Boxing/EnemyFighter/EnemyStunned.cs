using System;
using System.Collections;
using UnityEngine;

public class EnemyStunned : State
{
    public EnemyStunned(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_STUNNED;
    }

    public override void enter()
    {
        Debug.Log("EnemyStunned.enter()");
        fighter.StartCoroutine(startStunned());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        Debug.Log("EnemyStunned.exit()");
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
    }
    
    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
}