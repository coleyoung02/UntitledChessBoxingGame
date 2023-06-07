using System;
using System.Collections;
using UnityEngine;

public class EnemyHeavyPunchingSnd : State
{
    private Coroutine coroutine;
    private static readonly int GoHeavyPunchingSnd = Animator.StringToHash("GoHeavyPunchingSnd");

    public EnemyHeavyPunchingSnd(Animator _anim, Transform _player, EnemyFighter _fighter) : base(_anim, _player,
        _fighter)
    {
        name = STATE.E_HEAVYPUNCHINGSND;
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
            nextState = new EnemyFakeIdle(anim, player, (EnemyFighter)fighter);
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