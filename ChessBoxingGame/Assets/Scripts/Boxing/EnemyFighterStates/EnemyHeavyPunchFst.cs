using System;
using System.Collections;
using UnityEngine;

public class EnemyHeavyPunchingFst : State
{

    private Coroutine coroutine;
    private int numPunches;
    private static readonly int GoHeavyPunchingFst = Animator.StringToHash("GoHeavyPunchingFst");

    public EnemyHeavyPunchingFst(Animator _anim, Transform _player, EnemyFighter _fighter, int numPunches=1) : base(_anim, _player, _fighter)
    {
        name = STATE.E_HEAVYPUNCHINGFST;
        this.numPunches = numPunches;
        coroutine = null;
    }

    public override void enter()
    {
        //Debug.Log("EnemyHeavyPunchingFst.enter()");
        coroutine = fighter.StartCoroutine(startHeavyPunchingFst());
        base.enter();
        anim.SetBool(GoHeavyPunchingFst, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoHeavyPunchingFst, false);
    }
    
    public IEnumerator startHeavyPunchingFst()
    {
        yield return new WaitForSeconds(Constants.Enemy.HEAVY_PUNCH_FST_TELE_TIME);
        if (fighter.currentState == this)
        {
            nextState = new EnemyHeavyPunchingSnd(anim, player, (EnemyFighter)fighter, numPunches); 
            stage = EVENT.EXIT;
        }
        coroutine = null;
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
        if (coroutine != null)
        {
            fighter.StopCoroutine(coroutine);
            coroutine = null;
        }
        nextState = new EnemyKO(anim, player, (EnemyFighter)fighter);
        stage = EVENT.EXIT;
    }
    
}