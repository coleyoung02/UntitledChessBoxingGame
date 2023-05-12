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
        Debug.Log("EnemyBlocking.enter()");
        fighter.StartCoroutine(startBlocking());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        Debug.Log("EnemyBlocking.exit()");
        base.exit();
    }

    public IEnumerator startBlocking()
    {
        yield return new WaitForSeconds(Constants.Enemy.BLOCKING_TIME);
        if (fighter.currentState == this)
        {
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