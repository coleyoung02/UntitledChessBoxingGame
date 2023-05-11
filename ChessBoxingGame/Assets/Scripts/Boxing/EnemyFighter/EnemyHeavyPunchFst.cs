using System;
using System.Collections;
using UnityEngine;

public class EnemyHeavyPunchingFst : State
{
    public EnemyHeavyPunchingFst(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player, _fighter)
    {
        name = STATE.E_HEAVYPUNCHINGFST;
    }

    public override void enter()
    {
        Debug.Log("EnemyHeavyPunchingFst.enter()");
        fighter.StartCoroutine(startHeavyPunchingFst());
        base.enter();
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        Debug.Log("EnemyHeavyPunchingFst.exit()");
        base.exit();
    }
    
    public IEnumerator startHeavyPunchingFst()
    {
        yield return new WaitForSeconds(Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyHeavyPunchingSnd(anim, player, (EnemyFighter)fighter); 
            stage = EVENT.EXIT;
        }
    }
    
    public void goStunned()
    {
        nextState = new EnemyStunned(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
    public override void goKO()
    {
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
}