using System;
using System.Collections;
using UnityEngine;

public class EnemyHeavyPunchingSnd : State
{
    private Coroutine coroutine;
    private int numPunches;
    private static readonly int GoHeavyPunchingSnd = Animator.StringToHash("GoHeavyPunchingSnd");

    public EnemyHeavyPunchingSnd(Animator _anim, Transform _player, EnemyFighter _fighter, int numPunches) : base(_anim, _player,
        _fighter)
    {
        name = STATE.E_HEAVYPUNCHINGSND;
        this.numPunches = numPunches;
        coroutine = null;
    }

    public override void enter()
    {
        fighter.StartCoroutine(startHeavyPunchingSnd());
        base.enter();
        anim.SetBool(GoHeavyPunchingSnd, true);
    }

    public override void update()
    {
        base.update();
    }

    public override void exit()
    {
        base.exit();
        anim.SetBool(GoHeavyPunchingSnd, false);
    }

    public IEnumerator startHeavyPunchingSnd()
    {
        yield return new WaitForSeconds(Constants.Enemy.HEAVY_PUNCH_SND_TELE_TIME);
        if (fighter.currentState == this)
        {
            fighter.doAttack(fighter.HeavyPunch);
            if (this.numPunches <= 1)
            {
                nextState = new EnemyFakeIdle(anim, player, (EnemyFighter)fighter);
            }
            else
            {
                nextState = new EnemyHeavyPunchingFst(anim, player, (EnemyFighter)fighter, numPunches - 1);
            }
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